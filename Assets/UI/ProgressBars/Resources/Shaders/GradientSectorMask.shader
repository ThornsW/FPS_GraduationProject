// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ProgressBars/GradientSectorMask" 
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
        _Offset ("Offset", Range (0,1)) = .5
    }

    SubShader
    {
        Tags
        { 
            "PreviewType"="Plane"
            "Queue"="Transparent"
        }
      
        Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

        Pass
        {		
        	CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            }; 

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };

			float _Offset;
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float _SmoothCoeff;
 
            fixed4 frag(v2f IN) : COLOR
            {
            	half2 tc = IN.texcoord;
                half4 texcol = tex2D (_MainTex, tc);
				half4 maskPixel = tex2D (_MaskTex, tc);
				
				float diff = maskPixel.r - 1 + _Offset;
				float alphaMultiplier = diff / 0.1666;
				if(alphaMultiplier > 1) alphaMultiplier = 1;
				texcol.a = alphaMultiplier * texcol.a;
				
               		
                return texcol;
            }
        	ENDCG
        }
    }
    Fallback "Sprites/Default"
}
