using System;
using Unity.Collections;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    public class PathBuilder : IDisposable
    {
        public enum Verb : byte
        {
            Begin = 0,
            LineTo = 1,
            QuadraticTo = 2,
            CubicTo = 3,
            Close = 4,
            End = 5,
        }

        public NativeArray<Vector2> Points => _points;
        public NativeArray<Verb> Verbs => _verbs;

        private NativeList<Vector2> _points;
        private NativeList<Verb> _verbs;
        private bool _beganPath;

        public PathBuilder(Allocator allocator)
        {
            _points = new NativeList<Vector2>(allocator);
            _verbs = new NativeList<Verb>(allocator);
        }

        ~PathBuilder()
        {
            Dispose();
        }

        public PathBuilder BeginAt(Vector2 at)
        {
            ThrowIfBeganPath();
            _beganPath = true;
            _points.Add(at);
            _verbs.Add(Verb.Begin);
            return this;
        }

        public PathBuilder LineTo(Vector2 to)
        {
            ThrowIfNotBeganPath();
            _points.Add(to);
            _verbs.Add(Verb.LineTo);
            return this;
        }

        public PathBuilder QuadraticTo(Vector2 controlPoint, Vector2 to)
        {
            ThrowIfNotBeganPath();
            _points.Add(controlPoint);
            _points.Add(to);
            _verbs.Add(Verb.QuadraticTo);
            return this;
        }

        public PathBuilder CubicTo(Vector2 controlPoint1, Vector2 controlPoint2, Vector2 to)
        {
            ThrowIfNotBeganPath();
            _points.Add(controlPoint1);
            _points.Add(controlPoint2);
            _points.Add(to);
            _verbs.Add(Verb.CubicTo);
            return this;
        }

        public PathBuilder Close()
        {
            ThrowIfNotBeganPath();
            _beganPath = false;
            _verbs.Add(Verb.Close);
            return this;
        }

        public PathBuilder End()
        {
            ThrowIfNotBeganPath();
            _beganPath = false;
            _verbs.Add(Verb.End);
            return this;
        }

        public PathBuilder Clear()
        {
            _points.Clear();
            _verbs.Clear();
            return this;
        }

        public PathBuilder AddLine(Vector2 from, Vector2 to)
        {
            ThrowIfBeganPath();
            return BeginAt(from)
                .LineTo(to)
                .End();
        }

        public PathBuilder AddQuadratic(Vector2 from, Vector2 control, Vector2 to)
        {
            ThrowIfBeganPath();
            return BeginAt(from)
                .QuadraticTo(control, to)
                .End();
        }

        public PathBuilder AddCubic(Vector2 from, Vector2 control1, Vector2 control2, Vector2 to)
        {
            ThrowIfBeganPath();
            return BeginAt(from)
                .CubicTo(control1, control2, to)
                .End();
        }

        public PathBuilder AddEllipse(Vector2 center, Vector2 size)
        {
            ThrowIfBeganPath();
            float w = size.x;
            float h = size.y;
            float x = center.x - w * 0.5f;
            float y = center.y - h * 0.5f;
            // Reference: https://stackoverflow.com/a/2173084
            const float kappa = .5522848f;
            float ox = w * 0.5f * kappa; // control point offset horizontal
            float oy = h * 0.5f * kappa; // control point offset vertical
            float xe = x + w;            // x-end
            float ye = y + h;            // y-end
            float xm = x + w * 0.5f;     // x-middle
            float ym = y + h * 0.5f;     // y-middle
            return BeginAt(new Vector2(x, ym))
                .CubicTo(new Vector2(x, ym - oy), new Vector2(xm - ox, y), new Vector2(xm, y))
                .CubicTo(new Vector2(xm + ox, y), new Vector2(xe, ym - oy), new Vector2(xe, ym))
                .CubicTo(new Vector2(xe, ym + oy), new Vector2(xm + ox, ye), new Vector2(xm, ye))
                .CubicTo(new Vector2(xm - ox, ye), new Vector2(x, ym + oy), new Vector2(x, ym))
                .End();
        }

        public PathBuilder AddCircle(Vector2 center, float radius)
        {
            return AddEllipse(center, new Vector2(radius, radius));
        }

        public PathBuilder AddRect(Rect rect)
        {
            float xMin = rect.xMin;
            float xMax = rect.xMax;
            float yMin = rect.yMin;
            float yMax = rect.yMax;
            return BeginAt(new Vector2(xMin, yMin))
                .LineTo(new Vector2(xMin, yMax))
                .LineTo(new Vector2(xMax, yMax))
                .LineTo(new Vector2(xMax, yMin))
                .Close();
        }

        public void Dispose()
        {
            if (_points.IsCreated)
            {
                _points.Dispose();
            }
            if (_verbs.IsCreated)
            {
                _verbs.Dispose();
            }
        }

        protected void ThrowIfBeganPath()
        {
            if (_beganPath)
            {
                throw new InvalidOperationException("This method cannot be called while building path. Please call End or Close.");
            }
        }

        protected void ThrowIfNotBeganPath()
        {
            if (!_beganPath)
            {
                throw new InvalidOperationException("This method can only be called while building path. Please call BeginAt.");
            }
        }
    }
}
