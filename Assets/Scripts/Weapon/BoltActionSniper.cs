using System;
using System.Collections;
using Scripts.Weapon.Missile;
using UnityEngine;

namespace Scripts.Weapon
{
    public class BoltActionSniper : Firearms
    {
        private FPMouseLook mouseLook;

        [Header("瞄准镜镜内渲染")] public Material scopeRenderMaterial;
        public Color defaultColor; // 开镜且具有放大效果
        public Color fadeColor; // 瞄准镜变灰变暗

        [Header("开镜后的鼠标灵敏度")] public float MouseSensitivityAfterOpenScope;
        private FPMouseLook fpMouseLook;

        private int alreadyInsertAmmoNum; // 本次装弹行为已进装入的子弹数
        private static IEnumerator reloadBoltActionSniperAmmoCheckerCoroutine;
        private FirearmsMultipleReloadAudioData firearmsMultipleReloadAudioData;

        protected override void Awake()
        {
            base.Awake();
            mouseLook = FindObjectOfType<FPMouseLook>();
            scopeRenderMaterial.color = fadeColor;
            fpMouseLook = GameObject.FindWithTag("Player").GetComponentInChildren<FPMouseLook>();
            firearmsMultipleReloadAudioData = (FirearmsMultipleReloadAudioData)FirearmsAudioData;
        }

        // 重写瞄准方法
        public override void Aiming(bool _isAiming)
        {
            base.Aiming(_isAiming);
            // 狙击枪的镜内渲染
            if (_isAiming)
            {
                // 开镜并减小鼠标灵敏度
                scopeRenderMaterial.color = defaultColor;
                fpMouseLook.ChangeMouseSensitivity(MouseSensitivityAfterOpenScope);
            }
            else
            {
                // 关镜并恢复鼠标灵敏度
                scopeRenderMaterial.color = fadeColor;
                fpMouseLook.RestoreMouseSensitivity();
            }
        }

        protected override void Shooting()
        {
            AnimatorStateInfo animatorInfo = GunAnimator.GetCurrentAnimatorStateInfo(2);
            if (animatorInfo.IsTag("MultipleReload")) return;
            if (CurrentAmmo <= 0) return;
            if (!IsAllowShooting()) return;
            CurrentAmmo--;

            MuzzleParticle.Play(); // 枪焰动画
            StartCoroutine(MuzzleFlashLightShine()); // 枪口火光
            GunAnimator.Play("Fire", IsAiming ? 1 : 0, 0); // 枪械开火后坐力动画

            CreateBullet(); // 发射子弹
            
            mouseLook.VibratingPerspective(); // 震动视角

            CasingParticle.Play(); // 抛壳动画
            LastFireTime = Time.time;

            // 延迟播放开枪后拉栓的声音
            FirearmsReloadAudioSource.clip = firearmsMultipleReloadAudioData.ReloadOpen;
            FirearmsReloadAudioSource.PlayDelayed(0.3f);
        }

        protected override void Reload()
        {
            // 射击动作未完全结束不进行换弹
            AnimatorStateInfo baseLayerAnimatorInfo = GunAnimator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo aimLayerAnimatorInfo = GunAnimator.GetCurrentAnimatorStateInfo(1);
            if (baseLayerAnimatorInfo.IsName("Base Layer.Fire") || 
                aimLayerAnimatorInfo.IsName("Aim Layer.Fire")) return;
            // 弹匣满时、没有备用弹药时不进行换弹
            if (CurrentAmmo == AmmoEachMag || CurrentMaxAmmoCarried == 0) return;
            // 设置换弹动画触发器和声音
            GunAnimator.SetTrigger("Reload");
            // 保证协程运行
            if (reloadBoltActionSniperAmmoCheckerCoroutine == null)
            {
                reloadBoltActionSniperAmmoCheckerCoroutine = CheckBoltActionSniperReloadAmmoHandler();
                StartCoroutine(reloadBoltActionSniperAmmoCheckerCoroutine);
            }
            else
            {
                StopCoroutine(reloadBoltActionSniperAmmoCheckerCoroutine);
                reloadBoltActionSniperAmmoCheckerCoroutine = null;
                reloadBoltActionSniperAmmoCheckerCoroutine = CheckBoltActionSniperReloadAmmoHandler();
                StartCoroutine(reloadBoltActionSniperAmmoCheckerCoroutine);
            }
        }

        // 拉栓换弹控制协程
        protected IEnumerator CheckBoltActionSniperReloadAmmoHandler()
        {
            while (true)
            {
                yield return null;

                // 当前还需装填的子弹数
                int NeedInsertAmmoNum = AmmoEachMag - CurrentAmmo;

                // 获取动画播放进度数据
                AnimatorStateInfo animatorInfo = GunAnimator.GetCurrentAnimatorStateInfo(2);
                double playingProgressNumber = animatorInfo.normalizedTime;
                double playingIntegerPart = Math.Truncate(playingProgressNumber); // 播放进度整数部分
                double playingDecimalPart = playingProgressNumber - playingIntegerPart; // 播放进度小数部分

                // 播放每颗子弹装填的声音
                if ((animatorInfo.IsName("Reload Layer.Reload_Open") && playingDecimalPart > 0.9f)
                    || (animatorInfo.IsName("Reload Layer.Reload_Insert") && playingDecimalPart < 0.1f))
                {
                    FirearmsReloadAudioSource.clip = firearmsMultipleReloadAudioData.ReloadInsert;
                    FirearmsReloadAudioSource.Play();
                }

                if (animatorInfo.IsName("Reload Layer.Reload_Insert") && NeedInsertAmmoNum > 0)
                {
                    if (playingDecimalPart >= 0.4 && alreadyInsertAmmoNum < playingIntegerPart + 1)
                    {
                        alreadyInsertAmmoNum++;
                        CurrentAmmo++;
                        CurrentMaxAmmoCarried--;
                        if (CurrentMaxAmmoCarried == 0)
                        {
                            GunAnimator.SetInteger("NeedInsertAmmoNum", 0);
                        }
                        else
                        {
                            GunAnimator.SetInteger("NeedInsertAmmoNum", --NeedInsertAmmoNum);
                        }
                    }
                }

                // 重置已装填子弹数
                if (NeedInsertAmmoNum == 0)
                {
                    alreadyInsertAmmoNum = 0;
                }
            }
        }

        protected void CreateBullet()
        {
            // Instantiate方法用于创建一个新的游戏对象，并将它添加到场景中
            GameObject tmpBullet = Instantiate(BulletPrefab, BulletSpawnPoint.position, transform.rotation);
            // 添加tag
            tmpBullet.tag = "Bullet";
            // 子弹散射（改变子弹射出的角度）
            tmpBullet.transform.eulerAngles += CalculateSpreadOffset();
            var bulletScript = tmpBullet.AddComponent<Bullet>();
            bulletScript.ImpactPrefab = BulletImpactPrefab;
            bulletScript.ImpactAudioData = ImpactAudioData;
            bulletScript.BleedingEffectPrefab = BleedingEffectPrefab;
            bulletScript.BulletSpeed = 100;
        }
    }
}