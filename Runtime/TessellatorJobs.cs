using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    [BurstCompile]
    public struct TessellationFillJob : IJob
    {
        public TessellationFillJob(Tessellator tessellator, PathBuilder pathBuilder, FillOptions? fillOptions)
        {
            _tessellatorHandle = tessellator.NativeHandle;
            _points = pathBuilder.Points;
            _verbs = pathBuilder.Verbs;
            _options = fillOptions;
        }

        [NativeDisableUnsafePtrRestriction] private IntPtr _tessellatorHandle;
        private NativeArray<Vector2>.ReadOnly _points;
        private NativeArray<PathBuilder.Verb>.ReadOnly _verbs;
        private FillOptions? _options;

        public unsafe static void ExecuteStatic(IntPtr tessellatorHandle, NativeArray<Vector2>.ReadOnly points, NativeArray<PathBuilder.Verb>.ReadOnly verbs, FillOptions? options)
        {
            FillOptions fillOptions = options ?? FillOptions.Default();
            LyonUnity.lyon_unity_triangulate_fill(
                tessellatorHandle,
                (Vector2*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(points),
                (byte*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(verbs),
                verbs.Length,
                ref fillOptions
            );
        }

        public void Execute()
        {
            ExecuteStatic(_tessellatorHandle, _points, _verbs, _options);
        }
    }

    [BurstCompile]
    public struct TessellationStrokeJob : IJob
    {
        public TessellationStrokeJob(Tessellator tessellator, PathBuilder pathBuilder, StrokeOptions? options)
        {
            _tessellatorHandle = tessellator.NativeHandle;
            _points = pathBuilder.Points;
            _verbs = pathBuilder.Verbs;
            _options = options;
        }

        [NativeDisableUnsafePtrRestriction] private IntPtr _tessellatorHandle;
        private NativeArray<Vector2>.ReadOnly _points;
        private NativeArray<PathBuilder.Verb>.ReadOnly _verbs;
        StrokeOptions? _options;

        public unsafe static void ExecuteStatic(IntPtr tessellatorHandle, NativeArray<Vector2>.ReadOnly points, NativeArray<PathBuilder.Verb>.ReadOnly verbs, StrokeOptions? options)
        {
            StrokeOptions strokeOptions = options ?? StrokeOptions.Default();
            LyonUnity.lyon_unity_triangulate_stroke(
                tessellatorHandle,
                (Vector2*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(points),
                (byte*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(verbs),
                verbs.Length,
                ref strokeOptions
            );
        }

        public void Execute()
        {
            ExecuteStatic(_tessellatorHandle, _points, _verbs, _options);
        }
    }
}
