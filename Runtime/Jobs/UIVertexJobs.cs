using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

namespace Gilzoide.LyonTesselation
{
    public static class UIVertexJobs
    {
        public static TessellationUIVertexJob<TIndex> CreateUIVertexJob<TIndex>(this GeometryBuilder<UIVertex, TIndex> tessellator, Graphic graphic, Vector4? uv = null)
            where TIndex : unmanaged
        {
            return tessellator.NativeHandle.CreateUIVertexJob(graphic.color, graphic.rectTransform.rect, uv);
        }

        public static TessellationUIVertexJob<TIndex> CreateUIVertexJob<TIndex>(this NativeGeometryBuilder<UIVertex, TIndex> tessellator, Graphic graphic, Vector4? uv = null)
            where TIndex : unmanaged
        {
            return tessellator.CreateUIVertexJob(graphic.color, graphic.rectTransform.rect, uv);
        }

        public static TessellationUIVertexJob<TIndex> CreateUIVertexJob<TIndex>(this GeometryBuilder<UIVertex, TIndex> tessellator, Color32 color, Rect rect, Vector4? uv = null)
            where TIndex : unmanaged
        {
            return tessellator.NativeHandle.CreateUIVertexJob(color,rect, uv);
        }

        public static TessellationUIVertexJob<TIndex> CreateUIVertexJob<TIndex>(this NativeGeometryBuilder<UIVertex, TIndex> tessellator, Color32 color, Rect rect, Vector4? uv = null)
            where TIndex : unmanaged
        {
            return new TessellationUIVertexJob<TIndex>(tessellator, color, rect, uv);
        }
    }


    [BurstCompile]
    public struct TessellationUIVertexJob<TIndex> : IJob
        where TIndex : unmanaged
    {
        public TessellationUIVertexJob(NativeGeometryBuilder<UIVertex, TIndex> geometryBuilder, Color32 color, Rect rect, Vector4? uv)
        {
            _geometry = geometryBuilder;
            _color = color;
            _rect = rect;
            _uv = uv;
        }

        private NativeGeometryBuilder<UIVertex, TIndex> _geometry;
        private Color32 _color;
        private Rect _rect;
        private Vector4? _uv;

        public readonly unsafe void Execute()
        {
            for (int i = 0, count = _geometry.Vertices.Length; i < count; i++)
            {
                ref UIVertex vertex = ref _geometry.Vertices.ElementAt(i);
                Vector3 position = vertex.position;
                Vector2 normalizedPosition = Rect.PointToNormalized(_rect, position);
                Vector4 uv = _uv is Vector4 uvRemap ? new Vector2(
                    Mathf.Lerp(uvRemap.x, uvRemap.z, normalizedPosition.x),
                    Mathf.Lerp(uvRemap.y, uvRemap.w, normalizedPosition.y)
                ) : normalizedPosition;
                vertex = new UIVertex
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
