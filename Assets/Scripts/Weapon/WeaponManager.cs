using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Items;
using Scripts.Weapon;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponManager : MonoBehaviour
{
    [Header("携带武器")]
    public Firearms MainWeapon; // 主武器
    public Firearms SecondaryWeapon; // 副武器
    private Firearms carriedWeapon; // 当前手持的武器
    
    [Header("拾取武器")]
    public List<Firearms> Arms = new List<Firearms>(); // 用于存储不同武器模型(带手)
    public Transform GunCameraTransform; // 枪械的Camera
    public float RaycastMaxDistance = 5; // 捡东西的最大检测距离
    public LayerMask CheckItemLayerMask; // 检查物品过滤用的层

    [SerializeField] private FPCharacterControllerMovement fpCharacterControllerMovement;
    private IEnumerator waitingForHolsterEndCoroutine;

    [Header("武器UI")]
    public PickWeaponHint PickWeaponHintUI; // 拾取武器提示UI
    public WeaponInfo WeaponInfoUI; // 当前武器信息显示UI
    
    [Header("手雷")]
    public float GrenadeSpawnDelay;
    public Transform GrenadePrefab;

    [Header("小刀近战")] 
    public float KnifeAttackDistance;
    public int KnifeAttackDamageValue;
    
    public Firearms GetCarriedWeapon() => carriedWeapon;

    private void Start()
    {
        // 主武器不为空
        if (MainWeapon)
        {
            carriedWeapon = MainWeapon;
            fpCharacterControllerMovement.SetAnimator(carriedWeapon.GunAnimator);
        }
    }

    private void Update()
    {
        if (PauseMenu.IsPaused) return;
        
        // 检查前方是否有物体
        CheckItem();

        if (!carriedWeapon) return;
        // 切换武器
        SwitchWeapon();

        // 按下鼠标左键开火
        if (Input.GetMouseButton(0))
        {
            carriedWeapon.HoldTrigger();
        }

        // 松开左键停止开火
        if (Input.GetMouseButtonUp(0))
        {
            carriedWeapon.ReleaseTrigger();
        }

        // 按下 R 换弹药
        if (Input.GetKeyDown(KeyCode.R))
        {
            carriedWeapon.ReloadAmmo();
        }

        // 按下鼠标右键进入机瞄状态
        if (Input.GetMouseButtonDown(1))
        {
            carriedWeapon.Aiming(true);
        }

        // 松开鼠标右键退出机瞄状态
        if (Input.GetMouseButtonUp(1))
        {
            carriedWeapon.Aiming(false);
        }
        
        // 拿下 T 键进行武器检视
        if (Input.GetKeyDown(KeyCode.T))
        {
            carriedWeapon.GunAnimator.SetTrigger("Inspect");
        }
        
        // 按下 G 键或鼠标滚轮投掷手雷
        if (Input.GetKeyDown (KeyCode.G) || Input.GetMouseButtonDown(2))
        {
            StartCoroutine (ThrowHandGrenade());
            carriedWeapon.GunAnimator.Play("GrenadeThrow", 0, 0.0f);
        }
        
        // 按下 E 键使用小刀近战攻击
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 在两种近战刀法中随机选择一种
            carriedWeapon.GunAnimator.Play(Random.Range(0f, 1f) < 0.5f ? "Knife Attack 1" : "Knife Attack 2", 0, 0f);
            StartCoroutine(KnifeAttackDamage());
        }
        
        // 更新子弹数
        WeaponInfoUI.UpdateAmmoInfo(carriedWeapon.GetCurrentAmmo(), 
            carriedWeapon.GetCurrentMaxAmmoCarried());
    }

    private void CheckItem()
    {
        // 用射线检测视野内可互动物体
        bool isItem = Physics.Raycast(GunCameraTransform.position,
            GunCameraTransform.forward,
            out RaycastHit raycastHitInfo,
            RaycastMaxDistance,
            CheckItemLayerMask);
        if (isItem)
        {
            var hasItem = raycastHitInfo.collider.TryGetComponent(out BaseItem tmpBaseItem);
            
            // 显示拾取武器提示
            if (tmpBaseItem is FirearmsItem tmpFirearmsItem)
            {
                PickWeaponHintUI.ShowWeaponHint(tmpFirearmsItem);
            }

            // 按 F 拾取武器/物品
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (hasItem)
                {
                    PickupWeapon(tmpBaseItem);
                    PickupAttachment(tmpBaseItem);
                }
            }
        }
        else
        {
            PickWeaponHintUI.HindWeaponHint();
        }
    }
    
    // 拾起武器
    private void PickupWeapon(BaseItem tmpBaseItem)
    {
        if (tmpBaseItem is FirearmsItem tmpFirearmsItem)
        {
            foreach (Firearms tmpArm in Arms)
            {
                if (tmpFirearmsItem.ArmsName.CompareTo(tmpArm.name) != 0) continue;
                // 替换主武器
                if (MainWeapon == null || (carriedWeapon == MainWeapon && SecondaryWeapon != null))
                {
                    MainWeapon = tmpArm;
                }
                // 替换副武器
                else
                {
                    SecondaryWeapon = tmpArm;
                }
                // 捡起武器后设置为当前手持武器
                SetCarriedWeapon(tmpArm);
            }
        }
    }

    // 拾起武器配件
    private void PickupAttachment(BaseItem _baseItem)
    {
        if (!(_baseItem is AttachmentItem tmpAttachmentItem)) return;
        switch (tmpAttachmentItem.CurrentAttachmentType)
        {
            case AttachmentItem.AttachmentType.Scope:
                foreach (Firearms.ScopeInfo tmpScopeInfo in carriedWeapon.ScopeInfos)
                {
                    if (tmpScopeInfo.ScopeName.CompareTo(tmpAttachmentItem.ItemName) != 0)
                    {
                        tmpScopeInfo.ScopeGameObject.SetActive(false);
                        continue;
                    }
                    tmpScopeInfo.ScopeGameObject.SetActive(true);
                    carriedWeapon.BaseIronSight.ScopeGameObject.SetActive(false);
                    carriedWeapon.SetCarriedScope(tmpScopeInfo);
                }
                break;
            case AttachmentItem.AttachmentType.Other:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // 切换武器
    private void SwitchWeapon()
    {
        // 数字键 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (MainWeapon == null) return;
            // 更换为主武器
            if (carriedWeapon == MainWeapon) return;
            if (carriedWeapon.gameObject.activeInHierarchy)
            {
                StartWaitingForHolsterEndCoroutine();
                carriedWeapon.GunAnimator.SetTrigger("Holster");
            }
            else
            {
                SetCarriedWeapon(MainWeapon);
            }
        }
        // 数字键 2
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (SecondaryWeapon == null) return;
            // 更换为副武器
            if (carriedWeapon == SecondaryWeapon) return;
            if (carriedWeapon.gameObject.activeInHierarchy)
            {
                StartWaitingForHolsterEndCoroutine();
                carriedWeapon.GunAnimator.SetTrigger("Holster");
            }
            else
            {
                SetCarriedWeapon(SecondaryWeapon);
            }
        }
        // 鼠标滚轮
        else if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (carriedWeapon == null || MainWeapon == null || SecondaryWeapon == null) return;
            StartWaitingForHolsterEndCoroutine();
            carriedWeapon.GunAnimator.SetTrigger("Holster");
        }
    }

    // 启动协程来判断收枪动画是否播放完成，然后进行枪支的切换
    private void StartWaitingForHolsterEndCoroutine()
    {
        if (waitingForHolsterEndCoroutine == null)
        {
            waitingForHolsterEndCoroutine = WaitingForHolsterEnd();
        }

        StartCoroutine(waitingForHolsterEndCoroutine);
    }

    private IEnumerator WaitingForHolsterEnd()
    {
        while (true)
        {
            AnimatorStateInfo animatorStateInfo = carriedWeapon.GunAnimator.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.IsTag("holster"))
            {
                if (animatorStateInfo.normalizedTime >= 0.9f)
                {
                    // 隐藏现在这把枪
                    // 替换目标枪支
                    var targetWeapon = carriedWeapon == MainWeapon ? SecondaryWeapon : MainWeapon;
                    SetCarriedWeapon(targetWeapon);
                    waitingForHolsterEndCoroutine = null;
                    yield break;
                }
            }

            yield return null;
        }
    }

    protected void SetCarriedWeapon(Firearms targetWeapon)
    {
        if (carriedWeapon)
        {
            carriedWeapon.gameObject.SetActive(false);
        }
        carriedWeapon = targetWeapon;
        carriedWeapon.gameObject.SetActive(true);
        fpCharacterControllerMovement.SetAnimator(carriedWeapon.GunAnimator);
        // 显示武器UI信息
        WeaponInfoUI.ShowWeaponInfo(targetWeapon.name, targetWeapon.WeaponName);
    }
    
    // 扔手雷
    private IEnumerator ThrowHandGrenade() {
        // 扔手雷延迟
        yield return new WaitForSeconds (GrenadeSpawnDelay);
        // 生成手雷预制体
        Instantiate(GrenadePrefab, 
            carriedWeapon.GrenadeSpawnPoint.position, 
            carriedWeapon.GrenadeSpawnPoint.rotation);
    }

    private IEnumerator KnifeAttackDamage()
    {
        while (true)
        {
            yield return null;
            
            // 获取动画播放进度数据
            AnimatorStateInfo animatorInfo = carriedWeapon.GunAnimator.GetCurrentAnimatorStateInfo(0);
            double playingProgressNumber = animatorInfo.normalizedTime;
            double playingIntegerPart = Math.Truncate(playingProgressNumber); // 播放进度整数部分
            double playingDecimalPart = playingProgressNumber - playingIntegerPart; // 播放进度小数部分

            // debug: 发射出一条射线检测是否攻击到了敌人
#if UNITY_EDITOR
            Debug.DrawRay(transform.position,
                transform.forward, Color.red);
#endif

            // 检测小刀攻击动画播放进度到大概40%时，进行攻击检测和造成伤害
            if ((animatorInfo.IsName("Base Layer.Knife Attack 1") ||
                 animatorInfo.IsName("Base Layer.Knife Attack 2")) &&
                playingDecimalPart >= 0.4)
            {
                // 当射线检测到敌人时，伤害才有效
                RaycastHit hit;
                if (Physics.Raycast(transform.position,
                        transform.forward, out hit, KnifeAttackDistance)
                    && hit.collider.CompareTag("Enemy"))
                {
                    // 调用敌人血量模块进行扣血
                    var enemy = hit.collider.gameObject.GetComponent<EnemyFeature>();
                    enemy.TakeDamage(KnifeAttackDamageValue);
                }
                yield break;
            }
        }
    }
}