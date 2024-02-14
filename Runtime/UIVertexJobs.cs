using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

namespace Gilzoide.LyonTesselation
{
    public static class UIVertexJobs
    {
        public static TessellationUIVertexJob<TIndex> CreateUIVertexJob<TIndex>(this Tessellator<UIVertex, TIndex> tessellator, Graphic graphic, Vector4? uv = null)
            where TIndex : unmanaged
        {
            return tessellator.NativeHandle.CreateUIVertexJob(graphic.color, graphic.rectTransform.rect, uv);
        }

        public static TessellationUIVertexJob<TIndex> CreateUIVertexJob<TIndex>(this NativeTessellator<UIVertex, TIndex> tessellator, Graphic graphic, Vector4? uv = null)
            where TIndex : unmanaged
        {
            return tessellator.CreateUIVertexJob(graphic.color, graphic.rectTransform.rect, uv);
        }

        public static TessellationUIVertexJob<TIndex> CreateUIVertexJob<TIndex>(this Tessellator<UIVertex, TIndex> tessellator, Color32 color, Rect rect, Vector4? uv = null)
            where TIndex : unmanaged
        {
            return tessellator.NativeHandle.CreateUIVertexJob(color,rect, uv);
        }

        public static TessellationUIVertexJob<TIndex> CreateUIVertexJob<TIndex>(this NativeTessellator<UIVertex, TIndex> tessellator, Color32 color, Rect rect, Vector4? uv = null)
            where TIndex : unmanaged
        {
            return new TessellationUIVertexJob<TIndex>(tessellator, color, rect, uv);
        }
    }


    [BurstCompile]
    public struct TessellationUIVertexJob<TIndex> : IJob
        where TIndex : unmanaged
    {
        public TessellationUIVertexJob(NativeTessellator<UIVertex, TIndex> tessellator, Color32 color, Rect rect, Vector4? uv)
        {
            _tessellator = tessellator;
            _color = color;
            _rect = rect;
            _uv = uv;
        }

        private NativeTessellator<UIVertex, TIndex> _tessellator;
        private Color32 _color;
        private Rect _rect;
        private Vector4? _uv;

        public readonly unsafe void Execute()
        {
            using (NativeArray<UIVertex> vertices = _tessellator.Vertices)
            {
                UIVertex* verticesPtr = (UIVertex*) vertices.GetUnsafePtr();
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 position = verticesPtr[i].position;
                    Vector2 normalizedPosition = Rect.PointToNormalized(_rect, position);
                    Vector4 uv = _uv is Vector4 uvRemap ? new Vector2(
                        Mathf.Lerp(uvRemap.x, uvRemap.z, normalizedPosition.x),
                        Mathf.Lerp(uvRemap.y, uvRemap.w, normalizedPosition.y)
                    ) : normalizedPosition;
                    verticesPtr[i] = new UIVertex
                    {
                        position = position,
                        normal = Vector3.back,
                        tangent = new Vector4(1, 0, 0, -1),
                        color = _color,
                        uv0 = uv,
                        uv1 = uv,
                        uv2 = uv,
                        uv3 = uv,
                    };
                }
            }
        }
    }
}
