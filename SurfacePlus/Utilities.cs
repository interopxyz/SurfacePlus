using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus
{
    public static class Utilities
    {
        public static Interval Remap(this Interval input, Interval target)
        {
            double min = target.Min;
            double max = target.Max;

            return new Interval(min + input.Min * (max - min), min + input.Max * (max - min));
        }
        public static double Evaluate(this Interval input, double t)
        {
            return input.Min + t * (input.Max - input.Min);
        }

        public static bool TryGetSurface(this Curve input, out Surface surface)
        {
            NurbsCurve c = input.ToNurbsCurve();
            if (c.Points.Count == 4)
            {
                surface = c.ToSurface();
                return true;
            }
            else
            {
                surface = null;
                return false;
            }
        }

            public static Surface ToSurface(this NurbsCurve input)
            {
                Surface surface = NurbsSurface.CreateFromCorners(input.Points[0].Location, input.Points[1].Location, input.Points[2].Location, input.Points[3].Location);
                return surface;
            }

            public static Surface ToSurface(this Curve input)
        {
            NurbsCurve c = input.ToNurbsCurve();
            return c.ToSurface();
        }

        public static Surface ToSurface(this Polyline input)
        {
            Surface surface = NurbsSurface.CreateFromCorners(input[0], input[1], input[2], input[3]);
            return surface;
        }
    }
}