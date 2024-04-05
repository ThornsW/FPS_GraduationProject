using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpringUtility
{
    public Vector3 Values; // 代表当前的震动强度(相机角度改变幅度)

    private float frequency;
    private float damp;
    private Vector3 dampValues;

    public CameraSpringUtility(float frequency, float damp)
    {
        this.frequency = frequency;
        this.damp = damp;
    }

    public void UpdateSpring(float deltaTime, Vector3 target)
    {
        // 震动强度逐渐衰减，直到为 0
        Values -= deltaTime * frequency * dampValues;
        dampValues = Vector3.Lerp(dampValues, Values - target, damp * deltaTime);
    }
}
