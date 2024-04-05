using Assets.ProgressBars.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ProgressBars.Editor
{
	[CustomEditor(typeof(GuiProgressBarUI), true)]
	public class ProgressBarEditorUi : UnityEditor.Editor {

		private GuiProgressBarUI _pb;
		bool _showKnobSettings;
		bool _showTextSettings;

		public override void OnInspectorGUI()
		{
			if(_pb == null) _pb = (GuiProgressBarUI)target;



			_pb.MainSprite = EditorGUILayout.ObjectField("Progress bar sprite", _pb.MainSprite, typeof(Sprite), true) as Sprite;
			_pb.TextureWrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup ("Wrap mode", _pb.TextureWrapMode);
			_pb.MaskSprite = EditorGUILayout.ObjectField("Mask sprite", _pb.MaskSprite, typeof(Sprite), true) as Sprite;

			EditorGUILayout.Space ();

			_showKnobSettings = EditorGUILayout.Foldout (_showKnobSettings, "Knob settings");
			EditorGUI.indentLevel += 1;
			if (_showKnobSettings) {
                _pb.Knob = EditorGUILayout.ObjectField("Transform", _pb.Knob, typeof(RectTransform), true) as RectTransform;
				_pb.KnobPositionOffset = EditorGUILayout.FloatField ("Position offset", _pb.KnobPositionOffset);
				_pb.KnobMinPercent = Mathf.Clamp(EditorGUILayout.FloatField ("Min percent", _pb.KnobMinPercent), 0f, 1f);
				_pb.KnobMaxPercent = Mathf.Clamp(EditorGUILayout.FloatField ("Max percent", _pb.KnobMaxPercent), 0f, 1f);
			}
			EditorGUI.indentLevel -= 1;

			_showTextSettings = EditorGUILayout.Foldout (_showTextSettings, "Text settings");

			EditorGUI.indentLevel += 1;
			if (_showTextSettings) {
                _pb.TextMesh = EditorGUILayout.ObjectField("Text", _pb.TextMesh, typeof(Text), true) as Text;
				_pb.DigitsAfterComma = EditorGUILayout.IntField("Digits after comma", _pb.DigitsAfterComma);
				_pb.TextSuffix = EditorGUILayout.TextField("Suffix", _pb.TextSuffix);
				_pb.TextIndication = (TextIndicationType)EditorGUILayout.EnumPopup ("Indication type", _pb.TextIndication);
				_pb.SectorsCount = EditorGUILayout.IntField("Sectors count", _pb.SectorsCount);
			}

			EditorGUI.indentLevel -= 1;

			EditorGUILayout.Space ();
			_pb.Value = EditorGUILayout.Slider (_pb.Value, 0, 1);
			EditorGUILayout.Space ();

			if(GUI.changed)
				EditorUtility.SetDirty(_pb); 
		}
	}
}
