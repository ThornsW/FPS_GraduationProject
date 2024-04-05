using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace TMPro
{
	[RequireComponent(typeof(TMP_Text))]
	[ExecuteInEditMode]
	public class TextMeshProEffect : MonoBehaviour
	{
		public enum EffectType : byte
		{
			Waves,
			Grow,
			Unfold,
			UnfoldAndWaves,
			Sketch,
			Bounce
		}

		public enum MixType : byte
		{
			Multiply,
			Add
		}

		[ExecuteInEditMode]
		internal class SharedState : MonoBehaviour
		{
			internal bool TextMeshIsUpdated;

			private void LateUpdate()
			{
				TextMeshIsUpdated = false;
			}
		}

		[Serializable]
		private sealed class öù
		{
			public static readonly öù öƒ = new öù();

			public static Func<TextMeshProEffect, bool> öá;

			internal bool öÿ(TextMeshProEffect öÜ)
			{
				return öÜ == null || !öÜ.enabled;
			}
		}

		[StructLayout(LayoutKind.Auto)]
		private struct öí
		{
			public TMP_CharacterInfo öú;

			public TextMeshProEffect öñ;
		}

		[StructLayout(LayoutKind.Auto)]
		private struct öÑ
		{
			public float öª;

			public TMP_CharacterInfo òo;

			public TextMeshProEffect òο;
		}

		[StructLayout(LayoutKind.Auto)]
		private struct òо
		{
			public float òô;

			public TMP_CharacterInfo òö;

			public TextMeshProEffect òò;
		}

		public EffectType Type;

		public float DurationInSeconds = 0.5f;

		public float Amplitude = 1f;

		[Space]
		[Range(0f, 1f)]
		public float CharacterDurationRatio = 0f;

		public int CharactersPerDuration = 0;

		[Space]
		public Gradient Gradient = new Gradient();

		public MixType Mix = MixType.Multiply;

		[Space]
		public bool AutoPlay = true;

		public bool Repeat;

		public string ForWords;

		private readonly List<(int from, int to)> öà = new List<(int, int)>();

		[HideInInspector]
		public bool IsFinished;

		private float öå;

		private TMP_Text öç;

		private EffectType öê;

		private bool öë;

		private bool öè;

		private bool öï;

		private float öî;

		private string öì;

		private ushort öÄ;

		private float[] öÅ = new float[10];

		private SharedState öÉ;

		private float öæ;

		private TMP_TextInfo öÆ;

		private string öû;

		public List<(int from, int to)> Intervals => öà;

		private SharedState SharedStateProp
		{
			get
			{
				if (öÉ != null)
				{
					return öÉ;
				}
				öÉ = GetComponent<SharedState>();
				if (öÉ == null)
				{
					öÉ = base.gameObject.AddComponent<SharedState>();
					öÉ.hideFlags = (HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild);
				}
				return öÉ;
			}
		}

		public void CopyTo(TextMeshProEffect effect)
		{
			effect.Type = Type;
			effect.DurationInSeconds = DurationInSeconds;
			effect.Amplitude = Amplitude;
			effect.CharacterDurationRatio = CharacterDurationRatio;
			effect.CharactersPerDuration = CharactersPerDuration;
			effect.Gradient = Gradient;
			effect.Mix = Mix;
			effect.AutoPlay = AutoPlay;
			effect.Repeat = Repeat;
			effect.ForWords = ForWords;
		}

		public void Apply()
		{
			öç = GetComponent<TMP_Text>();
			öê = Type;
			öë = (öê == EffectType.Unfold || öê == EffectType.Grow || öê == EffectType.Bounce);
			öè = (öê == EffectType.Sketch);
			öï = false;
			öî = -1f;
		}

		private void OnEnable()
		{
			if (AutoPlay)
			{
				Play();
			}
		}

		private void OnDestroy()
		{
			öÉ = GetComponent<SharedState>();
			if (!(öÉ == null))
			{
				TextMeshProEffect[] components = base.gameObject.GetComponents<TextMeshProEffect>();
				if (components.Length == 0 || components.All(öù.öƒ.öÿ))
				{
					UnityEngine.Object.Destroy(öÉ);
				}
			}
		}

		private void OnValidate()
		{
			if (AutoPlay)
			{
				Play();
			}
			else
			{
				Apply();
			}
		}

		private void LateUpdate()
		{
			if ((UnityEngine.Object)(object)öç == null || DurationInSeconds <= 0f || !öï)
			{
				return;
			}
			if (Repeat && IsFinished)
			{
				Play();
			}
			if (!SharedStateProp.TextMeshIsUpdated)
			{
				öç.ForceMeshUpdate();
				SharedStateProp.TextMeshIsUpdated = true;
			}
			öÆ = öç.textInfo;
			TMP_MeshInfo[] array = öÆ.CopyMeshInfoVertexData();
			int characterCount = öÆ.characterCount;
			if (characterCount == 0)
			{
				IsFinished = true;
				return;
			}
			float num = Time.realtimeSinceStartup - öå;
			if (öì != öç.text || ForWords != öû)
			{
				öî = -1f;
				öì = öç.text;
				öû = ForWords;
				ë();
			}
			if (CharactersPerDuration > 0)
			{
				öæ = DurationInSeconds * (float)öì.Length / (float)CharactersPerDuration;
			}
			else
			{
				öæ = DurationInSeconds;
			}
			if (öè && num >= öî)
			{
				öî = num + öæ;
				öÄ++;
				if (öÅ.Length < characterCount * 2)
				{
					öÅ = new float[characterCount * 2];
				}
				for (int i = 0; i < öÅ.Length; i++)
				{
					öÅ[i] = UnityEngine.Random.value;
				}
			}
			if (öë && num > öæ)
			{
				num = öæ;
				IsFinished = true;
			}
			float num2 = num / öæ;
			if (!öë)
			{
				num2 %= 1f;
			}
			float characterDurationRatio = CharacterDurationRatio;
			float num3 = Mathf.Lerp(1f / (float)characterCount, 1f, characterDurationRatio);
			int num4 = 0;
			int num5 = characterCount;
			if (öà.Count > 0 || !string.IsNullOrEmpty(ForWords))
			{
				num5 = 0;
				for (int j = 0; j < öà.Count; j++)
				{
					(int, int) tuple = öà[j];
					num5 += tuple.Item2 - tuple.Item1 + 1;
				}
			}
			for (int k = 0; k < characterCount; k++)
			{
				if (öà.Count > 0 || !string.IsNullOrEmpty(ForWords))
				{
					bool flag = false;
					for (int l = 0; l < öà.Count; l++)
					{
						(int, int) tuple2 = öà[l];
						if (k >= tuple2.Item1 && k <= tuple2.Item2)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						continue;
					}
				}
				TMP_CharacterInfo î = öÆ.characterInfo[k];
				if (î.isVisible)
				{
					float num6 = Mathf.Lerp((float)num4 * 1f / (float)num5, 0f, characterDurationRatio);
					float value = (num2 - num6) / num3;
					value = Mathf.Clamp01(value);
					int materialReferenceIndex = î.materialReferenceIndex;
					int vertexIndex = î.vertexIndex;
					Color32[] colors = öÆ.meshInfo[materialReferenceIndex].colors32;
					Vector3[] vertices = array[materialReferenceIndex].vertices;
					Vector3[] vertices2 = öÆ.meshInfo[materialReferenceIndex].vertices;
					è(öÆ, î, vertexIndex, colors, vertices2, vertices, num2, value, öÄ);
					num4++;
				}
			}
			for (int m = 0; m < öÆ.meshInfo.Length; m++)
			{
				if (m < öÆ.materialCount)
				{
					öÆ.meshInfo[m].mesh.vertices = öÆ.meshInfo[m].vertices;
					öç.UpdateGeometry(öÆ.meshInfo[m].mesh, m);
				}
			}
			öç.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
		}

		private void ë()
		{
			öà.Clear();
			if (string.IsNullOrWhiteSpace(ForWords) || öì == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(öÆ.characterCount);
			for (int i = 0; i < öÆ.characterCount; i++)
			{
				stringBuilder.Append(öÆ.characterInfo[i].character);
			}
			bool flag = (öç.fontStyle & (FontStyles.LowerCase | FontStyles.UpperCase | FontStyles.SmallCaps)) != 0;
			string text = stringBuilder.ToString();
			string[] array = ForWords.Split(new char[2]
			{
				'\t',
				' '
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				int startIndex = 0;
				while (true)
				{
					startIndex = text.IndexOf(text2, startIndex, flag ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
					if (startIndex == -1)
					{
						break;
					}
					if (startIndex <= 0 || char.IsWhiteSpace(text[startIndex - 1]))
					{
						int num = startIndex + text2.Length;
						if (num >= text.Length || char.IsWhiteSpace(text[num]))
						{
							öà.Add((startIndex, startIndex + text2.Length - 1));
						}
					}
					startIndex += text2.Length;
				}
			}
		}

		private void è(TMP_TextInfo ï, TMP_CharacterInfo î, int ì, Color32[] Ä, Vector3[] Å, Vector3[] É, float æ, float Æ, ushort û)
		{
			if (öè)
			{
				ª(î, ì, Ä, ù(öÄ + î.index));
			}
			else
			{
				ª(î, ì, Ä, Æ);
			}
			switch (Type)
			{
			case EffectType.Waves:
				οö(î, ì, Å, É, æ);
				break;
			case EffectType.Grow:
				οç(î, ì, Å, É, Æ);
				break;
			case EffectType.Unfold:
				οì(î, ì, Å, É, Æ);
				break;
			case EffectType.UnfoldAndWaves:
				οì(î, ì, Å, É, Æ);
				οö(î, ì, Å, Å, æ);
				break;
			case EffectType.Sketch:
				Ü(î, ì, Å, É, Æ, û);
				break;
			case EffectType.Bounce:
				οü(î, ì, Å, É, Æ);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private float ù(int ÿ)
		{
			int num = Mathf.Abs(ÿ % öÅ.Length);
			return öÅ[num];
		}

		private void Ü(TMP_CharacterInfo ƒ, int á, Vector3[] í, Vector3[] ú, float ñ, int Ñ)
		{
			öí οÜ = default(öí);
			οÜ.öú = ƒ;
			οÜ.öñ = this;
			í[á] = ú[á] - οû(á, Ñ, ref οÜ);
			í[á + 1] = ú[á + 1] - οû(á + 1, Ñ, ref οÜ);
			í[á + 2] = ú[á + 2] - οû(á + 2, Ñ, ref οÜ);
			í[á + 3] = ú[á + 3] - οû(á + 3, Ñ, ref οÜ);
		}

		private void ª(TMP_CharacterInfo οo, int οο, Color32[] οо, float οô)
		{
			Color b = Gradient.Evaluate(οô);
			if (Mix == MixType.Multiply)
			{
				ref Color32 reference = ref οо[οο];
				reference *= b;
				ref Color32 reference2 = ref οо[οο + 1];
				reference2 *= b;
				ref Color32 reference3 = ref οо[οο + 2];
				reference3 *= b;
				ref Color32 reference4 = ref οо[οο + 3];
				reference4 *= b;
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					Color c = οо[οο + i] + b;
					c.a *= b.a;
					οо[οο + i] = c;
				}
			}
		}

		private void οö(TMP_CharacterInfo οò, int οó, Vector3[] οÖ, Vector3[] οº, float οÇ)
		{
			öÑ οí = default(öÑ);
			οí.öª = οÇ;
			οí.òo = οò;
			οí.òο = this;
			οÖ[οó] = οº[οó] - οƒ(οó, ref οí);
			οÖ[οó + 1] = οº[οó + 1] - οƒ(οó + 1, ref οí);
			οÖ[οó + 2] = οº[οó + 2] - οƒ(οó + 2, ref οí);
			οÖ[οó + 3] = οº[οó + 3] - οƒ(οó + 3, ref οí);
		}

		private void οü(TMP_CharacterInfo οé, int οâ, Vector3[] οä, Vector3[] οà, float οå)
		{
			òо οÑ = default(òо);
			οÑ.òô = οå;
			οÑ.òö = οé;
			οÑ.òò = this;
			οä[οâ] = οà[οâ] - οú(οâ, ref οÑ);
			οä[οâ + 1] = οà[οâ + 1] - οú(οâ + 1, ref οÑ);
			οä[οâ + 2] = οà[οâ + 2] - οú(οâ + 2, ref οÑ);
			οä[οâ + 3] = οà[οâ + 3] - οú(οâ + 3, ref οÑ);
		}

		private void οç(TMP_CharacterInfo οê, int οë, Vector3[] οè, Vector3[] οï, float οî)
		{
			οè[οë] = οï[οë];
			οè[οë + 3] = οï[οë + 3];
			οè[οë + 1] = Vector3.Lerp(οï[οë], οï[οë + 1], οî);
			οè[οë + 2] = Vector3.Lerp(οï[οë + 3], οï[οë + 2], οî);
			οè[οë] = Vector3.LerpUnclamped(οï[οë], οè[οë], Amplitude);
			οè[οë + 1] = Vector3.LerpUnclamped(οï[οë + 1], οè[οë + 1], Amplitude);
			οè[οë + 2] = Vector3.LerpUnclamped(οï[οë + 2], οè[οë + 2], Amplitude);
			οè[οë + 3] = Vector3.LerpUnclamped(οï[οë + 3], οè[οë + 3], Amplitude);
		}

		private void οì(TMP_CharacterInfo οÄ, int οÅ, Vector3[] οÉ, Vector3[] οæ, float οÆ)
		{
			Vector3 a = (οæ[οÅ] + οæ[οÅ + 1]) * 0.5f;
			Vector3 a2 = (οæ[οÅ + 3] + οæ[οÅ + 2]) * 0.5f;
			οÉ[οÅ] = Vector3.Lerp(a, οæ[οÅ], οÆ);
			οÉ[οÅ + 3] = Vector3.Lerp(a2, οæ[οÅ + 3], οÆ);
			οÉ[οÅ + 1] = Vector3.Lerp(a, οæ[οÅ + 1], οÆ);
			οÉ[οÅ + 2] = Vector3.Lerp(a2, οæ[οÅ + 2], οÆ);
			οÉ[οÅ] = Vector3.LerpUnclamped(οæ[οÅ], οÉ[οÅ], Amplitude);
			οÉ[οÅ + 1] = Vector3.LerpUnclamped(οæ[οÅ + 1], οÉ[οÅ + 1], Amplitude);
			οÉ[οÅ + 2] = Vector3.LerpUnclamped(οæ[οÅ + 2], οÉ[οÅ + 2], Amplitude);
			οÉ[οÅ + 3] = Vector3.LerpUnclamped(οæ[οÅ + 3], οÉ[οÅ + 3], Amplitude);
		}

		[ContextMenu("Play")]
		public void Play()
		{
			Apply();
			IsFinished = false;
			öå = Time.realtimeSinceStartup;
			öï = true;
		}

		[ContextMenu("Finish")]
		public void Finish()
		{
			öå = float.MinValue;
		}

		[CompilerGenerated]
		private Vector3 οû(int οù, int οÿ, ref öí οÜ)
		{
			float num = οÜ.öú.pointSize * 0.1f * Amplitude;
			float num2 = ù(οù << οÿ);
			float num3 = ù(οù << οÿ >> 5);
			return new Vector3(num2 * num, num3 * num, 0f);
		}

		[CompilerGenerated]
		private Vector3 οƒ(int οá, ref öÑ οí)
		{
			float f = (float)Math.PI * -2f * οí.öª + (float)(οá / 4) * 0.3f;
			return new Vector3(0f, Mathf.Cos(f) * οí.òo.pointSize * 0.3f * Amplitude, 0f);
		}

		[CompilerGenerated]
		private Vector3 οú(int οñ, ref òо οÑ)
		{
			float f = (float)Math.PI * -2f * οÑ.òô;
			return new Vector3(0f, Mathf.Cos(f) * οÑ.òö.pointSize * 0.3f * Amplitude, 0f);
		}
	}
}
