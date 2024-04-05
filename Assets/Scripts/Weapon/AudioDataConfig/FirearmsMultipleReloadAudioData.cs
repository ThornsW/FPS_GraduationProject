using UnityEngine;

namespace Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/Firearms Multiple Reload Audio Data")]
    public class FirearmsMultipleReloadAudioData : FirearmsAudioData
    {
        public AudioClip ReloadOpen;
        public AudioClip ReloadInsert;
        public AudioClip ReloadClose;
    }
}