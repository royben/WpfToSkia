using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfToSkia.ExtensionsMethods
{
    public static class GeometryExtensions
    {
        public static SKPath ToSKPath(this PathGeometry path)
        {
            SKPath sk = new SKPath();

            foreach (var figure in path.Figures)
            {
                sk.AddPath(figure.ToSKPath(), SKPathAddMode.Append);
            }

            return sk;
        }

        private static SKPath ToSKPath(this PathFigure figure)
        {
            SKPath sk = new SKPath();

            sk.MoveTo(figure.StartPoint.ToSKPoint());

            foreach (var segment in figure.Segments)
            {
                sk.AddSegment(segment);
            }

            //sk.LineTo(figure.StartPoint.ToSKPoint());

            if (figure.IsClosed)
            {
                sk.Close();
            }

            return sk;
        }

        private static void AddSegment(this SKPath path, PathSegment segment)
        {
            if (segment is LineSegment)
            {
                var s = segment as LineSegment;
                path.LineTo(s.Point.ToSKPoint());
            }
            else if (segment is PolyLineSegment)
            {
                var s = segment as PolyLineSegment;
                path.AddPoly(s.Points.Select(x => x.ToSKPoint()).Reverse().ToArray(), false);
            }
            else if (segment is ArcSegment)
            {
                var s = segment as ArcSegment;
                path.AddArc(new Rect(s.Point, s.Size).ToSKRect(), 180 + -s.RotationAngle.ToFloat(), -180);
            }
            else if (segment is BezierSegment)
            {
                var s = segment as BezierSegment;
                path.CubicTo(s.Point1.ToSKPoint(), s.Point2.ToSKPoint(), s.Point3.ToSKPoint());
            }
            else if (segment is PolyBezierSegment)
            {
                var s = segment as PolyBezierSegment;

                if (s.Points.Count % 3 != 0)
                {
                    throw new ArgumentException("Number of PolyBezierSegment points must be divisable by 3.");
                }

                for (int i = 0; i < s.Points.Count; i += 3)
                {
                    var p1 = s.Points[i];
                    var p2 = s.Points[i + 1];
                    var p3 = s.Points[i + 2];

                    path.CubicTo(p1.ToSKPoint(), p2.ToSKPoint(), p3.ToSKPoint());
                }
            }
            else if (segment is PolyQuadraticBezierSegment)
            {
                var s = segment as PolyQuadraticBezierSegment;

                for (int i = 0; i < s.Points.Count; i += 2)
                {
                    var p1 = s.Points[i];
                    var p2 = s.Points[i + 1];

                    path.CubicTo(p1.ToSKPoint(), p2.ToSKPoint(), p2.ToSKPoint());
                }
            }
        }
    }
}
