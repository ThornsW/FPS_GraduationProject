using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Tooltip("血条")] 
    public Slider HealthBarSlider;
    [Tooltip("血条显示文字")] 
    public TMP_Text HealthValueText;
    [Tooltip("血条变化速度")] 
    public float Speed;
    [Tooltip("玩家血量模块")] 
    public PlayerHealthController PlayerHealth;

    private void Update()
    {
        UpdateHealthBar(PlayerHealth.GetCurrentHealth(), 
            PlayerHealth.GetMaxHealth());
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        var currentSliderValue = HealthBarSlider.value;
        var targetValue = currentHealth;
        // 如果当前血条的数值已是人物的实际血量，则退出
        if ((int) currentSliderValue == targetValue) return;
        if (currentSliderValue < targetValue)
        {
            currentSliderValue += Speed;
            currentSliderValue = currentSliderValue > 100 ? 100 : currentSliderValue;
        }
        else
        {
            currentSliderValue -= Speed;
            currentSliderValue = currentSliderValue < 0 ? 0 : currentSliderValue;
        }
        // 更新血条UI
        HealthBarSlider.value = currentSliderValue;
        // 更新血量文字
        HealthValueText.SetText(HealthBarSlider.value + "%");
    }
}