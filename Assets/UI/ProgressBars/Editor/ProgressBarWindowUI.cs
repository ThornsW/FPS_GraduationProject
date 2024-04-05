using UnityEditor;
using UnityEngine;

namespace Assets.ProgressBars.Editor
{
    public class ProgressBarWindowUi:ProgressBarWindowBase 
    {
        [MenuItem("Progress Bars/UI progress bars")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProgressBarWindowUi>();
            window.PrefabsPath = "Assets/ProgressBars/Prefabs/UI/ProgressbarUI{0}.prefab";
            window.WindowTitle = "UI progress bars";
            window.GameObjectName = "ProgressBarUI{0}";
        }

        protected override void SetProgressBarParent(Transform barTransform)
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                barTransform.transform.SetParent(canvas.transform);
            }
        }
    }
}