Shader "Custom/VisibleBehindWallsSurf"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _ColorOnHide ("Color when hide", Color) = (0,0,0,0)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        [HDR] _EmissionColor("Color", Color) = (0,0,0)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.002, 0.03)) = .005
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200
        
        Pass
        {
            Cull off
            ZWrite off
            ZTest Always
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
            };
	
            float4 _ColorOnHide;
			uniform float _Outline;
			uniform float4 _OutlineColor;
			
			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float3 norm   = normalize(mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				float2 offset = TransformViewToProjection(norm.xy);

				// o.normal = norm;

				#ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE //to handle recent standard asset package on older version of unity (before 5.5)
					o.vertex.xy += offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(o.vertex.z) * _Outline;
				#else
					o.vertex.xy += offset * o.vertex.z * _Outline;
				#endif
				o.color = _OutlineColor;
				if (v.normal.x == 0)
					o.color = float4(0,0,0,0);
				
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            
            ENDCG
        }    

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows


        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed4 _EmissionColor;
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Emission = c.rgb * tex2D(_MainTex, IN.uv_MainTex).a * _EmissionColor;
        }
        ENDCG

    }
    FallBack "Diffuse"
}
