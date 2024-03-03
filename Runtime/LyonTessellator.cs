using System;
using System.Runtime.InteropServices;
using AOT;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    public unsafe struct LyonTessellator<TGeometryBuilder>
        where TGeometryBuilder : struct, IGeometryBuilder
    {
        private LyonGeometryBuilderVTable _vTable;
        public TGeometryBuilder GeometryBuilder;

        public LyonTessellator(TGeometryBuilder geometryBuilder)
        {
            _vTable = new LyonGeometryBuilderVTable
            {
                AddVertex_ptr = Marshal.GetFunctionPointerForDelegate<LyonGeometryBuilderVTable.AddVertexDelegate>(AddVertex),
                AddIndex_ptr = Marshal.GetFunctionPointerForDelegate<LyonGeometryBuilderVTable.AddIndexDelegate>(AddIndex),
            };
            GeometryBuilder = geometryBuilder;
        }

        public static implicit operator LyonTessellator<TGeometryBuilder>(TGeometryBuilder geometryBuilder)
        {
            return new LyonTessellator<TGeometryBuilder>(geometryBuilder);
        }

        public static implicit operator TGeometryBuilder(LyonTessellator<TGeometryBuilder> geometryBuilder)
        {
            return geometryBuilder.GeometryBuilder;
        }

        public void FillPath(NativePathBuilder pathBuilder, FillOptions? fillOptions = null)
        {
            FillOptions options = fillOptions ?? FillOptions.Default();
            RustInterop.lyon_unity_triangulate_fill(
                ref _vTable,
                (Vector2*) pathBuilder.Points.GetUnsafeReadOnlyPtr(),
                (byte*) pathBuilder.Verbs.GetUnsafeReadOnlyPtr(),
                pathBuilder.Verbs.Length,
                ref options
            );
        }

        public void StrokePath(NativePathBuilder pathBuilder, StrokeOptions? strokeOptions = null)
        {
            StrokeOptions options = strokeOptions ?? StrokeOptions.Default();
            RustInterop.lyon_unity_triangulate_stroke(
                ref _vTable,
                (Vector2*) pathBuilder.Points.GetUnsafeReadOnlyPtr(),
                (byte*) pathBuilder.Verbs.GetUnsafeReadOnlyPtr(),
                pathBuilder.Verbs.Length,
                ref options
            );
        }

        [MonoPInvokeCallback(typeof(LyonGeometryBuilderVTable.AddVertexDelegate))]
        private static uint AddVertex(LyonGeometryBuilderVTable* geometryBuilder, float x, float y)
        {
            return UnsafeUtility.AsRef<TGeometryBuilder>(&geometryBuilder[1]).AddVertex(x, y);
        }

        [MonoPInvokeCallback(typeof(LyonGeometryBuilderVTable.AddIndexDelegate))]
        private static void AddIndex(LyonGeometryBuilderVTable* geometryBuilder, uint index)
        {
            UnsafeUtility.AsRef<TGeometryBuilder>(&geometryBuilder[1]).AddIndex(index);
        }
    }

    internal unsafe struct LyonGeometryBuilderVTable
    {
        [NativeDisableUnsafePtrRestriction] public IntPtr AddVertex_ptr;
        [NativeDisableUnsafePtrRestriction] public IntPtr AddIndex_ptr;

        public delegate uint AddVertexDelegate(LyonGeometryBuilderVTable* geometryBuilder, float x, float y);
        public delegate void AddIndexDelegate(LyonGeometryBuilderVTable* geometryBuilder, uint index);
    }

    internal struct GeometryBuilderHandle : IGeometryBuilder, IDisposable
    {
        GCHandle _geometryBuilderHandle;

        public GeometryBuilderHandle(IGeometryBuilder geometryBuilder)
        {
            if (geometryBuilder == null)
            {
                throw new ArgumentNullException(nameof(geometryBuilder));
            }
            _geometryBuilderHandle = GCHandle.Alloc(geometryBuilder);
        }

        public void Dispose()
        {
            _geometryBuilderHandle.Free();
        }

        public uint AddVertex(float x, float y)
        {
            return ((IGeometryBuilder) _geometryBuilderHandle.Target).AddVertex(x, y);
        }

        public void AddIndex(uint index)
        {
            ((IGeometryBuilder) _geometryBuilderHandle.Target).AddIndex(index);
        }
    }

    internal unsafe static class RustInterop
    {
        internal const string LibraryPath = "lyon_unity";

        [DllImport(LibraryPath)]
        internal static extern int lyon_unity_triangulate_fill(ref LyonGeometryBuilderVTable vTable, Vector2* points, byte* verbs, int verbsLength, ref FillOptions options);

        [DllImport(LibraryPath)]
        internal static extern int lyon_unity_triangulate_stroke(ref LyonGeometryBuilderVTable vTable, Vector2* points, byte* verbs, int verbsLength, ref StrokeOptions options);
    }
}
