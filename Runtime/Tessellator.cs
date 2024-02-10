using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    public class Tessellator : IDisposable
    {
        public IntPtr NativeHandle { get; private set; }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle _atomicSafetyHandle;
        private static readonly int _safetyId = AtomicSafetyHandle.NewStaticSafetyId<Tessellator>();
#endif

        public Tessellator()
        {
            NativeHandle = LyonUnity.lyon_unity_buffer_new();
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            _atomicSafetyHandle = AtomicSafetyHandle.Create();
            AtomicSafetyHandle.SetStaticSafetyId(ref _atomicSafetyHandle, _safetyId);
            AtomicSafetyHandle.SetAllowSecondaryVersionWriting(_atomicSafetyHandle, false);
#endif
        }

        ~Tessellator()
        {
            Dispose();
        }

        public NativeArray<Vector2> Vertices
        {
            get
            {
                unsafe
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    AtomicSafetyHandle.CheckGetSecondaryDataPointerAndThrow(_atomicSafetyHandle);

                    AtomicSafetyHandle secondarySafetyHandle = _atomicSafetyHandle;
                    AtomicSafetyHandle.UseSecondaryVersion(ref secondarySafetyHandle);
#endif
                    LyonUnity.lyon_unity_buffer_get_vertices(NativeHandle, out Vector2* ptr, out int size);
                    NativeArray<Vector2> slice = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector2>(ptr, size, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref slice, secondarySafetyHandle);
#endif
                    return slice;
                }
            }
        }

        public NativeArray<ushort> Indices
        {
            get
            {
                unsafe
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    AtomicSafetyHandle.CheckGetSecondaryDataPointerAndThrow(_atomicSafetyHandle);

                    AtomicSafetyHandle secondarySafetyHandle = _atomicSafetyHandle;
                    AtomicSafetyHandle.UseSecondaryVersion(ref secondarySafetyHandle);
#endif
                    LyonUnity.lyon_unity_buffer_get_indices(NativeHandle, out ushort* ptr, out int size);
                    NativeArray<ushort> slice = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ushort>(ptr, size, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref slice, secondarySafetyHandle);
#endif
                    return slice;
                }
            }
        }

        public void Clear()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(_atomicSafetyHandle);
#endif
            LyonUnity.lyon_unity_buffer_clear(NativeHandle);
        }

        public unsafe void AppendPathFill(PathBuilder pathBuilder, FillOptions? fillOptions = null)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(_atomicSafetyHandle);
#endif
            var points = pathBuilder.Points;
            var verbs = pathBuilder.Verbs;
            var options = fillOptions ?? FillOptions.Default();

            LyonUnity.lyon_unity_triangulate_fill(
                NativeHandle,
                (Vector2*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(points),
                (byte*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(verbs),
                verbs.Length,
                ref options
            );
        }

        public unsafe void AppendPathStroke(PathBuilder pathBuilder)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(_atomicSafetyHandle);
#endif
            var points = pathBuilder.Points;
            var verbs = pathBuilder.Verbs;

            LyonUnity.lyon_unity_triangulate_stroke(
                NativeHandle,
                (Vector2*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(points),
                (byte*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(verbs),
                verbs.Length
            );
        }

        public TessellationFillJob CreatePathFillJob(PathBuilder pathBuilder, FillOptions? fillOptions = null)
        {
            return new TessellationFillJob(this, pathBuilder, fillOptions);
        }

        public TessellationStrokeJob CreatePathStrokeJob(PathBuilder pathBuilder)
        {
            return new TessellationStrokeJob(this, pathBuilder);
        }

        public void Dispose()
        {
            if (NativeHandle != IntPtr.Zero)
            {
                LyonUnity.lyon_unity_buffer_destroy(NativeHandle);
                NativeHandle = IntPtr.Zero;
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckDeallocateAndThrow(_atomicSafetyHandle);
            AtomicSafetyHandle.Release(_atomicSafetyHandle);
#endif
        }
    }
}
