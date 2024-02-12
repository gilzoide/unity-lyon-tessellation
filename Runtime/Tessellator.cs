using System;
using Gilzoide.LyonTesselation.Internal;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    [NativeContainer]
    public struct Tessellator<TVertex, TIndex> : IDisposable
        where TVertex : unmanaged
        where TIndex : unmanaged
    {
        [field: NativeDisableUnsafePtrRestriction]
        public IntPtr NativeHandle { get; private set; }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety;
        private static readonly int _safetyId = AtomicSafetyHandle.NewStaticSafetyId<Tessellator<TVertex, TIndex>>();
#endif

        public static Tessellator<TVertex, TIndex> Allocate()
        {
            unsafe
            {
                if (sizeof(TVertex) < 2 * sizeof(float))
                {
                    throw new ArgumentException("Vertex size must fit at least 2 floats.", nameof(TVertex));
                }
                if (sizeof(TIndex) < sizeof(ushort))
                {
                    throw new ArgumentException("Index size must fit at least one ushort.", nameof(TIndex));
                }
                IntPtr nativeHandle = LyonUnity.lyon_unity_buffer_new(sizeof(TVertex), sizeof(TIndex));
                return new Tessellator<TVertex, TIndex>(nativeHandle);
            }
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

        public readonly NativeArray<TVertex> Vertices
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
                    LyonUnity.lyon_unity_buffer_get_vertices(NativeHandle, out void* ptr, out int size);
                    NativeArray<TVertex> array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<TVertex>(ptr, size, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, secondarySafetyHandle);
#endif
                    return array;
                }
            }
        }

        public readonly NativeArray<TIndex> Indices
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
                    LyonUnity.lyon_unity_buffer_get_indices(NativeHandle, out void* ptr, out int size);
                    NativeArray<TIndex> array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<TIndex>(ptr, size, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, secondarySafetyHandle);
#endif
                    return array;
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
                throw new NullReferenceException("Tessellator was Disposed of or never created.");
            }
        }
    }
}
