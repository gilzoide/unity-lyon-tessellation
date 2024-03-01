using System;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    [Serializable]
    public struct StrokeOptions
    {
        public LineCap StartCap;
        public LineCap EndCap;
        public LineJoin LineJoin;
        [Min(0.001f)] public float LineWidth;
        [Min(1)] public float MiterLimit;
        [Min(0.001f)] public float Tolerance;

        public const LineCap DefaultLineCap = LineCap.Butt;
        public const LineJoin DefaultLineJoin = LineJoin.Miter;
        public const float DefaultLineWidth = 1;
        public const float DefaultMiterLimit = 4;
        public const float DefaultTolerance = 0.1f;

        public static StrokeOptions Default()
        {
            return new StrokeOptions
            {
                StartCap = DefaultLineCap,
                EndCap = DefaultLineCap,
                LineJoin = DefaultLineJoin,
                LineWidth = DefaultLineWidth,
                MiterLimit = DefaultMiterLimit,
                Tolerance = DefaultTolerance,
            };
        }
    }
}
