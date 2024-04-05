using System;
using UnityEngine;

/*
 * 镜头震动
 */
public class CameraSpring : MonoBehaviour
{
    public float Frequency = 25; // 震动频率
    public float Damp = 15; // 回归视角中心速度系数

    public Vector2 MinRecoilRange; // 最小震动范围
    public Vector2 MaxRecoilRange; // 最大震动范围

    private CameraSpringUtility cameraSpringUtility;
    private Transform cameraSpringTransform;
    
    private void Start()
    {
        cameraSpringUtility = new CameraSpringUtility(Frequency, Damp);
        cameraSpringTransform = transform;
    }

    private void Update()
    {
        // 镜头震动就是通过改变相机的角度来实现的
        cameraSpringUtility.UpdateSpring(Time.deltaTime, Vector3.zero);
        cameraSpringTransform.localRotation = Quaternion.Slerp(cameraSpringTransform.localRotation,
            Quaternion.Euler(cameraSpringUtility.Values), Time.deltaTime * 10);
    }

    public void StartCameraSpring()
    {
        // 在限制的震动范围内，沿y轴和z轴转动方向随机数值进行震动
        cameraSpringUtility.Values = new Vector3(0,
            UnityEngine.Random.Range(MinRecoilRange.x, MaxRecoilRange.x),
            UnityEngine.Random.Range(MinRecoilRange.y, MaxRecoilRange.y));
    }
}
