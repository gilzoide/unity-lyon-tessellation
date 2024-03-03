using System;
using Unity.Collections;

namespace Gilzoide.LyonTesselation
{
    public class GeometryBuilder<TVertex, TIndex> : IDisposable
        where TVertex : unmanaged
        where TIndex : unmanaged
    {

        public bool IsCreated => _nativeHandle.IsCreated;
        public NativeList<TVertex> Vertices => _nativeHandle.Vertices;
        public NativeList<TIndex> Indices => _nativeHandle.Indices;

        public NativeGeometryBuilder<TVertex, TIndex> NativeHandle => _nativeHandle;
        private NativeGeometryBuilder<TVertex, TIndex> _nativeHandle;

        public GeometryBuilder(Allocator allocator = Allocator.Persistent)
        {
            _nativeHandle = new NativeGeometryBuilder<TVertex, TIndex>(allocator);
        }

        public void Clear()
        {
            _nativeHandle.Clear();
        }

        public void Dispose()
        {
            _nativeHandle.Dispose();
        }

        public static implicit operator NativeGeometryBuilder<TVertex, TIndex>(GeometryBuilder<TVertex, TIndex> tessellator)
        {
            return tessellator?._nativeHandle ?? default;
        }
    }
}
