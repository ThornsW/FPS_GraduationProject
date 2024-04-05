using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    public RectTransform Reticle;
    public CharacterController CharacterController;

    public float OriginalSize;
    public float TargetSize;
    private float currentSize;

    private bool isMoving; // 移动状态
    private bool isShooting; // 射击状态

    private void Update()
    {
        // 走路时准心扩散
        isMoving = CharacterController.velocity.magnitude > 0;
        if (isMoving || isShooting)
        {
            Diffuse();
        }
        else
        {
            Shrink();
        }
        Reticle.sizeDelta = new Vector2(currentSize, currentSize);
        // 还原射击状态
        if (isShooting) SetShootingState(false);
    }

    // 准心扩散
    public void Diffuse()
    {
        currentSize = Mathf.Lerp(currentSize, TargetSize, Time.deltaTime * 5);
    }

    // 准心收缩
    public void Shrink()
    {
        currentSize = Mathf.Lerp(currentSize, OriginalSize, Time.deltaTime * 5);
    }

    public void SetShootingState(bool state)
    {
        isShooting = state;
    }
}
