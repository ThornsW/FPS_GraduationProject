using UnityEngine;

/*
 * 角色脚步声
 */
public class PlayerFootstepListener : MonoBehaviour
{
    public FootstepAudioData FootstepAudioData;
    public AudioSource FootstepAudioSource;
    public LayerMask LayerMask; // 层
    
    private CharacterController characterController;
    private Transform footstepTransform;
    private FPCharacterControllerMovement fpCharacterControllerMovement;
    
    // 下次播放的时间
    private float nextPlayTime;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        footstepTransform = GetComponent<Transform>();
        fpCharacterControllerMovement = characterController.GetComponent<FPCharacterControllerMovement>();
    }

    private void FixedUpdate()
    {
        // 时间累加
        nextPlayTime += Time.deltaTime;
        if (characterController.isGrounded)
        {
            // 当角色开始移动
            if (characterController.velocity.normalized.magnitude >= 0.1f)
            {
                // 用来检测角色是否触地的最低端，也就是角色的脚
                var detectEnd = footstepTransform.position + Vector3.down * (characterController.height / 2 +
                                                                             characterController.skinWidth +
                                                                             Mathf.Abs(characterController.center.y));
                // 是否碰到地面
                bool isHit = Physics.Linecast(footstepTransform.position, detectEnd, out RaycastHit hitInfo, LayerMask);

                // debug: 用红线标识落脚处，只在unity编辑界面中有效
#if UNITY_EDITOR
                Debug.DrawLine(footstepTransform.position, detectEnd, Color.red, 0.25f);
#endif

                // 检测到碰到地面
                if (isHit)
                {
                    RandomPlayHitAudio(hitInfo);
                }
            }
        }
    }

    // 随机播放被碰撞材质的其中一种音效
    private void RandomPlayHitAudio(RaycastHit hitInfo)
    {
        float playInterval = 0;
        foreach (var audioElement in FootstepAudioData.FootstepAudios)
        {
            // 检测碰到的材质类型、声音播放间隔
            if (hitInfo.collider.CompareTag(audioElement.Tag))
            {
                // 根据角色的不同状态，更改音效播放间隔
                switch (fpCharacterControllerMovement.CharacterState)
                {
                    case FPCharacterControllerMovement.State.Walking:
                        playInterval = audioElement.WalkingInterval;
                        break;
                    case FPCharacterControllerMovement.State.Sprinting:
                        playInterval = audioElement.SprintingInterval;
                        break;
                    case FPCharacterControllerMovement.State.Crouching:
                        playInterval = audioElement.CouchingInterval;
                        break;
                    case FPCharacterControllerMovement.State.Jumping:
                        playInterval = 0;
                        break;
                }
                
                if (nextPlayTime >= playInterval)
                {
                    // 随机播放多种此材质碰撞的声音之一
                    var audioCount = audioElement.AudioClips.Count;
                    var audioIndex = UnityEngine.Random.Range(0, audioCount);
                    AudioClip footstepAudioClip = audioElement.AudioClips[audioIndex];
                    FootstepAudioSource.clip = footstepAudioClip;
                    FootstepAudioSource.Play();
                    // 时间重置为 0
                    nextPlayTime = 0;
                    break;
                }
            }
        }
    }
}