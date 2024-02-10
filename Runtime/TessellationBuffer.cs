using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    public class TessellationBuffer : IDisposable
    {
        public IntPtr NativeHandle { get; private set; }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle _atomicSafetyHandle;
        private static readonly int _safetyId = AtomicSafetyHandle.NewStaticSafetyId<TessellationBuffer>();
#endif

        public TessellationBuffer()
        {
            NativeHandle = LyonUnity.lyon_unity_buffer_new();
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            _atomicSafetyHandle = AtomicSafetyHandle.Create();
            AtomicSafetyHandle.SetStaticSafetyId(ref _atomicSafetyHandle, _safetyId);
            AtomicSafetyHandle.SetAllowSecondaryVersionWriting(_atomicSafetyHandle, false);
#endif
        }

        ~TessellationBuffer()
        {
            Dispose();
        }

        public NativeSlice<Vector2> Vertices
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
                    NativeSlice<Vector2> slice = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<Vector2>(ptr, 0, size);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref slice, secondarySafetyHandle);
#endif
                    return slice;
                }
            }
        }

        public NativeSlice<ushort> Indices
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
                    NativeSlice<ushort> slice = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<ushort>(ptr, 0, size);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref slice, secondarySafetyHandle);
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

        public unsafe void TriangulateFill(PathEvent* events, int eventsLength)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(_atomicSafetyHandle);
#endif
            LyonUnity.lyon_unity_triangulate_fill(NativeHandle, events, eventsLength);
        }

        public void TriangulateFill(PathEvent[] events)
        {
            unsafe
            {
                fixed (PathEvent* ptr = events)
                {
                    TriangulateFill(ptr, events.Length);
                }
            }
        }

#if UNITY_2021_2_OR_NEWER
        public void TriangulateFill(ReadOnlySpan<PathEvent> events)
        {
            unsafe
            {
                fixed (PathEvent* ptr = events)
                {
                    TriangulateFill(ptr, events.Length);
                }
            }
        }
#endif

        public unsafe void TriangulateStroke(PathEvent* events, int eventsLength)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(_atomicSafetyHandle);
#endif
            LyonUnity.lyon_unity_triangulate_stroke(NativeHandle, events, eventsLength);
        }

        public void TriangulateStroke(PathEvent[] events)
        {
            unsafe
            {
                fixed (PathEvent* ptr = events)
                {
                    TriangulateStroke(ptr, events.Length);
                }
            }
        }

#if UNITY_2021_2_OR_NEWER
        public void TriangulateStroke(ReadOnlySpan<PathEvent> events)
        {
            unsafe
            {
                fixed (PathEvent* ptr = events)
                {
                    TriangulateStroke(ptr, events.Length);
                }
            }
        }
#endif

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
