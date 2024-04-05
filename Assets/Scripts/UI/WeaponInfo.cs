using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfo : MonoBehaviour
{
    [Tooltip("武器icon数据")] 
    public WeaponIconData WeaponIconConfigData;
    [Tooltip("武器信息显示面板")] 
    public GameObject WeaponInfoPanel;
    [Tooltip("武器名称")] 
    public TMP_Text WeaponNameText;
    [Tooltip("武器图标")] 
    public Image WeaponIconImage;
    [Tooltip("子弹数量")] 
    public TMP_Text CurrentAmmoCountText;
    public TMP_Text RemainingAmmoCountText;

    private void Start()
    {
        WeaponInfoPanel.SetActive(false);
    }

    public void ShowWeaponInfo(string armName, string weaponName)
    {
        foreach (var weaponIconDataElement in WeaponIconConfigData.WeaponLogoDataList)
        {
            if (armName.Equals(weaponIconDataElement.Name))
            {
                WeaponNameText.SetText(weaponName);
                WeaponIconImage.sprite = weaponIconDataElement.WeaponIconSprite;
                WeaponInfoPanel.SetActive(true);
                break;
            }
        }
    }

    public void HideWeaponInfo()
    {
        WeaponInfoPanel.SetActive(false);
    }

    // 更新武器信息 UI 的子弹数量
    public void UpdateAmmoInfo(int currentAmmo, int remainingAmmo)
    {
        CurrentAmmoCountText.SetText(currentAmmo.ToString());
        RemainingAmmoCountText.SetText(remainingAmmo.ToString());
    }
}
