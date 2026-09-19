// ================================================================================
// 【Codex 中文研读注释｜学习副本，不是原作者注释】
// 原始路径：Assets\NOT_Lonely\Weatherade SRS\Shaders\Blit\TexturePacking.shader
// 分类/优先级：积雪遮挡算法 / A-必读
// 文件职责：把原始深度转换为指数深度的一、二阶矩，并提供通道打包 Pass。
// 数据流位置：sceneDepthTex -> RG moments -> _SRS_depth。
// 主要 Unity 技术：EVSM/VSM 矩、全屏 Blit、reversed-Z
// 建议关注：重点是 Pass 0 的 exp、E[x]、E[x²]。
// 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
// ================================================================================

Shader "Hidden/NOT_Lonely/Weatherade/NL_TexturePacking"
{
    Properties
    {
        _DepthTex("DepthTex", 2D) = "white" {}
        _rgb("rgb", 2D) = "red" {}
		_a("a", 2D) = "black" {}
        _rg("rg", 2D) = "red" {}
		_b("b", 2D) = "black" {}
        //_VsmExp("VsmExp", float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            // 【中文注释】Pass 0 输出指数深度的一、二阶矩，可模糊后做概率遮挡测试。
            Name "Depth to VSM"
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            UNITY_DECLARE_TEX2D(_DepthTex);
            uniform float _VsmExp;

            float2 frag (v2f i) : SV_Target
            {
                //_VsmExp = 25;
                float2 result;
                float depth = UNITY_SAMPLE_TEX2D(_DepthTex, i.uv).r;
                
                #if !defined(UNITY_REVERSED_Z)
                    depth = 1-depth;
                #endif

                depth = depth * -1;
                
                depth = 2.0 * depth - 1.0;
                depth = exp(_VsmExp * depth);
                
                result.x = depth;
                result.y = depth * depth;
                return result;
            }
            ENDCG
        }

        Pass
        {
            // 【中文注释】把 RG 矩与原始 Depth 合到 RGB；ARGB 模式调用该 Pass。
            Name "Pack to RGB"
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            UNITY_DECLARE_TEX2D(_rg);
            UNITY_DECLARE_TEX2D(_b);

            float3 frag (v2f i) : SV_Target
            {
                float2 rg = UNITY_SAMPLE_TEX2D(_rg, i.uv).rg;
                float b = UNITY_SAMPLE_TEX2D(_b, i.uv);
                
                return float3(rg, b);
            }
            ENDCG
        }

        Pass
        {
            Name "Pack to RGBA"
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            UNITY_DECLARE_TEX2D(_rgb);
            UNITY_DECLARE_TEX2D(_a);

            float4 frag (v2f i) : SV_Target
            {
                float3 rgb = UNITY_SAMPLE_TEX2D(_rgb, i.uv).rgb;
                float a = UNITY_SAMPLE_TEX2D(_a, i.uv);
                
                return float4(rgb, a);
            }
            ENDCG
        }
    }
}
