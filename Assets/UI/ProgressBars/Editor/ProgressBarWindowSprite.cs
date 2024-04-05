using UnityEditor;
using UnityEngine;

namespace Assets.ProgressBars.Editor
{
    public class ProgressBarWindowSprite:ProgressBarWindowBase
    {
        [MenuItem("Progress Bars/Sprite progress bars")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProgressBarWindowSprite>();
            window.PrefabsPath = "Assets/ProgressBars/Prefabs/ProgressBar{0}.prefab";
            window.WindowTitle = "Sprite progress bars";
            window.GameObjectName = "ProgressBar{0}";
        }
    }
}