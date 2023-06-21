using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurfacePlus
{

    public enum PanelTypes { Corner, Loft, Edge, Split };
    public enum SurfaceDirection { U, V };
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
            else if (c.Points.Count == 3)
            {
                surface = c.ToSurface(true);
                return true;
            }
            else
            {
                surface = null;
                return false;
            }
        }

        public static NurbsSurface Unitize(this Surface input)
        {
            NurbsSurface surface = input.ToNurbsSurface();
            surface.SetDomain(0, new Interval(0, 1));
            surface.SetDomain(1, new Interval(0, 1));

            return surface;
        }

        public static Surface ToSurface(this NurbsCurve input, bool isTriangle = false)
        {
            if (isTriangle)
            {
                return NurbsSurface.CreateFromCorners(input.Points[0].Location, input.Points[1].Location, input.Points[2].Location);
            }
            else
            {
                return NurbsSurface.CreateFromCorners(input.Points[0].Location, input.Points[1].Location, input.Points[2].Location, input.Points[3].Location);
            }
        }

        public static Surface ToSurface(this Curve input, bool isTriangle = false)
        {
            NurbsCurve c = input.ToNurbsCurve();
            return c.ToSurface(isTriangle);
        }

        public static Surface ToSurface(this Polyline input, bool isTriangle = false)
        {
            if (isTriangle)
            {
                return NurbsSurface.CreateFromCorners(input[0], input[1], input[2]);
            }
            else
            {
                return NurbsSurface.CreateFromCorners(input[0], input[1], input[2], input[3]);
            }
        }

        #region panels

        public static List<Surface> CornerStrips(this Surface input, SurfaceDirection direction, int count)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);
            List<double> t = isocurve.DivideByCount(count, false).ToList();

            for (int i = 0; i < count - 1; i++)
            {
                Surface[] split = surface.Split(x, t[i]);
                surfaces.Add(split[0].ToCornerSurface(direction));
                surface = split[1].ToNurbsSurface();
                surface.SetDomain((int)direction, new Interval(t[i], 1));
            }
            surfaces.Add(surface.ToCornerSurface(direction));

            return surfaces;
        }

        public static Surface ToCornerSurface(this Surface input, SurfaceDirection direction)
        {
            int x = (int)direction;
            Curve a = input.IsoCurve(x, input.Domain(1 - x).Min);
            Curve b = input.IsoCurve(x, input.Domain(1 - x).Max);
            return NurbsSurface.CreateFromCorners(a.PointAtStart, a.PointAtEnd, b.PointAtEnd, b.PointAtStart);
        }

        public static List<Surface> LoftStrips(this Surface input, SurfaceDirection direction, int count)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);
            List<double> t = isocurve.DivideByCount(count, false).ToList();

            for (int i = 0; i < count - 1; i++)
            {
                Surface[] split = surface.Split(x, t[i]);
                surfaces.Add(split[0].ToLoftSurface(direction));
                surface = split[1].ToNurbsSurface();
                surface.SetDomain((int)direction, new Interval(t[i], 1));
            }
            surfaces.Add(surface.ToLoftSurface(direction));

            return surfaces;
        }

        public static Surface ToLoftSurface(this Surface input, SurfaceDirection direction)
        {
            int x = (int)direction;
            Curve a = input.IsoCurve(1 - x, input.Domain(x).Min);
            Curve b = input.IsoCurve(1 - x, input.Domain(x).Max);
            return Brep.CreateFromLoft(new Curve[] { a, b }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0].Surfaces[0];
        }

        public static List<Surface> EdgeStrips(this Surface input, SurfaceDirection direction, int count)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);
            List<double> t = isocurve.DivideByCount(count, false).ToList();

            for (int i = 0; i < count - 1; i++)
            {
                Surface[] split = surface.Split(x, t[i]);
                surfaces.Add(split[0].ToEdgeSurface(direction));
                surface = split[1].ToNurbsSurface();
                surface.SetDomain((int)direction, new Interval(t[i], 1));
            }
            surfaces.Add(surface.ToEdgeSurface(direction));

            return surfaces;
        }

        public static Surface ToEdgeSurface(this Surface input, SurfaceDirection direction)
        {
            int x = (int)direction;
            List<Curve> curves = new List<Curve>();
            curves.Add(input.IsoCurve(1 - x, input.Domain(x).Min));
            curves.Add(input.IsoCurve(1 - x, input.Domain(x).Max));
            curves.Add(input.IsoCurve(x, input.Domain(1 - x).Min));
            curves.Add(input.IsoCurve(x, input.Domain(1 - x).Max));
            return Brep.CreateEdgeSurface(curves).Surfaces[0];
        }

        public static List<Surface> SplitStrips(this Surface input, SurfaceDirection direction, int count)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);
            List<double> t = isocurve.DivideByCount(count, false).ToList();

            for (int i = 0; i < count - 1; i++)
            {
                Surface[] split = surface.Split(x, t[i]);
                surfaces.Add(split[0]);
                surface = split[1].ToNurbsSurface();
                surface.SetDomain(x, new Interval(t[i], 1));
            }
            surfaces.Add(surface);

            return surfaces;
        }

        public static List<Surface> CornerQuads(this Surface input, SurfaceDirection direction, int countU, int countV)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            List<double> u = isocurve.DivideByCount(countU-1, true).ToList();

            for (int i = 1; i < countU; i++)
            {
                Curve crvA = surface.IsoCurve(1-x, u[i-1]);
                Curve crvB = surface.IsoCurve(1-x, u[i]);
                List<double> va = crvA.DivideByCount(countV, true).ToList();
                List<double> vb = crvB.DivideByCount(countV, true).ToList();
                for (int j = 1; j < countV+1; j++) surfaces.Add(NurbsSurface.CreateFromCorners(crvA.PointAt(va[j - 1]), crvA.PointAt(va[j]), crvB.PointAt(vb[j]), crvB.PointAt(vb[j - 1])));
            }

            return surfaces;
        }
        public static List<Surface> LoftQuads(this Surface input, SurfaceDirection direction, int countU, int countV)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            List<double> u = isocurve.DivideByCount(countU - 1, true).ToList();

            for (int i = 1; i < countU; i++)
            {
                Curve crvA = surface.IsoCurve(1 - x, u[i - 1]);
                Curve crvB = surface.IsoCurve(1 - x, u[i]);
                List<double> va = crvA.DivideByCount(countV, true).ToList();
                Curve[] crvsA = crvA.Split(va);
                List<double> vb = crvB.DivideByCount(countV, true).ToList();
                Curve[] crvsB = crvB.Split(vb);
                for (int j = 0; j < countV; j++)
                {
                    surfaces.Add(Brep.CreateFromLoft(new Curve[] { crvsA[j], crvsB[j] }, Point3d.Unset, Point3d.Unset, LoftType.Straight,false)[0].Surfaces[0]);
                }
            }

            return surfaces;
        }

        #endregion

        #region division

        public static List<Curve> IsoCurves(this Surface input, SurfaceDirection direction, int count)
        {
            NurbsSurface surface = input.Unitize();
            List<Curve> curves = new List<Curve>();

            int x = (int)direction;
            double t = 1.0 / count;

            for (int i = 0; i < count + 1; i++)
            {
                curves.Add(surface.IsoCurve(1 - x, t * i));
            }

            return curves;
        }

        public static List<Curve> DivideCount(this Surface input, SurfaceDirection direction, int count)
        {
            NurbsSurface surface = input.Unitize();
            List<Curve> curves = new List<Curve>();

            int x = 1 - (int)direction;
            Curve isocurve = surface.IsoCurve(1 - x, 0);
            //curves.Add(isocurve);

            List<double> t = isocurve.DivideByCount(count, true).ToList();

            for (int i = 0; i < count + 1; i++)
            {
                curves.Add(surface.IsoCurve(x, t[i]));
            }

            return curves;
        }

        public static List<Curve> DivideLength(this Surface input, SurfaceDirection direction, Point3d parameter, double length)
        {
            NurbsSurface surface = input.Unitize();
            List<Curve> curves = new List<Curve>();

            int x = 1 - (int)direction;
            Curve isocurve = surface.IsoCurve(1 - x, parameter.X);
            //curves.Add(isocurve);

            List<double> t = new List<double>();

            if (parameter.Y <= 0)
            {
                t = isocurve.DivideByLength(length, true).ToList();
            }
            else if (parameter.Y >= 0)
            {
                isocurve.Reverse();
                 t = isocurve.DivideByLength(length, true).ToList();
            }
            else
            {
            Curve[] subcurves = isocurve.Split(parameter.Y);
                double[] a = subcurves[0].DivideByLength(length,false);
                Array.Reverse(a);
                t.AddRange(a);
                t.AddRange(subcurves[0].DivideByLength(length, true));
            }

            int count = t.Count;

            for (int i = 0; i < count; i++)
            {
                curves.Add(surface.IsoCurve(x, t[i]));
            }

            return curves;
        }

        public static List<Curve> DivideSpan(this Surface input, SurfaceDirection direction, double parameter, double length, bool below)
        {
            NurbsSurface surface = input.Unitize();
            List<Curve> curves = new List<Curve>();

            int x = 1 - (int)direction;
            Curve isocurve = surface.IsoCurve(1 - x, parameter);
            //curves.Add(isocurve);

            int count = isocurve.DivideByLength(length,false).ToList().Count();
            if (below) count += 1;
            List<double> t = isocurve.DivideByCount(count, true).ToList();

            for (int i = 0; i < count; i++)
            {
                curves.Add(surface.IsoCurve(x, t[i]));
            }

            return curves;
        }

        #endregion

    }
}