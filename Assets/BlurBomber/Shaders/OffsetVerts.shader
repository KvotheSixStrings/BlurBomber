Shader "Shreveport Arcade/Offset Verts" {
  Properties {
    [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    _Offset ("Vertex Offset (X, Y, Angle, Size)", Vector) = (0,0,0,1)
  }

  SubShader {
    Tags { 
      "Queue"="Transparent" 
      "IgnoreProjector"="True" 
      "RenderType"="Transparent" 
      "PreviewType"="Plane"
      "CanUseSpriteAtlas"="True"
    }

    Cull Off
    Lighting Off
    ZWrite Off
    Fog { Mode Off }
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      
      struct appdata_t {
        float4 vertex   : POSITION;
        float4 color    : COLOR;
        float2 texcoord : TEXCOORD0;
      };

      struct v2f {
        float4 vertex   : SV_POSITION;
        fixed4 color    : COLOR;
        half2 texcoord  : TEXCOORD0;
      };
      
      float4 _Offset;

      v2f vert(appdata_t IN) {
        v2f OUT;
        OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);

        float ang = radians(_Offset.z);
        float2 pos = OUT.vertex.xy * _Offset.w;
        pos = float2(pos.x*cos(ang) - pos.y*sin(ang), pos.x*sin(ang) + pos.y*cos(ang));
        pos += _Offset.xy;
        OUT.vertex.xy = pos;

        OUT.texcoord = IN.texcoord;
        OUT.color = IN.color;
        return OUT;
      }

      sampler2D _MainTex;

      fixed4 frag(v2f IN) : SV_Target {
        fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
        return c;
      }
      ENDCG
    }
  }
}
