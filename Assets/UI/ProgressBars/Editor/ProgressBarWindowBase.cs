using UnityEditor;
using UnityEngine;

namespace Assets.ProgressBars.Editor
{
    public class ProgressBarWindowBase:EditorWindow
    {
        protected string PrefabsPath { get; set; }
        protected string WindowTitle { get; set; }
        protected string GameObjectName { get; set; }

        private Vector2 _pos = Vector2.zero;

        // ReSharper disable once UnusedMember.Local
        void OnGUI()
        {
            titleContent.text = WindowTitle;
            const int padding = 5;
            int x = padding, y = padding;
            int width = Screen.width - padding;
            const int spacing = 80 + padding;

            _pos = GUILayout.BeginScrollView(_pos);
            {
                for (int i = 1; i < 18; ++i)
                {
                    var rect = new Rect(x, y, 80, 80);
                    var buttonRect = new Rect(x-2, y-2, 84, 84);

                    if (GUI.Button(buttonRect, "Button"))
                    {
                        InstantiateProgressBar(i);
                    }

                    string previewPath = string.Format("Assets/ProgressBars/Editor/PrefabPreviews/progress-bar-preview-{0}.png", i);
                
                    Texture tex =(Texture2D)AssetDatabase.LoadAssetAtPath(previewPath, typeof(Texture2D));

                    GUI.DrawTexture(rect, tex);

                    x += spacing;
				
                    if (x + spacing > width)
                    {
                        y += spacing;
                        x = padding;
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        protected virtual void SetProgressBarParent(Transform barTransform)
        {
            //do nothing
        }

        private void InstantiateProgressBar (int progressBarIndex)
        {
            var index = progressBarIndex;

            var progressBar = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath(string.Format(PrefabsPath, index), typeof(GameObject)));

            SetProgressBarParent(progressBar.transform); 
            progressBar.name = string.Format(GameObjectName, index);
        }
    }
}