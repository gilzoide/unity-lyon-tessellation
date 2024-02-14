using System;
using Unity.Collections;

namespace Gilzoide.LyonTesselation
{
    public class Tessellator<TVertex, TIndex> : ITessellator<TVertex, TIndex>, IDisposable
        where TVertex : unmanaged
        where TIndex : unmanaged
    {
        public NativeTessellator<TVertex, TIndex> NativeHandle => _nativeHandle;
        private readonly NativeTessellator<TVertex, TIndex> _nativeHandle;

        public Tessellator()
        {
            _nativeHandle = NativeTessellator<TVertex, TIndex>.Allocate();
        }

        public bool IsCreated => _nativeHandle.IsCreated;

        public NativeArray<TVertex> Vertices => _nativeHandle.Vertices;
        public NativeArray<TIndex> Indices => _nativeHandle.Indices;

        public void Clear()
        {
            _nativeHandle.Clear();
        }

        public void AppendPathFill(NativePathBuilder pathBuilder, FillOptions? fillOptions = null)
        {
            _nativeHandle.AppendPathFill(pathBuilder, fillOptions);
        }

        public void AppendPathStroke(NativePathBuilder pathBuilder, StrokeOptions? strokeOptions = null)
        {
            _nativeHandle.AppendPathStroke(pathBuilder, strokeOptions);
        }

        public void Dispose()
        {
            _nativeHandle.Dispose();
        }

        public static implicit operator NativeTessellator<TVertex, TIndex>(Tessellator<TVertex, TIndex> tessellator)
        {
            return tessellator?._nativeHandle ?? default;
        }
    }
}
