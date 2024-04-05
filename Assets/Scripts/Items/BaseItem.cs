using UnityEngine;

namespace Scripts.Items
{
    public abstract class BaseItem : MonoBehaviour
    {
        public enum ItemType
        {
            Firearms,
            Attachment,
            Supplies,
            MissionKey,
            Collection,
            Other
        }

        public ItemType CurrentItemType;

        public int ItemId;

        public string ItemName;
    }
    
}