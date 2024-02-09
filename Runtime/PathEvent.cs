using System.Runtime.InteropServices;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PathEvent
    {
        public PathEventType Type;
        public int Close;
        public Vector2 From;
        public Vector2 To;
        public Vector2 ControlPoint1;
        public Vector2 ControlPoint2;

        public static PathEvent Begin(Vector2 at)
        {
            return new PathEvent
            {
                Type = PathEventType.Begin,
                From = at,
            };
        }

        public static PathEvent Line(Vector2 from, Vector2 to)
        {
            return new PathEvent
            {
                Type = PathEventType.Line,
                From = from,
                To = to,
            };
        }

        public static PathEvent Quadratic(Vector2 from, Vector2 controlPoint1, Vector2 to)
        {
            return new PathEvent
            {
                Type = PathEventType.Quadratic,
                From = from,
                ControlPoint1 = controlPoint1,
                To = to,
            };
        }

        public static PathEvent Cubic(Vector2 from, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 to)
        {
            return new PathEvent
            {
                Type = PathEventType.Cubic,
                From = from,
                ControlPoint1 = controlPoint1,
                ControlPoint2 = controlPoint2,
                To = to,
            };
        }

        public static PathEvent End(Vector2 last, Vector2 first, bool close)
        {
            return new PathEvent
            {
                Type = PathEventType.End,
                From = last,
                To = first,
                Close = close ? 1 : 0,
            };
        }
    }
}
