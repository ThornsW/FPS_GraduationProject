namespace Scripts.Items
{
    public class FirearmsItem : BaseItem
    {
        public enum FirearmsType
        {
            AssaultRifle,
            HandGun,
            SMG,
            Sniper,
            Shotgun,
            RocketLauncher,
        }

        public FirearmsType CurrentFirearmsType;
        
        public string ArmsName;
    }
}