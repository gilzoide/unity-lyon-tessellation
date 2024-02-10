using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    public struct TessellationFillJob : IJob
    {
        public TessellationFillJob(Tessellator tessellator, PathBuilder pathBuilder, FillOptions? fillOptions)
        {
            _tessellatorHandle = tessellator.NativeHandle;
            _points = pathBuilder.Points;
            _verbs = pathBuilder.Verbs;
            _fillOptions = fillOptions;
        }

        [NativeDisableUnsafePtrRestriction] private IntPtr _tessellatorHandle;
        private NativeArray<Vector2>.ReadOnly _points;
        private NativeArray<PathBuilder.Verb>.ReadOnly _verbs;
        private FillOptions? _fillOptions;

        public unsafe void Execute()
        {
            FillOptions fillOptions = _fillOptions ?? FillOptions.Default();
            LyonUnity.lyon_unity_triangulate_fill(
                _tessellatorHandle,
                (Vector2*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(_points),
                (byte*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(_verbs),
                _verbs.Length,
                ref fillOptions
            );
        }
    }

    public struct TessellationStrokeJob : IJob
    {
        public TessellationStrokeJob(Tessellator tessellator, PathBuilder pathBuilder)
        {
            _tessellatorHandle = tessellator.NativeHandle;
            _points = pathBuilder.Points;
            _verbs = pathBuilder.Verbs;
        }

        [NativeDisableUnsafePtrRestriction] private IntPtr _tessellatorHandle;
        private NativeArray<Vector2>.ReadOnly _points;
        private NativeArray<PathBuilder.Verb>.ReadOnly _verbs;

        public unsafe void Execute()
        {
            LyonUnity.lyon_unity_triangulate_stroke(
                _tessellatorHandle,
                (Vector2*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(_points),
                (byte*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(_verbs),
                _verbs.Length
            );
        }
    }
}
