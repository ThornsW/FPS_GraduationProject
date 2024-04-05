using UnityEngine;

public class FPMovement : MonoBehaviour
{
    private Transform characterTransform;

    private Rigidbody characterRigidbody;

    public float Speed; // 速度

    private bool isGrounded; // 是否着地

    public float Gravity; // 重力
    
    public float JumpHeight; // 跳跃高度
    
    void Start()
    {
        characterTransform = transform;
        characterRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            var tmpHorizontal = Input.GetAxis("Horizontal");
            var tmpVertical = Input.GetAxis("Vertical");

            // 获取当前的方向并转化为世界坐标系，同时乘以速度
            var tmpCurrentDirection = new Vector3(tmpHorizontal, 0, tmpVertical);
            tmpCurrentDirection = characterTransform.TransformDirection(tmpCurrentDirection);
            tmpCurrentDirection *= Speed;

            // 求出速度变化量
            var tmpCurrentVelocity = characterRigidbody.velocity;
            var tmpVelocityChange = tmpCurrentDirection - tmpCurrentVelocity;
            tmpVelocityChange.y = 0;
            
            // 给游戏对象施加力（速度变换模式）
            characterRigidbody.AddForce(tmpVelocityChange, ForceMode.VelocityChange);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                characterRigidbody.velocity = new Vector3(tmpCurrentVelocity.x, CalculateJumpHighSpeed(), tmpCurrentVelocity.z);
            }

        }

        // G = mg
        characterRigidbody.AddForce(new Vector3(0, -Gravity * characterRigidbody.mass, 0));
    }

    private float CalculateJumpHighSpeed()
    {
        // v = 2gh开根号
        return Mathf.Sqrt(2 * Gravity * JumpHeight);
    }

    private void OnCollisionStay(Collision other)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }

}
