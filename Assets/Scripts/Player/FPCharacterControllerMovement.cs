using System.Collections;
using UnityEngine;

/*
 * 角色控制器
 */
public class FPCharacterControllerMovement : MonoBehaviour
{
    private CharacterController characterController; // 角色控制器
    private Transform characterTransform;
    private Animator characterAnimator; // 动画控制器
    private WeaponManager weaponManager;
    
    private Vector3 movementDirection;
    private float velocity; // 角色速度

    public float WalkSpeed; // 行走速度
    public float SprintingSpeed; // 冲刺速度
    public float WalkSpeedWhenCrouched; // 下蹲行走速度
    public float SprintingSpeedWhenCrouched; // 下蹲冲刺速度
    public float Gravity = 9.8f; // 默认重力
    public float JumpHeight; // 跳跃高度
    public float CrouchHeight = 1f; // 蹲下高度

    private bool isCrouched; // 是否蹲下
    private float originHeight; // 原始高度

    // 角色状态枚举
    public enum State
    {
        Idle,
        Walking,
        Sprinting,
        Crouching,
        Jumping,
        Others
    }

    public State CharacterState = State.Idle;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterTransform = GetComponent<Transform>();
        originHeight = characterController.height;
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        // 初始化当前速度
        var tmpCurrentSpeed = WalkSpeed;

        if (characterController.isGrounded)
        {
            // 获取用户输入
            var inputHorizontal = Input.GetAxis("Horizontal");
            var inputVertical = Input.GetAxis("Vertical");
            // 将要移动的方向，转换为世界坐标系
            movementDirection = characterTransform.TransformDirection(new Vector3(inputHorizontal, 0, inputVertical));

            var currentVelocity = characterController.velocity;
            currentVelocity.y = 0; // 去掉纵向的速度
            velocity = currentVelocity.magnitude; // 速度的大小
            // 根据速度修改状态
            if (velocity == 0) CharacterState = State.Idle;
            if (velocity > 0 && velocity <= WalkSpeed) CharacterState = State.Walking;

            // 跳跃
            if (Input.GetButtonDown("Jump"))
            {
                movementDirection.y = JumpHeight;
                CharacterState = State.Jumping;
            }

            // 蹲下或起立
            if (Input.GetKeyDown(KeyCode.C))
            {
                var tmpCurrentHeight = isCrouched ? originHeight : CrouchHeight;
                StartCoroutine(DoCrouch(tmpCurrentHeight));
                isCrouched = !isCrouched;
            }

            // 计算人物移动速度
            if (isCrouched)
            {
                // 蹲下冲刺/行走
                tmpCurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeedWhenCrouched : WalkSpeedWhenCrouched;
                CharacterState = State.Crouching;
            }
            else
            {
                // 站立冲刺/行走 (瞄准状态下不可到达冲刺速度)
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (weaponManager.GetCarriedWeapon() != null 
                        && weaponManager.GetCarriedWeapon().IsAiming) return;
                    tmpCurrentSpeed = SprintingSpeed;
                    CharacterState = State.Sprinting;
                }
                else
                {
                    tmpCurrentSpeed = WalkSpeed;
                }
            }

            if (characterAnimator != null)
            {
                // 设置阻尼器时间，使得动画过渡更加顺畅丝滑
                characterAnimator.SetFloat("Velocity", velocity, 0.25f, Time.deltaTime);
            }
        }

        // 重力
        movementDirection.y -= Gravity * Time.deltaTime;
        // 角色移动
        characterController.Move(tmpCurrentSpeed * Time.deltaTime * movementDirection);
    }

    // 执行下蹲或起立
    private IEnumerator DoCrouch(float target)
    {
        float tmpCurrentHeight = 0;
        while (Mathf.Abs(characterController.height - target) > 0.1f)
        {
            yield return null;
            // 改变角色高度，丝滑过度
            characterController.height =
                Mathf.SmoothDamp(characterController.height, target,
                    ref tmpCurrentHeight, Time.deltaTime * 5);
        }
    }

    internal void SetAnimator(Animator _animator)
    {
        characterAnimator = _animator;
    }
}
