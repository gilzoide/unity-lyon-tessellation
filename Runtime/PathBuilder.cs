using System;
using Unity.Collections;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    public class PathBuilder : IDisposable
    {
        public NativeArray<Vector2> Points => _nativeHandle.Points;
        public NativeArray<PathVerb> Verbs => _nativeHandle.Verbs;

        public NativePathBuilder NativeHandle => _nativeHandle;
        private NativePathBuilder _nativeHandle;

        public PathBuilder(Allocator allocator = Allocator.Persistent)
        {
            _nativeHandle = new NativePathBuilder(allocator);
        }

        ~PathBuilder()
        {
            Dispose();
        }

        public PathBuilder BeginAt(Vector2 at)
        {
            _nativeHandle.BeginAt(at);
            return this;
        }

        public PathBuilder LineTo(Vector2 to)
        {
            _nativeHandle.LineTo(to);
            return this;
        }

        public PathBuilder QuadraticBezierTo(Vector2 controlPoint, Vector2 to)
        {
            _nativeHandle.QuadraticBezierTo(controlPoint, to);
            return this;
        }

        public PathBuilder CubicBezierTo(Vector2 controlPoint1, Vector2 controlPoint2, Vector2 to)
        {
            _nativeHandle.CubicBezierTo(controlPoint1, controlPoint2, to);
            return this;
        }

        public PathBuilder Close()
        {
            _nativeHandle.Close();
            return this;
        }

        public PathBuilder End()
        {
            _nativeHandle.End();
            return this;
        }

        public PathBuilder Clear()
        {
            _nativeHandle.Clear();
            return this;
        }

        public PathBuilder AddLine(Vector2 from, Vector2 to)
        {
            _nativeHandle.AddLine(from, to);
            return this;
        }

        public PathBuilder AddQuadraticBezier(Vector2 from, Vector2 control, Vector2 to)
        {
            _nativeHandle.AddQuadraticBezier(from, control, to);
            return this;
        }

        public PathBuilder AddCubicBezier(Vector2 from, Vector2 control1, Vector2 control2, Vector2 to)
        {
            _nativeHandle.AddCubicBezier(from, control1, control2, to);
            return this;
        }

        public PathBuilder AddEllipse(Vector2 center, Vector2 size)
        {
            _nativeHandle.AddEllipse(center, size);
            return this;
        }

        public PathBuilder AddCircle(Vector2 center, float radius)
        {
            _nativeHandle.AddCircle(center, radius);
            return this;
        }

        public PathBuilder AddRect(Rect rect)
        {
            _nativeHandle.AddRect(rect);
            return this;
        }

        public PathBuilder AddRoundedRect(Rect rect, float cornerRadius)
        {
            _nativeHandle.AddRoundedRect(rect, cornerRadius);
            return this;
        }

        public void Dispose()
        {
            _nativeHandle.Dispose();
        }

        public static implicit operator NativePathBuilder(PathBuilder pathBuilder)
        {
            return pathBuilder?._nativeHandle ?? default;
        }
    }
}
