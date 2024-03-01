using UnityEngine;

namespace Gilzoide.LyonTessellation.Samples.RenderPrimitives
{
    public static class ShaderProperties
    {
        public static readonly int Positions = Shader.PropertyToID("_Positions");
        public static readonly int ObjectToWorld = Shader.PropertyToID("_ObjectToWorld");
    }
}
