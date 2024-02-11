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

        private NativeList<Vector2> _points;
        private NativeList<Verb> _verbs;

        public NativeArray<Vector2> Points => _points;
        public NativeArray<Verb> Verbs => _verbs;

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
            // TODO: track previous path ended
            _points.Add(at);
            _verbs.Add(Verb.Begin);
            return this;
        }

        public PathBuilder LineTo(Vector2 to)
        {
            _points.Add(to);
            _verbs.Add(Verb.LineTo);
            return this;
        }

        public PathBuilder QuadraticTo(Vector2 controlPoint, Vector2 to)
        {
            _points.Add(controlPoint);
            _points.Add(to);
            _verbs.Add(Verb.QuadraticTo);
            return this;
        }

        public PathBuilder CubicTo(Vector2 controlPoint1, Vector2 controlPoint2, Vector2 to)
        {
            _points.Add(controlPoint1);
            _points.Add(controlPoint2);
            _points.Add(to);
            _verbs.Add(Verb.CubicTo);
            return this;
        }

        public PathBuilder Close()
        {
            _verbs.Add(Verb.Close);
            return this;
        }

        public PathBuilder End()
        {
            // TODO: track path began
            _verbs.Add(Verb.End);
            return this;
        }

        public PathBuilder Clear()
        {
            _points.Clear();
            _verbs.Clear();
            return this;
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
    }
}
