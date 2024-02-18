using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    public struct NativePathBuilder : IDisposable, INativeDisposable
    {
        public readonly NativeArray<Vector2> Points => _points;
        public readonly NativeArray<PathVerb> Verbs => _verbs;

        private NativeList<Vector2> _points;
        private NativeList<PathVerb> _verbs;

        public NativePathBuilder(Allocator allocator)
        {
            _points = new NativeList<Vector2>(allocator);
            _verbs = new NativeList<PathVerb>(allocator);
        }

        public void BeginAt(Vector2 at)
        {
            ThrowIfBeganPath();
            _points.Add(at);
            _verbs.Add(PathVerb.Begin);
        }

        public void LineTo(Vector2 to)
        {
            ThrowIfNotBeganPath();
            _points.Add(to);
            _verbs.Add(PathVerb.LineTo);
        }

        public void QuadraticBezierTo(Vector2 controlPoint, Vector2 to)
        {
            ThrowIfNotBeganPath();
            _points.Add(controlPoint);
            _points.Add(to);
            _verbs.Add(PathVerb.QuadraticBezierTo);
        }

        public void CubicBezierTo(Vector2 controlPoint1, Vector2 controlPoint2, Vector2 to)
        {
            ThrowIfNotBeganPath();
            _points.Add(controlPoint1);
            _points.Add(controlPoint2);
            _points.Add(to);
            _verbs.Add(PathVerb.CubicBezierTo);
        }

        public void Close()
        {
            ThrowIfNotBeganPath();
            _verbs.Add(PathVerb.Close);
        }

        public void End()
        {
            ThrowIfNotBeganPath();
            _verbs.Add(PathVerb.End);
        }

        public void Clear()
        {
            _points.Clear();
            _verbs.Clear();
        }

        public void AddLine(Vector2 from, Vector2 to)
        {
            ThrowIfBeganPath();
            BeginAt(from);
            LineTo(to);
            End();
        }

        public void AddQuadraticBezier(Vector2 from, Vector2 control, Vector2 to)
        {
            ThrowIfBeganPath();
            BeginAt(from);
            QuadraticBezierTo(control, to);
            End();
        }

        public void AddCubicBezier(Vector2 from, Vector2 control1, Vector2 control2, Vector2 to)
        {
            ThrowIfBeganPath();
            BeginAt(from);
            CubicBezierTo(control1, control2, to);
            End();
        }

        public void AddEllipse(Vector2 center, Vector2 size)
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
            BeginAt(new Vector2(x, ym));
            CubicBezierTo(new Vector2(x, ym - oy), new Vector2(xm - ox, y), new Vector2(xm, y));
            CubicBezierTo(new Vector2(xm + ox, y), new Vector2(xe, ym - oy), new Vector2(xe, ym));
            CubicBezierTo(new Vector2(xe, ym + oy), new Vector2(xm + ox, ye), new Vector2(xm, ye));
            CubicBezierTo(new Vector2(xm - ox, ye), new Vector2(x, ym + oy), new Vector2(x, ym));
            End();
        }

        public void AddCircle(Vector2 center, float radius)
        {
            AddEllipse(center, new Vector2(radius, radius));
        }

        public void AddRect(Rect rect)
        {
            float xMin = rect.xMin;
            float xMax = rect.xMax;
            float yMin = rect.yMin;
            float yMax = rect.yMax;
            BeginAt(new Vector2(xMin, yMin));
            LineTo(new Vector2(xMin, yMax));
            LineTo(new Vector2(xMax, yMax));
            LineTo(new Vector2(xMax, yMin));
            Close();
        }

        public void AddRoundedRect(Rect rect, float cornerRadius)
        {
            if (cornerRadius > 0)
            {
                float xMin = rect.xMin;
                float xMax = rect.xMax;
                float yMin = rect.yMin;
                float yMax = rect.yMax;
                // Reference: https://pomax.github.io/bezierinfo/#circles_cubic
                const float factor = 0.551785f;
                float controlOffset = cornerRadius * factor;
                BeginAt(new Vector2(xMin, yMin + cornerRadius));
                CubicBezierTo(
                    new Vector2(xMin, yMin + cornerRadius - controlOffset),
                    new Vector2(xMin + cornerRadius - controlOffset, yMin),
                    new Vector2(xMin + cornerRadius, yMin)
                );
                LineTo(new Vector2(xMax - cornerRadius, yMin));
                CubicBezierTo(
                    new Vector2(xMax - cornerRadius + controlOffset, yMin),
                    new Vector2(xMax, yMin + cornerRadius - controlOffset),
                    new Vector2(xMax, yMin + cornerRadius)
                );
                LineTo(new Vector2(xMax, yMax - cornerRadius));
                CubicBezierTo(
                    new Vector2(xMax, yMax - cornerRadius + controlOffset),
                    new Vector2(xMax - cornerRadius + controlOffset, yMax),
                    new Vector2(xMax - cornerRadius, yMax)
                );
                LineTo(new Vector2(xMin + cornerRadius, yMax));
                CubicBezierTo(
                    new Vector2(xMin + cornerRadius - controlOffset, yMax),
                    new Vector2(xMin, yMax - cornerRadius + controlOffset),
                    new Vector2(xMin, yMax - cornerRadius)
                );
                Close();
            }
            else
            {
                AddRect(rect);
            }
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

        public readonly JobHandle Dispose(JobHandle inputDeps)
        {
            return this.ScheduleDisposeJob(inputDeps);
        }

        internal readonly void ThrowIfBeganPath()
        {
            for (int i = _verbs.Length - 1; i >= 0; i--)
            {
                switch (_verbs[i])
                {
                    case PathVerb.Close:
                    case PathVerb.End:
                        return;

                    case PathVerb.Begin:
                        throw new InvalidOperationException("This method cannot be called while building path. Please call End or Close.");
                }
            }
        }

        internal readonly void ThrowIfNotBeganPath()
        {
            for (int i = _verbs.Length - 1; i >= 0; i--)
            {
                switch (_verbs[i])
                {
                    case PathVerb.Begin:
                        return;

                    case PathVerb.Close:
                    case PathVerb.End:
                        throw new InvalidOperationException("This method can only be called while building path. Please call BeginAt.");
                }
            }
        }
    }
}
