using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Gilzoide.LyonTesselation
{
    [BurstCompile]
    public struct TessellationFillJob<TGeometryBuilder> : IJob
        where TGeometryBuilder : struct, IGeometryBuilder
    {
        public TessellationFillJob(TGeometryBuilder geometryBuilder, NativePathBuilder pathBuilder, FillOptions? fillOptions)
        {
            _geometryBuilder = geometryBuilder;
            _pathBuilder = pathBuilder;
            _options = fillOptions;
        }

        private LyonTessellator<TGeometryBuilder> _geometryBuilder;
        [ReadOnly] private NativePathBuilder _pathBuilder;
        private FillOptions? _options;

        public readonly void Execute()
        {
            _geometryBuilder.FillPath(_pathBuilder, _options);
        }
    }

    [BurstCompile]
    public struct TessellationStrokeJob<TGeometryBuilder> : IJob
        where TGeometryBuilder : struct, IGeometryBuilder
    {
        public TessellationStrokeJob(TGeometryBuilder geometryBuilder, NativePathBuilder pathBuilder, StrokeOptions? strokeOptions)
        {
            _geometryBuilder = geometryBuilder;
            _pathBuilder = pathBuilder;
            _options = strokeOptions;
        }

        private LyonTessellator<TGeometryBuilder> _geometryBuilder;
        [ReadOnly] private NativePathBuilder _pathBuilder;
        private StrokeOptions? _options;

        public readonly void Execute()
        {
            _geometryBuilder.StrokePath(_pathBuilder, _options);
        }
    }
}
