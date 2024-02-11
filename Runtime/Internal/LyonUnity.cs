using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Gilzoide.LyonTesselation.Internal
{
    internal unsafe static class LyonUnity
    {
        public const string LibraryPath = "lyon_unity";

        [DllImport(LibraryPath)]
        public static extern IntPtr lyon_unity_buffer_new(int sizeofVertex, int sizeofIndex);

        [DllImport(LibraryPath)]
        public static extern void lyon_unity_buffer_destroy(IntPtr buffer);

        [DllImport(LibraryPath)]
        public static extern void lyon_unity_buffer_clear(IntPtr buffer);

        [DllImport(LibraryPath)]
        public static extern void lyon_unity_buffer_get_vertices(IntPtr buffer, out void* verticesPtr, out int verticesLength);

        [DllImport(LibraryPath)]
        public static extern void lyon_unity_buffer_get_indices(IntPtr buffer, out void* indicesPtr, out int indicesLength);

        [DllImport(LibraryPath)]
        public static extern void lyon_unity_triangulate_fill(IntPtr buffer, Vector2* points, byte* verbs, int verbsLength, ref FillOptions options);

        [DllImport(LibraryPath)]
        public static extern void lyon_unity_triangulate_stroke(IntPtr buffer, Vector2* points, byte* verbs, int verbsLength, ref StrokeOptions options);
    }
}
