using Gilzoide.LyonTesselation.Internal;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Gilzoide.LyonTesselation
{
    public static class TessellatorJobs
    {
        public static TessellationFillJob<TVertex, TIndex> CreatePathFillJob<TVertex, TIndex>(this Tessellator<TVertex, TIndex> tessellator, NativePathBuilder pathBuilder, FillOptions? options = null)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            return tessellator.NativeHandle.CreatePathFillJob(pathBuilder, options);
        }

        public static TessellationFillJob<TVertex, TIndex> CreatePathFillJob<TVertex, TIndex>(this NativeTessellator<TVertex, TIndex> tessellator, NativePathBuilder pathBuilder, FillOptions? options = null)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            return new TessellationFillJob<TVertex, TIndex>(tessellator, pathBuilder, options);
        }

        public static TessellationStrokeJob<TVertex, TIndex> CreatePathStrokeJob<TVertex, TIndex>(this Tessellator<TVertex, TIndex> tessellator, NativePathBuilder pathBuilder, StrokeOptions? options)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            return tessellator.NativeHandle.CreatePathStrokeJob(pathBuilder, options);
        }

        public static TessellationStrokeJob<TVertex, TIndex> CreatePathStrokeJob<TVertex, TIndex>(this NativeTessellator<TVertex, TIndex> tessellator, NativePathBuilder pathBuilder, StrokeOptions? options)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            return new TessellationStrokeJob<TVertex, TIndex>(tessellator, pathBuilder, options);
        }
    }

    [BurstCompile]
    public struct TessellationFillJob<TVertex, TIndex> : IJob
        where TVertex : unmanaged
        where TIndex : unmanaged
    {
        public TessellationFillJob(NativeTessellator<TVertex, TIndex> tessellator, NativePathBuilder pathBuilder, FillOptions? fillOptions)
        {
            _tessellator = tessellator.ToRust();
            _pathBuilder = pathBuilder;
            _options = fillOptions;
        }

        private TessellatorRust _tessellator;
        [ReadOnly] private NativePathBuilder _pathBuilder;
        private FillOptions? _options;

        public readonly void Execute()
        {
            _tessellator.AppendPathFill(_pathBuilder, _options);
        }
    }

    [BurstCompile]
    public struct TessellationStrokeJob<TVertex, TIndex> : IJob
        where TVertex : unmanaged
        where TIndex : unmanaged
    {
        public TessellationStrokeJob(NativeTessellator<TVertex, TIndex> tessellator, NativePathBuilder pathBuilder, StrokeOptions? options)
        {
            _tessellator = tessellator.ToRust();
            _pathBuilder = pathBuilder;
            _options = options;
        }

        private TessellatorRust _tessellator;
        [ReadOnly] private NativePathBuilder _pathBuilder;
        StrokeOptions? _options;

        public readonly void Execute()
        {
            _tessellator.AppendPathStroke(_pathBuilder, _options);
        }
    }
}
