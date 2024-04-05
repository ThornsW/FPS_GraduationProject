using System.Collections.Generic;
using UnityEngine;

/*
 * 脚步音效数据类
 */
// 创建菜单项
[CreateAssetMenu(menuName = "FPS/Footstep Audio Data")]
public class FootstepAudioData : ScriptableObject
{
    public List<FootstepAudio> FootstepAudios = new List<FootstepAudio>();
}

[System.Serializable]
public class FootstepAudio
{
    // 标签
    public string Tag;
    // 音效列表
    public List<AudioClip> AudioClips = new List<AudioClip>();
    // 走路的音效间隔
    public float WalkingInterval;
    // 奔跑的音效间隔
    public float SprintingInterval;
    // 蹲下的音效间隔
    public float CouchingInterval;
}
