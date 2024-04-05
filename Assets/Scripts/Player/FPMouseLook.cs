using UnityEngine;

/*
 * 第一人称视角移动
 */
public class FPMouseLook : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField] private Transform CharacterTransform; // 传入角色整体组件对象
    private Vector3 cameraRotation; // 缓存上一帧的值

    public float DefaultMouseSensitivity = 3.9f; // 默认鼠标灵敏度
    public float CurrentMouseSensitivity; // 当前鼠标灵敏度
    public Vector2 MaximumAngle; // 最大角度限制
    public float SmoothTime; // 平滑参数

    public AnimationCurve RecoilCurve;
    public Vector2 RecoilRange;

    public float RecoilFadeOutTime = 0.3f;
    private float currentRecoilTime;
    private Vector2 currentRecoil;
    private CameraSpring cameraSpring;

    private void Start()
    {
        cameraTransform = transform;
        cameraSpring = GetComponentInChildren<CameraSpring>();
        CurrentMouseSensitivity = DefaultMouseSensitivity;
    }

    private void Update()
    {
        // 获取鼠标两个方向的运动增量
        var tmpMouseX = Input.GetAxis("Mouse X");
        var tmpMouseY = Input.GetAxis("Mouse Y");

        // 在原来角度的基础上进行变化（变化量=鼠标输入×灵敏度系数）
        cameraRotation.y += tmpMouseX * CurrentMouseSensitivity;
        cameraRotation.x -= tmpMouseY * CurrentMouseSensitivity;

        // 枪的后坐力
        CalculateRecoilOffset();
        cameraRotation.y += currentRecoil.y;
        cameraRotation.x -= currentRecoil.x;
        
        // 限制仰视和俯视的角度
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, MaximumAngle.x, MaximumAngle.y);

        // 此方法用于转换欧拉角Vector3(x,y,z)对应的四元数Quaternion
        var targetRotationX = Quaternion.Euler(cameraRotation.x, 0, 0);
        var targetRotationY = Quaternion.Euler(0, cameraRotation.y, 0);
        
        // 平滑转动
        // 相机沿着X轴转动
        cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, targetRotationX, SmoothTime * Time.deltaTime);
        // 角色身体沿着Y轴转动，相机Y轴方向的视角也随之转动
        CharacterTransform.localRotation = Quaternion.Slerp(CharacterTransform.localRotation, targetRotationY, SmoothTime * Time.deltaTime);
    }

    // 计算后坐力偏移
    private void CalculateRecoilOffset()
    {
        currentRecoilTime += Time.deltaTime;
        float tmp_RecoilFraction = currentRecoilTime / RecoilFadeOutTime;
        // 获得指定时间点的曲线值
        float tmp_RecoilValue = RecoilCurve.Evaluate(tmp_RecoilFraction);
        // 计算插值
        currentRecoil = Vector2.Lerp(Vector2.zero, currentRecoil, tmp_RecoilValue);
    }

    // 震动视角
    public void VibratingPerspective()
    {
        currentRecoil += RecoilRange;
        cameraSpring.StartCameraSpring();
        currentRecoilTime = 0;
    }

    // 修改鼠标灵敏度
    public void ChangeMouseSensitivity(float sensitivityValue)
    {
        CurrentMouseSensitivity = sensitivityValue;
    }
    
    // 还原鼠标灵敏度
    public void RestoreMouseSensitivity()
    {
        CurrentMouseSensitivity = DefaultMouseSensitivity;
    }
}