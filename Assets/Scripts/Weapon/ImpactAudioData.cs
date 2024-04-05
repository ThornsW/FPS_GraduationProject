using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/Impact Audio Data")]
    public class ImpactAudioData : ScriptableObject
    {
        public List<ImpactAudioWithTag> ImpactAudioWithTags;
    }
    
    [System.Serializable]
    public class ImpactAudioWithTag
    {
        public string Tag;
        public List<AudioClip> ImpactAudioClips;
    }
}