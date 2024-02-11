using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    [BurstCompile]
    public struct TessellationFillJob : IJob
    {
        public TessellationFillJob(Tessellator tessellator, PathBuilder pathBuilder, FillOptions? fillOptions)
        {
            _tessellator = tessellator;
            _points = pathBuilder.Points;
            _verbs = pathBuilder.Verbs;
            _options = fillOptions;
        }

        private Tessellator _tessellator;
        [ReadOnly] private NativeArray<Vector2> _points;
        [ReadOnly] private NativeArray<PathBuilder.Verb> _verbs;
        private FillOptions? _options;

        public void Execute()
        {
            _tessellator.AppendPathFill(_points, _verbs, _options);
        }
    }

    [BurstCompile]
    public struct TessellationStrokeJob : IJob
    {
        public TessellationStrokeJob(Tessellator tessellator, PathBuilder pathBuilder, StrokeOptions? options)
        {
            _tessellator = tessellator;
            _points = pathBuilder.Points;
            _verbs = pathBuilder.Verbs;
            _options = options;
        }

        private Tessellator _tessellator;
        [ReadOnly] private NativeArray<Vector2> _points;
        [ReadOnly] private NativeArray<PathBuilder.Verb> _verbs;
        StrokeOptions? _options;

        public void Execute()
        {
            _tessellator.AppendPathStroke(_points, _verbs, _options);
        }
    }
}
