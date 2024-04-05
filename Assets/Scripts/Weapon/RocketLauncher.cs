using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Weapon
{
    public class RocketLauncher : Firearms
    {
        private FPMouseLook mouseLook;
        private FirearmsLauncherAudioData firearmsLauncherAudioData;

        [Header("炮弹")] public SkinnedMeshRenderer ProjectileRenderer;
        public Transform ProjectilePrefab;
        public float AutoReloadDelay  = 0.5f; // 自动填充延迟时间
        public float ShowProjectileDelayTime = 1f; // 射出后隐藏炮管上炮弹模型时间

        protected override void Awake()
        {
            base.Awake();
            mouseLook = FindObjectOfType<FPMouseLook>();
            firearmsLauncherAudioData = (FirearmsLauncherAudioData)FirearmsAudioData;
            FirearmsReloadAudioSource.clip = firearmsLauncherAudioData.ReloadProjectile;
        }

        protected override void Shooting()
        {
            AnimatorStateInfo animatorInfo = GunAnimator.GetCurrentAnimatorStateInfo(2);
            if (animatorInfo.IsTag("ReloadAmmo")) return;
            if (CurrentAmmo <= 0) return;
            if (!IsAllowShooting()) return;
            CurrentAmmo--;

            MuzzleParticle.Play(); // 枪焰动画
            StartCoroutine(MuzzleFlashLightShine()); // 枪口火光
            GunAnimator.Play("Fire", IsAiming ? 1 : 0, 0); // 枪械开火后坐力动画

            CreateProjectile(); // 发射炮弹

            mouseLook.VibratingPerspective(); // 震动视角
            
            StartCoroutine(ShowProjectileDelayHandler());
            StartCoroutine(AutoReload());
            
            LastFireTime = Time.time;
        }

        protected override void Reload()
        {
            // 弹匣满时、没有备用弹药时不进行换弹
            if (CurrentAmmo == AmmoEachMag || CurrentMaxAmmoCarried == 0) return;
            // 避免换弹时可以开枪，先把子弹退回备用弹药
            var tmpCurrentAmmo = CurrentAmmo;
            CurrentAmmo = 0;
            CurrentMaxAmmoCarried += tmpCurrentAmmo;
            // 设置换弹动画触发器
            GunAnimator.SetTrigger("Reload");
            // 播放换弹声音
            FirearmsReloadAudioSource.Play();
            // 启动换弹进度检测协程
            StartCoroutine(CheckReloadAmmoAnimationEndHandler());
        }

        protected void CreateProjectile()
        {
            Instantiate(
                ProjectilePrefab,
                BulletSpawnPoint.transform.position,
                BulletSpawnPoint.transform.rotation);
        }

        // 使火箭筒上的炮弹模型消失一段时间后恢复，模拟发射后炮弹空缺
        private IEnumerator ShowProjectileDelayHandler()
        {
            ProjectileRenderer.GetComponent<SkinnedMeshRenderer>().enabled = false;
            if (CurrentMaxAmmoCarried <= 0) yield break;
            yield return new WaitForSeconds(ShowProjectileDelayTime);
            ProjectileRenderer.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
        
        // 发射后短暂延迟然后自动填充炮弹
        private IEnumerator AutoReload () {
            yield return new WaitForSeconds (AutoReloadDelay);
	        Reload();
        }
    }
}