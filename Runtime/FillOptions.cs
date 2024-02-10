using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Gilzoide.LyonTesselation
{
    [Serializable]
    public struct FillOptions
    {
        [Min(0.01f)] public float Tolerance;
        public FillRule FillRule;
        public SweepOrientation SweepOrientation;
        [MarshalAs(UnmanagedType.Bool)] public bool HandleIntersections;

        public const float DefaultTolerance = 0.1f;
        public const FillRule DefaultFillRule = FillRule.EvenOdd;
        public const SweepOrientation DefaultSweepOrientation = SweepOrientation.Vertical;
        public const bool DefaultHandleIntersections = true;

        public static FillOptions Default()
        {
            return new FillOptions
            {
                Tolerance = DefaultTolerance,
                FillRule = DefaultFillRule,
                SweepOrientation = DefaultSweepOrientation,
                HandleIntersections = DefaultHandleIntersections,
            };
        }
    }
}
