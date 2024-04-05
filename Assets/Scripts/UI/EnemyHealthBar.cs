using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image HealthBarSprite; // 血条填充图片
    private Transform player; // 玩家

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        // 使得血条UI始终朝向玩家
        transform.rotation = Quaternion.LookRotation(player.position - transform.position);
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        // 更新血条UI (修改图片填充)
        HealthBarSprite.fillAmount = currentHealth / maxHealth;
    }

    public void DisableBarUI()
    {
        gameObject.SetActive(false);
    }
}
