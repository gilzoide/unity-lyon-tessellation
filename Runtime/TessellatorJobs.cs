using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    public static class TessellatorJobs
    {
        public static TessellationFillJob<TVertex, TIndex> CreatePathFillJob<TVertex, TIndex>(this Tessellator<TVertex, TIndex> self, PathBuilder pathBuilder, FillOptions? options = null)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            return new TessellationFillJob<TVertex, TIndex>(self, pathBuilder, options);
        }

        public static TessellationStrokeJob<TVertex, TIndex> CreatePathStrokeJob<TVertex, TIndex>(this Tessellator<TVertex, TIndex> self, PathBuilder pathBuilder, StrokeOptions? options)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            return new TessellationStrokeJob<TVertex, TIndex>(self, pathBuilder, options);
        }
    }

    [BurstCompile]
    public struct TessellationFillJob<TVertex, TIndex> : IJob
        where TVertex : unmanaged
        where TIndex : unmanaged
    {
        public TessellationFillJob(Tessellator<TVertex, TIndex> tessellator, PathBuilder pathBuilder, FillOptions? fillOptions)
        {
            _tessellator = tessellator;
            _points = pathBuilder.Points;
            _verbs = pathBuilder.Verbs;
            _options = fillOptions;
        }

        private Tessellator<TVertex, TIndex> _tessellator;
        [ReadOnly] private NativeArray<Vector2> _points;
        [ReadOnly] private NativeArray<PathBuilder.Verb> _verbs;
        private FillOptions? _options;

        public readonly void Execute()
        {
            _tessellator.AppendPathFill(_points, _verbs, _options);
        }
    }

    [BurstCompile]
    public struct TessellationStrokeJob<TVertex, TIndex> : IJob
        where TVertex : unmanaged
        where TIndex : unmanaged
    {
        public TessellationStrokeJob(Tessellator<TVertex, TIndex> tessellator, PathBuilder pathBuilder, StrokeOptions? options)
        {
            _tessellator = tessellator;
            _points = pathBuilder.Points;
            _verbs = pathBuilder.Verbs;
            _options = options;
        }

        private Tessellator<TVertex, TIndex> _tessellator;
        [ReadOnly] private NativeArray<Vector2> _points;
        [ReadOnly] private NativeArray<PathBuilder.Verb> _verbs;
        StrokeOptions? _options;

        public readonly void Execute()
        {
            _tessellator.AppendPathStroke(_points, _verbs, _options);
        }
    }
}
