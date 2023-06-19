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
    }
}