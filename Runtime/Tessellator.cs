using System;
using Gilzoide.LyonTesselation.Internal;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    [NativeContainer]
    public struct Tessellator : IDisposable
    {
        [field: NativeDisableUnsafePtrRestriction]
        public IntPtr NativeHandle { get; private set; }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety;
        private static readonly int _safetyId = AtomicSafetyHandle.NewStaticSafetyId<Tessellator>();
#endif

        public static Tessellator Create()
        {
            return new Tessellator(LyonUnity.lyon_unity_buffer_new());
        }

        public Tessellator(IntPtr nativeHandle)
        {
            NativeHandle = nativeHandle;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Safety = AtomicSafetyHandle.Create();
            AtomicSafetyHandle.SetStaticSafetyId(ref m_Safety, _safetyId);
            AtomicSafetyHandle.SetAllowSecondaryVersionWriting(m_Safety, false);
#endif
        }

        public readonly bool IsCreated => NativeHandle != IntPtr.Zero;

        public readonly NativeArray<Vector2> Vertices
        {
            get
            {
                ThrowIfNotCreated();
                unsafe
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    AtomicSafetyHandle.CheckGetSecondaryDataPointerAndThrow(m_Safety);

                    AtomicSafetyHandle secondarySafetyHandle = m_Safety;
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

        public readonly NativeArray<ushort> Indices
        {
            get
            {
                ThrowIfNotCreated();
                unsafe
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    AtomicSafetyHandle.CheckGetSecondaryDataPointerAndThrow(m_Safety);

                    AtomicSafetyHandle secondarySafetyHandle = m_Safety;
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
            ThrowIfNotCreated();
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            LyonUnity.lyon_unity_buffer_clear(NativeHandle);
        }

        public readonly void AppendPathFill(PathBuilder pathBuilder, FillOptions? options = null)
        {
            AppendPathFill(pathBuilder.Points, pathBuilder.Verbs, options);
        }

        public readonly unsafe void AppendPathFill(NativeArray<Vector2> points, NativeArray<PathBuilder.Verb> verbs, FillOptions? fillOptions = null)
        {
            ThrowIfNotCreated();
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            FillOptions options = fillOptions ?? FillOptions.Default();
            LyonUnity.lyon_unity_triangulate_fill(
                NativeHandle,
                (Vector2*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(points),
                (byte*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(verbs),
                verbs.Length,
                ref options
            );
        }

        public readonly void AppendPathStroke(PathBuilder pathBuilder, StrokeOptions? strokeOptions = null)
        {
            AppendPathStroke(pathBuilder.Points, pathBuilder.Verbs, strokeOptions);
        }

        public readonly unsafe void AppendPathStroke(NativeArray<Vector2> points, NativeArray<PathBuilder.Verb> verbs, StrokeOptions? strokeOptions = null)
        {
            ThrowIfNotCreated();
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            StrokeOptions options = strokeOptions ?? StrokeOptions.Default();
            LyonUnity.lyon_unity_triangulate_stroke(
                NativeHandle,
                (Vector2*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(points),
                (byte*) NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(verbs),
                verbs.Length,
                ref options
            );
        }

        public readonly TessellationFillJob CreatePathFillJob(PathBuilder pathBuilder, FillOptions? options = null)
        {
            return new TessellationFillJob(this, pathBuilder, options);
        }

        public readonly TessellationStrokeJob CreatePathStrokeJob(PathBuilder pathBuilder, StrokeOptions? options)
        {
            return new TessellationStrokeJob(this, pathBuilder, options);
        }

        public void Dispose()
        {
            if (NativeHandle != IntPtr.Zero)
            {
                LyonUnity.lyon_unity_buffer_destroy(NativeHandle);
                NativeHandle = IntPtr.Zero;
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (AtomicSafetyHandle.IsHandleValid(m_Safety))
            {
                AtomicSafetyHandle.CheckDeallocateAndThrow(m_Safety);
                AtomicSafetyHandle.Release(m_Safety);
            }
#endif
        }

        internal readonly void ThrowIfNotCreated()
        {
            if (!IsCreated)
            {
                throw new NullReferenceException($"{nameof(Tessellator)} was Disposed of or never created.");
            }
        }
    }
}
