namespace Gilzoide.LyonTesselation
{
    public interface IGeometryBuilder
    {
        uint AddVertex(float x, float y);
        void AddTriangle(uint index1, uint index2, uint index3);
    }

    public static class GeometryBuilderError
    {
        public const uint InvalidVertex = uint.MaxValue - 1;
        public const uint TooManyVertices = uint.MaxValue;
    }

    public static class IGeometryBuilderExtensions
    {
        public static void FillPath<TGeometryBuilder>(this TGeometryBuilder geometryBuilder, NativePathBuilder pathBuilder, FillOptions? fillOptions = null)
            where TGeometryBuilder : struct, IGeometryBuilder
        {
            new LyonTessellator<TGeometryBuilder>(geometryBuilder).FillPath(pathBuilder, fillOptions);
        }

        public static void StrokePath<TGeometryBuilder>(this TGeometryBuilder geometryBuilder, NativePathBuilder pathBuilder, StrokeOptions? strokeOptions = null)
            where TGeometryBuilder : struct, IGeometryBuilder
        {
            new LyonTessellator<TGeometryBuilder>(geometryBuilder).StrokePath(pathBuilder, strokeOptions);
        }

        public static void FillPath(this IGeometryBuilder geometryBuilder, NativePathBuilder pathBuilder, FillOptions? fillOptions = null)
        {
            using (var handle = new GeometryBuilderHandle(geometryBuilder))
            {
                new LyonTessellator<GeometryBuilderHandle>(handle).FillPath(pathBuilder, fillOptions);
            }
        }

        public static void StrokePath(this IGeometryBuilder geometryBuilder, NativePathBuilder pathBuilder, StrokeOptions? strokeOptions = null)
        {
            using (var handle = new GeometryBuilderHandle(geometryBuilder))
            {
                new LyonTessellator<GeometryBuilderHandle>(handle).StrokePath(pathBuilder, strokeOptions);
            }
        }

        public static TessellationFillJob<NativeGeometryBuilder<TVertex, TIndex>> CreatePathFillJob<TVertex, TIndex>(this GeometryBuilder<TVertex, TIndex> geometryBuilder, NativePathBuilder pathBuilder, FillOptions? options)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            return geometryBuilder.NativeHandle.CreatePathFillJob(pathBuilder, options);
        }

        public static TessellationFillJob<TGeometryBuilder> CreatePathFillJob<TGeometryBuilder>(this TGeometryBuilder geometryBuilder, NativePathBuilder pathBuilder, FillOptions? options = null)
            where TGeometryBuilder : struct, IGeometryBuilder
        {
            return new TessellationFillJob<TGeometryBuilder>(geometryBuilder, pathBuilder, options);
        }

        public static TessellationStrokeJob<NativeGeometryBuilder<TVertex, TIndex>> CreatePathStrokeJob<TVertex, TIndex>(this GeometryBuilder<TVertex, TIndex> geometryBuilder, NativePathBuilder pathBuilder, StrokeOptions? options)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            return geometryBuilder.NativeHandle.CreatePathStrokeJob(pathBuilder, options);
        }

        public static TessellationStrokeJob<TGeometryBuilder> CreatePathStrokeJob<TGeometryBuilder>(this TGeometryBuilder geometryBuilder, NativePathBuilder pathBuilder, StrokeOptions? options)
            where TGeometryBuilder : struct, IGeometryBuilder
        {
            return new TessellationStrokeJob<TGeometryBuilder>(geometryBuilder, pathBuilder, options);
        }
    }
}
