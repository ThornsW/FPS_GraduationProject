using UnityEngine;

namespace Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/Firearms Mag Reload Audio Data")]
    public class FirearmsMagReloadAudioData : FirearmsAudioData
    {
        public AudioClip ReloadLeft;
        public AudioClip ReloadOutOf;
    }
}
