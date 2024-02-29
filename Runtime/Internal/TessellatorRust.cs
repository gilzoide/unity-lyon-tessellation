using System;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Gilzoide.LyonTesselation.Internal
{
    internal unsafe struct TessellatorRust
    {
        [NativeDisableUnsafePtrRestriction] public UnsafeList<byte>* VerticesListPtr;
        [NativeDisableUnsafePtrRestriction] public UnsafeList<byte>* IndicesListPtr;

        [NativeDisableUnsafePtrRestriction] public IntPtr PushBytesFunctionPtr;

        public int VertexSize;
        public int IndexSize;

        public byte IsIndex16;

        public TessellatorRust(NativeList<byte> vertices, NativeList<byte> indices, int vertexSize, int indexSize)
        {
            VerticesListPtr = vertices.GetUnsafeList();
            IndicesListPtr = indices.GetUnsafeList();
            PushBytesFunctionPtr = RustInterop.PushBytesFunctionPtr.Data;
            VertexSize = vertexSize;
            IndexSize = indexSize;
            IsIndex16 = (byte) (indexSize < sizeof(int) ? 1 : 0);
        }

        public void AppendPathFill(NativePathBuilder pathBuilder, FillOptions? fillOptions = null)
        {
            FillOptions options = fillOptions ?? FillOptions.Default();
            RustInterop.lyon_unity_triangulate_fill(
                ref this,
                (Vector2*) pathBuilder.Points.GetUnsafeReadOnlyPtr(),
                (byte*) pathBuilder.Verbs.GetUnsafeReadOnlyPtr(),
                pathBuilder.Verbs.Length,
                ref options
            );
        }

        public void AppendPathStroke(NativePathBuilder pathBuilder, StrokeOptions? strokeOptions = null)
        {
            StrokeOptions options = strokeOptions ?? StrokeOptions.Default();
            RustInterop.lyon_unity_triangulate_stroke(
                ref this,
                (Vector2*) pathBuilder.Points.GetUnsafeReadOnlyPtr(),
                (byte*) pathBuilder.Verbs.GetUnsafeReadOnlyPtr(),
                pathBuilder.Verbs.Length,
                ref options
            );
        }
    }

    internal unsafe static class RustInterop
    {
        internal const string LibraryPath = "lyon_unity";

        [DllImport(LibraryPath)]
        internal static extern int lyon_unity_triangulate_fill(ref TessellatorRust buffer, Vector2* points, byte* verbs, int verbsLength, ref FillOptions options);

        [DllImport(LibraryPath)]
        internal static extern int lyon_unity_triangulate_stroke(ref TessellatorRust buffer, Vector2* points, byte* verbs, int verbsLength, ref StrokeOptions options);

        internal delegate byte* PushBytesDelegate(UnsafeList<byte>* list, int size);
        [MonoPInvokeCallback(typeof(PushBytesDelegate))]
        internal static byte* PushBytes(UnsafeList<byte>* list, int size)
        {
            var currentSize = list->Length;
            list->Resize(currentSize + size, NativeArrayOptions.UninitializedMemory);
            return list->Ptr + currentSize;
        }
        internal static readonly SharedStatic<IntPtr> PushBytesFunctionPtr = SharedStatic<IntPtr>.GetOrCreate<PushBytesDelegate>();

        internal static TessellatorRust ToRust<TVertex, TIndex>(this NativeTessellator<TVertex, TIndex> tessellator)
            where TVertex : unmanaged
            where TIndex : unmanaged
        {
            return new TessellatorRust(tessellator.VertexBuffer, tessellator.IndexBuffer, sizeof(TVertex), sizeof(TIndex));
        }

        internal static void InitializeSharedStatic()
        {
            PushBytesFunctionPtr.Data = Marshal.GetFunctionPointerForDelegate<PushBytesDelegate>(PushBytes);
        }
    }
}
