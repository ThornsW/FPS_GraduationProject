using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 角色生命系统
 */
public class PlayerHealthController : MonoBehaviour
{
    
    public int CurrentHealth; // 当前血量

    public int MaxHealth = 100; // 最大血量

    public Image BloodSplatterImage; // 受伤血迹图片

    public float BeginRecoveryTime; // 脱战后开始回血所需时间

    public float BreatheRecoverySpeed; // 呼吸回血速度系数
    
    private float timer; // 呼吸回血计时器

    public int GetCurrentHealth() => CurrentHealth;
    public int GetMaxHealth() => MaxHealth;
    
    void Start()
    {
        CurrentHealth = MaxHealth;
        BloodSplatterImage.enabled = false;
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        // 脱战后一段时间开始回血
        if (timer >= BeginRecoveryTime)
        {
            BreatheRecovery();
        }
        // 血量小于50%时屏幕出现血迹
        BloodSplatterImage.enabled = CurrentHealth <= MaxHealth * 0.5f;
    }

    // 受到伤害
    public void TakeDamage(int damageValue)
    {
        // 重置计数器
        timer = 0;
        // 扣除生命值
        CurrentHealth -= damageValue;
        if (CurrentHealth <= 0)
        {
            // 触发死亡
            Debug.Log("玩家死亡");
        }
    }

    // 增加生命值
    public void AddHealth(float addValue)
    {
        CurrentHealth += (int) addValue;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }

    // 呼吸回血
    private void BreatheRecovery()
    {
        if (CurrentHealth == MaxHealth) return;
        AddHealth(BreatheRecoverySpeed * Time.deltaTime);
    }
    
}
