using System;
using System.Collections;
using Scripts.Weapon.Missile;
using UnityEngine;

namespace Scripts.Weapon
{
    public class Shotgun : Firearms
    {
        private FPMouseLook mouseLook;

        private int alreadyInsertAmmoNum; // 本次装弹行为已进装入的子弹数
        private static IEnumerator reloadShotgunAmmoCheckerCoroutine;
        private FirearmsMultipleReloadAudioData firearmsMultipleReloadAudioData;

        [Header("每颗霰弹的钢珠数")]
        public int ShotNum; 

        protected override void Awake()
        {
            base.Awake();
            mouseLook = FindObjectOfType<FPMouseLook>();
            firearmsMultipleReloadAudioData = (FirearmsMultipleReloadAudioData)FirearmsAudioData;
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

            CreateShotShell(); // 发射霰弹

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
            if (reloadShotgunAmmoCheckerCoroutine == null)
            {
                reloadShotgunAmmoCheckerCoroutine = CheckShotgunReloadAmmoHandler();
                StartCoroutine(reloadShotgunAmmoCheckerCoroutine);
            }
            else
            {
                StopCoroutine(reloadShotgunAmmoCheckerCoroutine);
                reloadShotgunAmmoCheckerCoroutine = null;
                reloadShotgunAmmoCheckerCoroutine = CheckShotgunReloadAmmoHandler();
                StartCoroutine(reloadShotgunAmmoCheckerCoroutine);
            }
        }

        // 拉栓换弹控制协程
        protected IEnumerator CheckShotgunReloadAmmoHandler()
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

        protected void CreateShotShell()
        {
            // 在一瞬间发射出多颗钢珠
            for (int i = 0; i < ShotNum; i++)
            {
                // 每一颗钢珠特性就和别的枪械的普通子弹一样，也都可以造成伤害
                GameObject tmpBullet = Instantiate(BulletPrefab, BulletSpawnPoint.position, transform.rotation);
                tmpBullet.tag = "Bullet";
                // 子弹改变射出角度发生散射
                tmpBullet.transform.eulerAngles += CalculateSpreadOffset();
                var bulletScript = tmpBullet.AddComponent<Bullet>();
                bulletScript.ImpactPrefab = BulletImpactPrefab;
                bulletScript.ImpactAudioData = ImpactAudioData;
                bulletScript.BleedingEffectPrefab = BleedingEffectPrefab;
                bulletScript.BulletSpeed = 100;
            }
        }
    }
}