Shader "Custom/DepthNormalOutlineWithCartoonColor"
{
    Properties
    {
        [Header(Outline Settings)]
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0.1, 10)) = 1
        
        [Header(Edge Sensitivity)]
        _DepthSensitivity ("Depth Sensitivity", Range(0.1, 10)) = 1
        _NormalSensitivity ("Normal Sensitivity", Range(0.1, 10)) = 1
        _NormalThreshold ("Normal Threshold", Range(0, 1)) = 0.5
        
        [Header(Cartoon Color Enhancement)]
        _Saturation ("Saturation Boost", Range(0.5, 3)) = 1.5
        _QuantizeLevels ("Color Quantize Levels", Range(1, 20)) = 6
        _Contrast ("Contrast Boost", Range(0.5, 2)) = 1.2
        _Vibrance ("Vibrance Boost", Range(0, 2)) = 0.8
        _HighlightBoost ("Highlight Boost", Range(0, 1)) = 0.2
    }
    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _CameraDepthNormalsTexture;
            float4 _MainTex_TexelSize;
            
            // 描边参数
            float4 _OutlineColor;
            float _OutlineThickness;
            float _DepthSensitivity;
            float _NormalSensitivity;
            float _NormalThreshold;
            
            // 颜色增强参数
            float _Saturation;
            float _QuantizeLevels;
            float _Contrast;
            float _Vibrance;
            float _HighlightBoost;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
                    o.uv.y = 1 - o.uv.y;
                #endif
                
                return o;
            }
            
            void CustomDecodeDepthNormal(float4 enc, out float depth, out float3 normal)
            {
                depth = DecodeFloatRG(enc.zw);
                float2 encodedNormal = enc.xy;
                float3 decodedNormal;
                decodedNormal.xy = encodedNormal * 2.0 - 1.0;
                decodedNormal.z = sqrt(1.0 - saturate(dot(decodedNormal.xy, decodedNormal.xy)));
                normal = normalize(decodedNormal);
            }
            
            // ================ 新增颜色增强函数 ================
            // HSV转换函数
            float3 rgb2hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }
            
            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }
            
            // 亮度计算
            float luminance(float3 color)
            {
                return dot(color, float3(0.2126, 0.7152, 0.0722));
            }
            
            // 色彩活力增强
            float3 applyVibrance(float3 color)
            {
                float maxColor = max(color.r, max(color.g, color.b));
                float minColor = min(color.r, min(color.g, color.b));
                float colorSaturation = maxColor - minColor;
                
                float3 lum = luminance(color);
                float3 desaturated = lerp(color, lum, 1.0 - _Vibrance);
                
                return lerp(desaturated, color, colorSaturation * 1.5);
            }
            // ================ 结束新增函数 ================
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 基础纹理采样
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 计算采样偏移
                float2 offsets[4] = {
                    float2(0, _OutlineThickness * _MainTex_TexelSize.y),
                    float2(0, -_OutlineThickness * _MainTex_TexelSize.y),
                    float2(-_OutlineThickness * _MainTex_TexelSize.x, 0),
                    float2(_OutlineThickness * _MainTex_TexelSize.x, 0)
                };
                
                // 中心点深度法线
                float centerDepth;
                float3 centerNormal;
                CustomDecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), centerDepth, centerNormal);
                
                float depthDiff = 0;
                float normalDiff = 0;
                
                // 采样四个方向
                for (int k = 0; k < 4; k++)
                {
                    float2 sampleUV = i.uv + offsets[k];
                    float sampleDepth;
                    float3 sampleNormal;
                    CustomDecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, sampleUV), sampleDepth, sampleNormal);
                    
                    depthDiff += abs(centerDepth - sampleDepth);
                    normalDiff += abs(dot(centerNormal, sampleNormal));
                }
                
                // 标准化并应用敏感度
                depthDiff = saturate(depthDiff * _DepthSensitivity * 0.25);
                normalDiff = 1.0 - saturate(normalDiff * _NormalSensitivity * 0.25);
                
                // 边缘检测
                float edge = saturate(depthDiff + step(_NormalThreshold, normalDiff));
                
                // 混合描边颜色
                col = lerp(col, _OutlineColor, edge);
                
                // ==================== 新增颜色增强部分 ====================
                // 1. 增强色彩活力 (智能饱和度)
                col.rgb = applyVibrance(col.rgb);
                
                // 2. 提高饱和度
                float3 hsvCol = rgb2hsv(col.rgb);
                hsvCol.y *= _Saturation; // 增强饱和度
                col.rgb = hsv2rgb(hsvCol);
                
                // 3. 颜色量化 - 创建卡通色块效果
                col.rgb = floor(col.rgb * _QuantizeLevels) / _QuantizeLevels;
                
                // 4. 增强对比度
                col.rgb = (col.rgb - 0.5) * _Contrast + 0.5;
                
                // 5. 卡通风格高光增强
                float brightness = max(col.r, max(col.g, col.b));
                float highlight = smoothstep(0.7, 0.9, brightness);
                col.rgb += highlight * _HighlightBoost * float3(1.0, 0.9, 0.8);
                
                // 6. 轻微泛光效果
                float3 brightAreas = saturate(col.rgb - 0.7);
                col.rgb += brightAreas * float3(0.8, 0.6, 0.9) * 0.1;
                
                // ==================== 结束颜色增强部分 ====================
                
                return col;
            }
            ENDCG
        }
    }
}