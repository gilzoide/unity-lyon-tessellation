using System;
using Gilzoide.LyonTesselation.Internal;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Gilzoide.LyonTesselation
{
    public unsafe struct NativeTessellator<TVertex, TIndex> : ITessellator<TVertex, TIndex>, IDisposable, INativeDisposable
        where TVertex : unmanaged
        where TIndex : unmanaged
    {
        internal readonly NativeList<byte> VertexBuffer => _vertexBuffer;
        internal readonly NativeList<byte> IndexBuffer => _indexBuffer;
        private NativeList<byte> _vertexBuffer;
        private NativeList<byte> _indexBuffer;

        public NativeTessellator(Allocator allocator)
        {
            unsafe
            {
                if (sizeof(TVertex) < 2 * sizeof(float))
                {
                    throw new ArgumentException("Vertex size must fit at least 2 floats.", nameof(TVertex));
                }
                if (sizeof(TIndex) < sizeof(ushort))
                {
                    throw new ArgumentException("Index size must fit at least one ushort.", nameof(TIndex));
                }
            }
            _vertexBuffer = new NativeList<byte>(allocator);
            _indexBuffer = new NativeList<byte>(allocator);
        }

        public readonly bool IsCreated => _vertexBuffer.IsCreated && _indexBuffer.IsCreated;
        public readonly int VerticesLength => _vertexBuffer.Length / sizeof(TVertex);
        public readonly int IndicesLength => _indexBuffer.Length / sizeof(TIndex);

        public readonly NativeArray<TVertex> Vertices => _vertexBuffer.AsArray().Reinterpret<TVertex>(sizeof(byte));
        public readonly NativeArray<TIndex> Indices => _indexBuffer.AsArray().Reinterpret<TIndex>(sizeof(byte));

        public ref TVertex VertexAt(int i) => ref UnsafeUtility.As<byte, TVertex>(ref _vertexBuffer.ElementAt(i * sizeof(TVertex)));
        public ref TIndex IndexAt(int i) => ref UnsafeUtility.As<byte, TIndex>(ref _indexBuffer.ElementAt(i * sizeof(TIndex)));

        public void Clear()
        {
            _vertexBuffer.Clear();
            _indexBuffer.Clear();
        }

        public readonly void AppendPathFill(NativePathBuilder pathBuilder, FillOptions? fillOptions = null)
        {
            this.ToRust().AppendPathFill(pathBuilder, fillOptions);
        }

        public readonly void AppendPathStroke(NativePathBuilder pathBuilder, StrokeOptions? strokeOptions = null)
        {
            this.ToRust().AppendPathStroke(pathBuilder, strokeOptions);
        }

        public void Dispose()
        {
            if (_vertexBuffer.IsCreated)
            {
                _vertexBuffer.Dispose();
            }
            if (_indexBuffer.IsCreated)
            {
                _indexBuffer.Dispose();
            }
        }

        public readonly JobHandle Dispose(JobHandle inputDeps)
        {
            return JobHandle.CombineDependencies(
                _vertexBuffer.Dispose(inputDeps),
                _indexBuffer.Dispose(inputDeps)
            );
        }
    }
}
