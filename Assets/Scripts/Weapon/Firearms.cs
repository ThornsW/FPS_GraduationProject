using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 火器
 */
namespace Scripts.Weapon
{
    public abstract class Firearms : MonoBehaviour, IWeapon
    {
        [Header("武器名称")] 
        public string WeaponName;
        
        [Header("相机")] 
        public Camera EyeCamera;
        public Camera GunCamera;

        [Header("准心")] 
        private GameObject crosshair; // 准心UI
        private CrosshairUI crosshairUI; // 准心UI的脚本控制对象

        [Header("枪焰与抛壳特效")] 
        public ParticleSystem MuzzleParticle; // 枪口粒子特效
        public ParticleSystem CasingParticle; // 抛壳粒子特效
        public Transform BulletSpawnPoint; // 子弹出射口

        [Header("枪口火光光源")] 
        public Light MuzzleFlashLight;
        public float LightDuration = 0.02f;

        [Header("枪械音效")] 
        public AudioSource FirearmsShootingAudioSource; // 开枪声播放源
        public AudioSource FirearmsReloadAudioSource; // 换弹声播放源
        public FirearmsAudioData FirearmsAudioData; // 枪械声音音源
        public ImpactAudioData ImpactAudioData; // 子弹撞击物体的声音

        [Header("子弹")] 
        public float FireRate; // 射速 (1s发射的子弹数)
        public int Damage; // 子弹的伤害
        public int AmmoEachMag = 30; // 一个弹匣子弹数量
        public int MaxAmmoCarried = 120; // 最大子弹携带数量
        public GameObject BulletPrefab; // 子弹预制体
        public GameObject BulletImpactPrefab; // 子弹撞击效果预制体
        public GameObject BleedingEffectPrefab; // 出血效果预制体
        public float SpreadAngle = 0.05f; // 散射角度

        [Header("瞄具")] 
        public List<ScopeInfo> ScopeInfos; // 可用瞄具列表
        public ScopeInfo BaseIronSight; // 基础瞄具
        protected ScopeInfo rigoutScopeInfo; // 当前装备的瞄具

        [Header("手雷")]
        public Transform GrenadeSpawnPoint; // 手雷投掷点
        
        public int GetCurrentAmmo() => CurrentAmmo;
        public int GetCurrentMaxAmmoCarried() => CurrentMaxAmmoCarried;

        protected int CurrentAmmo; // 当前弹匣里的子弹数量
        protected int CurrentMaxAmmoCarried; // 当前携带最大子弹数

        protected float LastFireTime; // 上一次开火时间

        internal Animator GunAnimator; // 动画
        protected AnimatorStateInfo GunStateInfo; // 动画播放状态

        protected float EyeOriginFOV;
        protected float GunOriginFov;
        public bool IsAiming; // 是否处于瞄准状态
        protected bool IsHoldingTrigger; // 是否孔扣动扳机状态

        private Vector3 originalEyePosition;
        protected Transform gunCameraTransform;

        private static IEnumerator reloadAmmoCheckerCoroutine; // 处理子弹数量的协程
        private static IEnumerator doAimCoroutine; // 处理瞄准动作的协程

        protected virtual void Awake()
        {
            // 初始化子弹数量
            CurrentAmmo = AmmoEachMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            // 初始化动作控制器
            GunAnimator = GetComponent<Animator>();
            // 初始化音源
            FirearmsShootingAudioSource.clip = FirearmsAudioData.ShootingAudio;
            // FOV
            EyeOriginFOV = EyeCamera.fieldOfView;
            GunOriginFov = GunCamera.fieldOfView;
            // 初始化GunCamera
            gunCameraTransform = GunCamera.transform;
            originalEyePosition = gunCameraTransform.localPosition;
            // 默认瞄具赋值
            rigoutScopeInfo = BaseIronSight;
            // 准心控制脚本对象
            crosshair = GameObject.Find("Crosshair");
            crosshairUI = crosshair.GetComponent<CrosshairUI>();
            // 初始化火光光源
            MuzzleFlashLight.enabled = false;
        }

        public void DoAttack()
        {
            // 腰射时准心扩散
            if (!IsAiming && CurrentAmmo > 0)
            {
                crosshairUI.SetShootingState(true);
            }

            Shooting();
        }

        protected abstract void Shooting();
        protected abstract void Reload();

        // protected abstract void Aim();

        // 是否可开枪
        protected bool IsAllowShooting()
        {
            return Time.time - LastFireTime > 1 / FireRate;
        }

        // 计算散射偏移
        protected Vector3 CalculateSpreadOffset()
        {
            // 瞄准时，FOV变小，子弹散射程度变小
            float tmp_SpreadPercent = SpreadAngle * EyeCamera.fieldOfView;
            // insideUnitCircle 返回一个单位圆范围内的随机点
            return tmp_SpreadPercent * UnityEngine.Random.insideUnitCircle;
        }

        // 检查一个换弹动画是否播放完毕
        protected IEnumerator CheckReloadAmmoAnimationEndHandler()
        {
            while (true)
            {
                yield return null;
                GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(2);
                if (GunStateInfo.IsTag("ReloadAmmo"))
                {
                    if (GunStateInfo.normalizedTime >= 0.9f)
                    {
                        // 装弹成功
                        // 补满一个弹匣所需的弹药数
                        var suppleAmmo = AmmoEachMag - CurrentAmmo;
                        // 需要判断备用弹药是否充足
                        if (CurrentMaxAmmoCarried >= suppleAmmo)
                        {
                            CurrentAmmo = AmmoEachMag;
                            CurrentMaxAmmoCarried -= suppleAmmo;
                        }
                        else
                        {
                            CurrentAmmo += CurrentMaxAmmoCarried;
                            CurrentMaxAmmoCarried = 0;
                        }
                    }
                }
            }
        }

        // 瞄准时放大视角
        protected IEnumerator DoAimHandler()
        {
            while (true)
            {
                yield return null;
                float tmpEyeCurrentFOV = 0;
                EyeCamera.fieldOfView =
                    Mathf.SmoothDamp(EyeCamera.fieldOfView,
                        IsAiming ? rigoutScopeInfo.EyeFov : EyeOriginFOV,
                        ref tmpEyeCurrentFOV,
                        Time.deltaTime * 2);
                float tmpGunCurrentFOV = 0;
                GunCamera.fieldOfView =
                    Mathf.SmoothDamp(GunCamera.fieldOfView,
                        IsAiming ? rigoutScopeInfo.GunFov : GunOriginFov,
                        ref tmpGunCurrentFOV,
                        Time.deltaTime * 2);

                // GunCamera 平滑过渡到对应瞄具的机瞄视野的位置
                Vector3 tmpRefPosition = Vector3.zero;
                gunCameraTransform.localPosition = Vector3.SmoothDamp(gunCameraTransform.localPosition,
                    IsAiming ? rigoutScopeInfo.GunCameraPosition : originalEyePosition,
                    ref tmpRefPosition,
                    Time.deltaTime * 2);
            }
        }

        public virtual void Aiming(bool _isAiming)
        {
            IsAiming = _isAiming;
            GunAnimator.SetBool("Aim", IsAiming);

            // 瞄准时隐藏十字准心
            crosshair.SetActive(!IsAiming);

            // 保证协程运行
            if (doAimCoroutine == null)
            {
                doAimCoroutine = DoAimHandler();
                StartCoroutine(doAimCoroutine);
            }
            else
            {
                StopCoroutine(doAimCoroutine);
                doAimCoroutine = null;
                doAimCoroutine = DoAimHandler();
                StartCoroutine(doAimCoroutine);
            }
        }

        internal void SetCarriedScope(ScopeInfo _scopeInfo)
        {
            rigoutScopeInfo = _scopeInfo;
        }

        internal void HoldTrigger()
        {
            DoAttack();
            IsHoldingTrigger = true;
        }

        internal void ReleaseTrigger()
        {
            IsHoldingTrigger = false;
        }

        internal void ReloadAmmo()
        {
            // 保证协程运行
            if (reloadAmmoCheckerCoroutine == null)
            {
                reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEndHandler();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }
            else
            {
                StopCoroutine(reloadAmmoCheckerCoroutine);
                reloadAmmoCheckerCoroutine = null;
                reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEndHandler();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }

            Reload();
        }

        protected IEnumerator MuzzleFlashLightShine()
        {
            MuzzleFlashLight.enabled = true;
            yield return new WaitForSeconds(LightDuration);
            MuzzleFlashLight.enabled = false;
        }

        // 倍镜信息
        [System.Serializable]
        public class ScopeInfo
        {
            public string ScopeName; // 倍镜名称
            public GameObject ScopeGameObject; // 倍镜游戏对象
            public float EyeFov; // 眼部视野
            public float GunFov; // 举枪视野
            public Vector3 GunCameraPosition; // 相机的位置
        }
    }
}