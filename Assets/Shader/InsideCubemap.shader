Shader "Unlit/InsideCubemap" {
    Properties {
        _Cubemap("Cubemap", CUBE) = "white" {}
    }
    SubShader {
        Tags { "Queue"="Background" "RenderType"="Background" }
        Cull Front // �ؼ�����ת��Ⱦ��

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float3 worldPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            samplerCUBE _Cubemap;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // ��ȡ��������λ��
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // ��������������ķ�������
                float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
                return texCUBE(_Cubemap, viewDir);
            }
            ENDCG
        }
    }
}
