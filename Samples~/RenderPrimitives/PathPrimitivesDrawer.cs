using System;
using Gilzoide.LyonTesselation;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Gilzoide.LyonTessellation.Samples.RenderPrimitives
{
    using Vertex = Vector4;
    using Index = UInt16;

    public class PathPrimitivesDrawer : MonoBehaviour
    {
        public FillOptions FillOptions = FillOptions.Default();
        public StrokeOptions StrokeOptions = StrokeOptions.Default();
        public bool Fill = true;
        public Material Material;

        private PathBuilder _pathBuilder;
        private Tessellator<Vertex, Index> _tessellator;
        private JobHandle _jobHandle;
        private GraphicsBuffer _vertexBuffer;
        private GraphicsBuffer _indexBuffer;
        private MaterialPropertyBlock _materialProperties;

        protected void OnEnable()
        {
            _pathBuilder = new();
            _tessellator = new();
        }

        protected void OnDisable()
        {
            _jobHandle.Complete();
            _tessellator?.Dispose();
            _tessellator = null;
            _pathBuilder?.Dispose();
            _pathBuilder = null;
            _indexBuffer?.Dispose();
            _indexBuffer = null;
            _vertexBuffer?.Dispose();
            _vertexBuffer = null;
        }

        protected void Update()
        {
            Vector2 center = Vector2.zero;
            _pathBuilder.Clear()
                .AddCircle(center, 1)
                .AddCircle(new Vector2(center.x - 0.2f, center.y + 0.2f), 0.2f)
                .AddCircle(new Vector2(center.x + 0.2f, center.y + 0.2f), 0.2f)
                .MoveTo(new Vector2(center.x - 0.3f, center.y - 0.05f))
                    .CubicBezierTo(
                        new Vector2(center.x - 0.3f, center.y - 0.45f),
                        new Vector2(center.x + 0.3f, center.y - 0.45f),
                        new Vector2(center.x + 0.3f, center.y - 0.05f)
                    )
                .Close();

            _tessellator.Clear();
            if (Fill)
            {
                _jobHandle = _tessellator.CreatePathFillJob(_pathBuilder, FillOptions).Schedule();
            }
            else
            {
                _jobHandle = _tessellator.CreatePathStrokeJob(_pathBuilder, StrokeOptions).Schedule();
            }
        }

        protected void LateUpdate()
        {
            _jobHandle.Complete();

            if (_tessellator.VerticesLength == 0 || _tessellator.IndicesLength == 0)
            {
                return;
            }

            if (_materialProperties == null)
            {
                _materialProperties = new MaterialPropertyBlock();
            }
            if (_vertexBuffer == null || _vertexBuffer.count < _tessellator.VerticesLength)
            {
                _vertexBuffer?.Dispose();
                _vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, GraphicsBuffer.UsageFlags.LockBufferForWrite, _tessellator.VerticesLength, UnsafeUtility.SizeOf<Vertex>());
                _materialProperties.SetBuffer(ShaderProperties.Positions, _vertexBuffer);
            }
            if (_indexBuffer == null || _indexBuffer.count < _tessellator.IndicesLength)
            {
                _indexBuffer?.Dispose();
                _indexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Index, GraphicsBuffer.UsageFlags.LockBufferForWrite, _tessellator.IndicesLength, UnsafeUtility.SizeOf<Index>());
            }

            var vertexData = _vertexBuffer.LockBufferForWrite<Vertex>(0, _tessellator.VerticesLength);
            _tessellator.Vertices.CopyTo(vertexData);
            _vertexBuffer.UnlockBufferAfterWrite<Vertex>(_tessellator.VerticesLength);

            var indexData = _indexBuffer.LockBufferForWrite<Index>(0, _tessellator.IndicesLength);
            _tessellator.Indices.CopyTo(indexData);
            _indexBuffer.UnlockBufferAfterWrite<Index>(_tessellator.IndicesLength);

            _materialProperties.SetMatrix(ShaderProperties.ObjectToWorld, transform.localToWorldMatrix);

            RenderParams renderParams = new(Material)
            {
                matProps = _materialProperties,
                worldBounds = new Bounds(transform.position, new Vector3(2, 2, 0))
            };
            Graphics.RenderPrimitivesIndexed(renderParams, MeshTopology.Triangles, _indexBuffer, _tessellator.IndicesLength);
        }
    }
}
