Shader "LyonTessellation/PathPrimitives"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float4 _Color;
            StructuredBuffer<float4> _Positions;
            uniform float4x4 _ObjectToWorld;

            v2f vert(uint vertexID: SV_VertexID, uint instanceID : SV_InstanceID)
            {
                v2f o;
                float4 pos = float4(_Positions[vertexID].xy, 0, 1);
                float4 wpos = mul(_ObjectToWorld, pos);
                o.pos = mul(UNITY_MATRIX_VP, wpos);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
