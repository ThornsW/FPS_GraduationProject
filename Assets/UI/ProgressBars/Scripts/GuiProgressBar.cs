using System.Globalization;
using UnityEngine;
using System;

namespace Assets.ProgressBars.Scripts
{
	public enum TextIndicationType
	{
		Value,
		Sectors
	}

    public class GuiProgressBar : MonoBehaviour
    {
		private const float Epsilon = 0.003f;
        private const string OffsetProperty = "_Offset";
        private const string AnimOffsetProperty = "_AnimOffset";
		private const string MainTextureProperty = "_MainTex";
		private const string MaskTextureProperty = "_MaskTex";

		private Color[] _maskPixels;

		private float _animOffset;

		[SerializeField] 
		private float _value;
		public float Value {
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				ValueUpdated();
			}
		}

		[SerializeField] 
		private Sprite _mainSprite;
		public Sprite MainSprite {
			get
			{
				return _mainSprite;
			}
			set
			{
				if(value == null) return;
				_mainSprite = value;
				MainSpriteUpdated();
			}
		}

		[SerializeField] 
		private Sprite _maskSprite;
		public Sprite MaskSprite {
			get
			{
				return _maskSprite;
			}
			set
			{
				if(value == null) return;
				_maskSprite = value;
				MaskSpriteUpdated();
			}
		}

		[SerializeField] 
		private TextureWrapMode _textureWrapMode;
		public TextureWrapMode TextureWrapMode {
			get
			{
				return _textureWrapMode;
			}
			set
			{
				_textureWrapMode = value;
				WrapModeChanged();
			}
		}

		[SerializeField] 
		private Transform _knob;
		public Transform Knob {
			get
			{
				return _knob;
			}
			set
			{
				_knob = value;
			}
		}

		[SerializeField] 
		private TextMesh _textMesh;
		public TextMesh TextMesh {
			get
			{
				return _textMesh;
			}
			set
			{
				_textMesh = value;
			}
		}

		[SerializeField] 
		private int _digitsAfterComma;
		public int DigitsAfterComma {
			get
			{
				return _digitsAfterComma;
			}
			set
			{
				_digitsAfterComma = value;
				ValueUpdated();
			}
		}

		[SerializeField] 
		private string _textSuffix;
		public string TextSuffix {
			get
			{
				return _textSuffix;
			}
			set
			{
				_textSuffix = value;
				ValueUpdated();
			}
		}

		[SerializeField] 
		private float _knobPositionOffset;
		public float KnobPositionOffset {
			get
			{
				return _knobPositionOffset;
			}
			set
			{
				_knobPositionOffset = value;
			}
		}

		[SerializeField] 
		private float _knobMinPercent;
		public float KnobMinPercent {
			get
			{
				return _knobMinPercent;
			}
			set
			{
				_knobMinPercent = value;
			}
		}

		[SerializeField] 
		private float _knobMaxPercent;
		public float KnobMaxPercent {
			get
			{
				return _knobMaxPercent;
			}
			set
			{
				_knobMaxPercent = value;
			}
		}

		[SerializeField] 
		private TextIndicationType _textIndication;
		public TextIndicationType TextIndication {
			get
			{
				return _textIndication;
			}
			set
			{
				_textIndication = value;
			}
		}

		[SerializeField] 
		private int _sectorsCount;
		public int SectorsCount {
			get
			{
				return _sectorsCount;
			}
			set
			{
				_sectorsCount = value;
			}
		}

        private void Start()
		{
			GetComponent<Renderer> ().sharedMaterial = Instantiate (GetComponent<Renderer> ().sharedMaterial);
			var rendererSprite = GetComponent<SpriteRenderer>().sprite;
			if (_mainSprite == null) {
				if(rendererSprite != null) 
				{
					_mainSprite = rendererSprite;
				} else return;
			}
			if (rendererSprite == null) {
				if(_mainSprite != null)
				{
					GetComponent<SpriteRenderer>().sprite = _mainSprite;
				}
			}
			if (_maskSprite == null) return;
			_mainSprite.texture.wrapMode = TextureWrapMode;
			_maskSprite.texture.wrapMode = TextureWrapMode;
			try{
				_maskPixels = _maskSprite.texture.GetPixels();
			} catch(Exception)
			{
				_maskPixels = new Color[0];
			}

			GetComponent<Renderer>().sharedMaterial.SetTexture(MainTextureProperty, _mainSprite.texture);
			GetComponent<Renderer>().sharedMaterial.SetTexture(MaskTextureProperty, _maskSprite.texture);
        }

		/// <summary>
		/// update main sprite texture
		/// </summary>
		private void MainSpriteUpdated()
		{
			GetComponent<SpriteRenderer>().sprite = _mainSprite;
			GetComponent<Renderer>().sharedMaterial.SetTexture(MainTextureProperty, _mainSprite.texture);
			_mainSprite.texture.wrapMode = TextureWrapMode;
		}

		/// <summary>
		/// update mask sprite texture
		/// </summary>
		private void MaskSpriteUpdated()
		{
			GetComponent<Renderer>().sharedMaterial.SetTexture(MaskTextureProperty, _maskSprite.texture);
			_maskSprite.texture.wrapMode = TextureWrapMode;
			try{
				_maskPixels = _maskSprite.texture.GetPixels();
			} catch(Exception)
			{
				_maskPixels = new Color[0];
			}
		}

		/// <summary>
		/// set new knob position if exist
		/// </summary>
		private void SetKnobPosition ()
		{
			if (_maskPixels != null && _maskPixels.Length > 0)
				SetKnobPositionByMaskData ();
			else 
				SetKnobPositionByBarWidth ();
		}

		/// <summary>
		/// Sets the position of knob using current value offset
		/// depends on bar width
		/// </summary>
		private void SetKnobPositionByBarWidth ()
		{
			if (_mainSprite == null)
				return;
			var barWidth = _mainSprite.bounds.size.x;
			if(_knob != null)
			{
				var value = _value;
				if(value > _knobMaxPercent) value = _knobMaxPercent;
				if(value < _knobMinPercent) value = _knobMinPercent;
				_knob.localPosition = new Vector3(barWidth * value - barWidth*0.5f + _knobPositionOffset, _knob.localPosition.y, 0);
			}
		}

		/// <summary>
		/// sets the positon of knob using value offset
		/// depends on mask pixels data
		/// </summary>
		private void SetKnobPositionByMaskData()
		{
			if (_maskSprite == null)
				return;
			var texture = _maskSprite.texture;
			
			int x = 0;
			int y = 0;
			int width = texture.width;
			int cnt = 0;
			
			int i = _maskPixels.Length;
			while (--i > -1) {
				if(_maskPixels[i].a < .9f) continue;
				
				if(_maskPixels[i].r > _value - Epsilon &&
				   _maskPixels[i].r < _value + Epsilon)
				{
					x += i % width;
					y += i / width;
					cnt++;
				}
			}
			
			if (cnt != 0) {
				x = x / cnt;
				y = y / cnt;
				if(_knob != null)
				{
					_knob.localPosition = new Vector3(x / 100f - width * 0.005f, y / 100f - width * 0.005f, 0);
				}
			}
		}

		/// <summary>
		/// Wraps the mode changed.
		/// </summary>
		private void WrapModeChanged()
		{
			if(_mainSprite != null) _mainSprite.texture.wrapMode = TextureWrapMode;
			if(_maskSprite != null) _maskSprite.texture.wrapMode = TextureWrapMode;
		}

		/// <summary>
		/// set current percent value and update knob position
		/// </summary>
		private void ValueUpdated()
		{
			SetPercent(_value);
			SetKnobPosition();
		}

        private void Update()
        {
			_animOffset += 0.001f;
			ValueUpdated();
		}
		
		/// <summary>
        /// update shader vars with current percent value
        /// </summary>
        /// <param name="value"></param>
        public void SetPercent(float value)
        {
			GetComponent<Renderer>().sharedMaterial.SetFloat(OffsetProperty, Value);
			GetComponent<Renderer>().sharedMaterial.SetFloat(AnimOffsetProperty, _animOffset); 
			if (_textMesh != null) {
				if(_textIndication == TextIndicationType.Value)
				{
					_textMesh.text = FormatNumber(value * 100);
				}
				else 
				{
					var parts = _sectorsCount;
					_textMesh.text = ((int)(value / (1f / parts))) + "/" + parts;
				}
			}
        }

		/// <summary>
		/// Formats the number to displayed string
		/// </summary>
		/// <returns>The number.</returns>
		/// <param name="number">Number.</param>
		private string FormatNumber(float number)
		{
			var multiplier = _digitsAfterComma * 10;
			if (multiplier == 0)
				return ((int)(number)) + _textSuffix;
			string result = ((int)(number * multiplier) / (float)multiplier).ToString(CultureInfo.InvariantCulture);
			if (result.IndexOf(".", StringComparison.Ordinal) == -1) {
				int i = _digitsAfterComma;
				string appendix = string.Empty;
				while(--i > -1)
				{
					appendix += "0" + _textSuffix;
				}
				result = string.Format("{0}.{1}", result, appendix);
			}
			return result;
		}
    }
}
