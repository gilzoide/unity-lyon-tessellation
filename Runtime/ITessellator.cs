using Unity.Collections;

namespace Gilzoide.LyonTesselation
{
    public interface ITessellator
    {
        void Clear();
        void AppendPathFill(NativePathBuilder pathBuilder, FillOptions? fillOptions = null);
        void AppendPathStroke(NativePathBuilder pathBuilder, StrokeOptions? strokeOptions = null);
    }

    public interface ITessellator<TVertex, TIndex> : ITessellator
        where TVertex : unmanaged
        where TIndex : unmanaged
    {
        NativeArray<TVertex> Vertices { get; }
        NativeArray<TIndex> Indices { get; }
    }
}
