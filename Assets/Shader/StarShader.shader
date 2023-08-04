Shader "Custom/StarShader"
{
    Properties
    {
        _MainTex("iChannel0", 2D) = "white" {}
        _SecondTex("iChannel1", 2D) = "white" {}
        _ThirdTex("iChannel2", 2D) = "white" {}
        _FourthTex("iChannel3", 2D) = "white" {}
        _Mouse("Mouse", Vector) = (0.5, 0.5, 0.5, 0.5)
        [ToggleUI] _GammaCorrect("Gamma Correction", Float) = 1
        _Resolution("Resolution (Change if AA is bad)", Range(1, 1024)) = 1
    }
        SubShader
        {
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

                // Built-in properties
                sampler2D _MainTex;   float4 _MainTex_TexelSize;
                sampler2D _SecondTex; float4 _SecondTex_TexelSize;
                sampler2D _ThirdTex;  float4 _ThirdTex_TexelSize;
                sampler2D _FourthTex; float4 _FourthTex_TexelSize;
                float4 _Mouse;
                float _GammaCorrect;
                float _Resolution;

                // GLSL Compatability macros
                #define glsl_mod(x,y) (((x)-(y)*floor((x)/(y))))
                #define texelFetch(ch, uv, lod) tex2Dlod(ch, float4((uv).xy * ch##_TexelSize.xy + ch##_TexelSize.xy * 0.5, 0, lod))
                #define textureLod(ch, uv, lod) tex2Dlod(ch, float4(uv, 0, lod))
                #define iResolution float3(_Resolution, _Resolution, _Resolution)
                #define iFrame (floor(_Time.y / 60))
                #define iChannelTime float4(_Time.y, _Time.y, _Time.y, _Time.y)
                #define iDate float4(2020, 6, 18, 30)
                #define iSampleRate (44100)
                #define iChannelResolution float4x4(                      \
                    _MainTex_TexelSize.z,   _MainTex_TexelSize.w,   0, 0, \
                    _SecondTex_TexelSize.z, _SecondTex_TexelSize.w, 0, 0, \
                    _ThirdTex_TexelSize.z,  _ThirdTex_TexelSize.w,  0, 0, \
                    _FourthTex_TexelSize.z, _FourthTex_TexelSize.w, 0, 0)

                // Global access to uv data
                static v2f vertex_output;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                float2 rotate(float2 uv, float th)
                {
                    return mul(transpose(float2x2(cos(th), sin(th), -sin(th), cos(th))),uv);
                }

                float sdStar5(float2 p, float r, float rf)
                {
                    const float2 k1 = float2(0.809017, -0.58778524);
                    const float2 k2 = float2(-k1.x, k1.y);
                    p.x = abs(p.x);
                    p -= 2. * max(dot(k1, p), 0.) * k1;
                    p -= 2. * max(dot(k2, p), 0.) * k2;
                    p.x = abs(p.x);
                    p.y -= r;
                    float2 ba = rf * float2(-k1.y, k1.x) - float2(0, 1);
                    float h = clamp(dot(p, ba) / dot(ba, ba), 0., r);
                    return length(p - ba * h) * sign(p.y * ba.x - p.x * ba.y);
                }

                float4 frag(v2f __vertex_output) : SV_Target
                {
                    vertex_output = __vertex_output;
                    float4 fragColor = 0;
                    float2 fragCoord = vertex_output.uv * _Resolution;
                    float2 uv = fragCoord / iResolution.xy;
                    uv -= 0.5;
                    uv.x *= iResolution.x / iResolution.y;
                    float d = sdStar5(rotate(uv, _Time.y), 0.12, 0.45);
                    float3 col = ((float3)step(0., -d));
                    col += clamp(((float3)0.001 / d), 0., 1.) * 12.;
                    col *= float3(1, 1, 0);
                    fragColor = float4(col, 1.);
                    if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                    return fragColor;
                }
                ENDCG
            }
        }
}

