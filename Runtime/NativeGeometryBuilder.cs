using System;
using Unity.Collections;
using Unity.Jobs;

namespace Gilzoide.LyonTesselation
{
    public unsafe struct NativeGeometryBuilder<TVertex, TIndex> : IGeometryBuilder, IDisposable, INativeDisposable
        where TVertex : unmanaged
        where TIndex : unmanaged
    {
        private static readonly bool IsIndex16 = sizeof(TIndex) < sizeof(uint);

        public readonly bool IsCreated => Vertices.IsCreated && Indices.IsCreated;

        public NativeList<TVertex> Vertices;
        public NativeList<TIndex> Indices;

        public NativeGeometryBuilder(Allocator allocator)
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
            Vertices = new NativeList<TVertex>(allocator);
            Indices = new NativeList<TIndex>(allocator);
        }

        public void Clear()
        {
            Vertices.Clear();
            Indices.Clear();
        }

        public void Dispose()
        {
            if (Vertices.IsCreated)
            {
                Vertices.Dispose();
            }
            if (Indices.IsCreated)
            {
                Indices.Dispose();
            }
        }

        public readonly JobHandle Dispose(JobHandle inputDeps)
        {
            return JobHandle.CombineDependencies(
                Vertices.Dispose(inputDeps),
                Indices.Dispose(inputDeps)
            );
        }

        public uint AddVertex(float x, float y)
        {
            int currentIndex = Vertices.Length;
            if (currentIndex >= int.MaxValue
                || (IsIndex16 && currentIndex >= ushort.MaxValue))
            {
                return GeometryBuilderError.TooManyVertices;
            }
            else if (float.IsNaN(x) || float.IsNaN(y))
            {
                return GeometryBuilderError.InvalidVertex;
            }

            TVertex vertex = default;
            float* vertexPtr = (float*) &vertex;
            vertexPtr[0] = x;
            vertexPtr[1] = y;
            Vertices.Add(vertex);
            return (uint) currentIndex;
        }

        public void AddTriangle(uint index1, uint index2, uint index3)
        {
            AddIndex(index1);
            AddIndex(index2);
            AddIndex(index3);
        }

        public void AddIndex(uint index)
        {
            TIndex newIndex = default;
            if (IsIndex16)
            {
                *(ushort*) &newIndex = (ushort) index;
            }
            else
            {
                *(uint*) &newIndex = index;
            }
            Indices.Add(newIndex);
        }
    }
}
