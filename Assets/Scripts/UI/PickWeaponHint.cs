using System;
using System.Collections.Generic;
using Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickWeaponHint : MonoBehaviour
{
    [Tooltip("武器icon数据")] 
    public WeaponIconData WeaponIconConfigData;
    [Tooltip("武器提示面板")] 
    public GameObject WeaponHintPanel;
    [Tooltip("提示文字")] 
    public TMP_Text HintText;
    [Tooltip("武器icon图")] 
    public Image WeaponIconImage;
    [Tooltip("提示文字前缀")] 
    public static string HintTextPreStr = "按 F 拾取武器 ";

    private void Start()
    {
        WeaponHintPanel.SetActive(false);
    }

    public void ShowWeaponHint(FirearmsItem firearmsItem)
    {
        foreach (var weaponIconDataElement in WeaponIconConfigData.WeaponLogoDataList)
        {
            if (firearmsItem.ArmsName.Equals(weaponIconDataElement.Name))
            {
                HintText.SetText(HintTextPreStr + firearmsItem.ItemName);
                WeaponIconImage.sprite = weaponIconDataElement.WeaponIconSprite;
                WeaponHintPanel.SetActive(true);
                break;
            }
        }
    }

    public void HindWeaponHint()
    {
        WeaponHintPanel.SetActive(false);
    }
}