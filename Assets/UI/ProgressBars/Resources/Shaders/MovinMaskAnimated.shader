// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ProgressBars/MovingMaskAnimated" 
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
        _Offset ("Offset", Range (0,1)) = .5
        _AnimOffset ("AnimOffset", Range (0,1)) = .5
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
            //#include "UnityCG.cginc"
			
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
            
			float _AnimOffset;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
				OUT.texcoord.x += _AnimOffset;
                OUT.color = IN.color;
                return OUT;
            }

			float _Offset;
            sampler2D _MainTex;
            sampler2D _MaskTex;
 
            fixed4 frag(v2f IN) : COLOR
            {
            	half2 tc = IN.texcoord;
                half4 texcol = tex2D (_MainTex, tc);

				tc.x -= _AnimOffset;
				tc.x -= _Offset;
				half4 maskPixel = tex2D (_MaskTex, tc);
               	texcol.a = maskPixel.a * texcol.a;

				tc.x += _Offset;
				maskPixel = tex2D (_MaskTex, tc);
				texcol.a = maskPixel.a * texcol.a;
				tc.x -= _Offset;

               	if(tc.x > 0) texcol.a = 0;	
                return texcol;
            }
        	ENDCG
        }
    }
    Fallback "Sprites/Default"
}
