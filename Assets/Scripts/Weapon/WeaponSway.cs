using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 移动视角时武器的摆动
 */
public class WeaponSway : MonoBehaviour
{
    public bool weaponSway = true; // 武器摆动开关

    public float swayAmount = 0.02f; // 摆动系数
    public float maxSwayAmount = 0.06f; // 摆动幅度上限
    public float swaySmoothValue = 4.0f; // 平滑系数

    private Vector3 initialSwayPosition; // 武器初始位置

    void Start()
    {
        initialSwayPosition = transform.localPosition;
    }

    void Update()
    {
        // 如果开启了武器摆动
        if (weaponSway)
        {
            float movementX = -Input.GetAxis("Mouse X") * swayAmount;
            float movementY = -Input.GetAxis("Mouse Y") * swayAmount;
            // 限制摆动范围
            movementX = Mathf.Clamp
                (movementX, -maxSwayAmount, maxSwayAmount);
            movementY = Mathf.Clamp
                (movementY, -maxSwayAmount, maxSwayAmount);
            // 线性插值来平滑过渡
            Vector3 finalSwayPosition = new Vector3
                (movementX, movementY, 0);
            transform.localPosition = Vector3.Lerp
            (transform.localPosition,
                initialSwayPosition + finalSwayPosition,
                Time.deltaTime * swaySmoothValue);
        }
    }
}