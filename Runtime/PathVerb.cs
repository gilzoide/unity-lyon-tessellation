namespace Gilzoide.LyonTesselation
{
    public enum PathVerb : byte
    {
        Begin = 0,
        LineTo = 1,
        QuadraticBezierTo = 2,
        CubicBezierTo = 3,
        Close = 4,
        End = 5,
    }
}
