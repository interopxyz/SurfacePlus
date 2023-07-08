using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurfacePlus
{

    public enum EdgeTypes { Faceted, Smooth };

    public enum PanelTypes { Corner, Loft, Iso };
    public enum BoundaryTypes { Polygon, Interpolated, Geodesic, Iso };
    public enum SurfaceDirection { U, V };
    public static class Utilities
    {
        #region numeric

        public static bool isOdd(this int input)
        {
            return (input % 2) == 0;
        }

        public static double TriangleArea(Point3d pointA, Point3d pointB, Point3d pointC)
        {
            double a = pointA.DistanceTo(pointB);
            double b = pointB.DistanceTo(pointC);
            double c = pointC.DistanceTo(pointA);
            double d = (a + b + c) / 2;

            return Math.Sqrt(d * (d - a) * (d - b) * (d - c));
        }
        public static List<double> Remap(this List<double> input, double min, double max)
        {
            List<double> output = new List<double>();
            double x = (max - min);
            foreach (double v in input) output.Add((v - min) / x);
                return output;
        }

        public static List<int> Remap(this List<double> input, double min, double max, double increment)
        {
            List<int> output = new List<int>();
            double x = (max - min);
            foreach (double v in input)
            {
                double t = (v - min) / x;
                int i = (int)Math.Ceiling(t / increment);
                double j = i * increment;
                //if ((t - j) >= 0.5) i += 1;
                output.Add(i);
            }
            return output;
        }

        public static Interval Remap(this Interval input, Interval target)
        {
            double min = target.Min;
            double max = target.Max;

            return new Interval(min + input.Min * (max - min), min + input.Max * (max - min));
        }

        public static double Remap(this Interval input, double value, Interval target)
        {
            double a = input.Max-input.Min;
            double b = target.Max - target.Min;

            return target.Min+((value-input.Min)/a)*b;
        }

        public static double Cap(this Interval input, double value)
        {
            double v = Math.Min(input.Max, value);
            return Math.Max(input.Min, v);

        }

        public static double Evaluate(this Interval input, double t)
        {
            return input.Min + t * (input.Max - input.Min);
        }

        public static double Tween(this double input, double target, double t)
        {
            return input + (target - input) * t;
        }
        public static Point2d Tween(this Point2d input, Point2d target, double t)
        {
            return input + (target - input) * t;
        }

        public static List<double> RandomList(int count, Interval domain, ref Random random)
        {
            Random random1 = random;
            List<double> outputs = new List<double>();
            double v = 0;
            for (int i = 0; i < count; i++)
            {
                v += domain.Cap(random1.NextDouble());
                outputs.Add(v);
            }

            return outputs.Remap(outputs[0], outputs[count - 1]);
        }

        public static List<int> RandomSteps(int count, Interval domain, ref Random random, int steps)
        {
            Random random1 = random;
            double step =1.0/ (count * steps);
            List<double> outputs = new List<double>();
            double v = 0;
            for (int i = 0; i < count; i++)
            {
                v += domain.Cap(random1.NextDouble());
                outputs.Add(v);
            }

            return outputs.Remap(outputs[0], outputs[count - 1],step);
        }

        #endregion

        #region to surface

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

        #endregion

        #region panels
        public static Surface ToCornerSurface(this Surface input, SurfaceDirection direction)
        {
            int x = (int)direction;
            Curve a = input.IsoCurve(x, input.Domain(1 - x).Min);
            Curve b = input.IsoCurve(x, input.Domain(1 - x).Max);
            return NurbsSurface.CreateFromCorners(a.PointAtStart, a.PointAtEnd, b.PointAtEnd, b.PointAtStart);
        }

        public static Surface ToLoftSurface(this Surface input, SurfaceDirection direction)
        {
            int x = (int)direction;
            Curve a = input.IsoCurve(1 - x, input.Domain(x).Min);
            Curve b = input.IsoCurve(1 - x, input.Domain(x).Max);
            return Brep.CreateFromLoft(new Curve[] { a, b }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0].Surfaces[0];
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

        #region strips

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

        public static List<Surface> SplitStrips(this Surface input, SurfaceDirection direction, int count, double shift = 0)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            List<double> t = isocurve.DivideByCount(count, true,shift).ToList();

            for (int i = 1; i < t.Count-1; i++)
            {
                Surface[] split = surface.Split(x, t[i]);
                surfaces.Add(split[0]);
                surface = split[1].ToNurbsSurface();
                surface.SetDomain(x, new Interval(t[i], 1));
            }
            surfaces.Add(surface);

            return surfaces;
        }

        public static List<Surface> SplitStrips(this Surface input, SurfaceDirection direction, int count, List<double> parameters)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            List<double> t = parameters;

            for (int i = 1; i < t.Count - 1; i++)
            {
                Surface[] split = surface.Split(x, t[i]);
                surfaces.Add(split[0]);
                surface = split[1].ToNurbsSurface();
                surface.SetDomain(x, new Interval(t[i], 1));
            }
            surfaces.Add(surface);

            return surfaces;
        }

        #endregion

        #region quads

        public static List<double> DivideByCount(this Curve input, int count, bool ends, double t)
        {
            List<double> outputs = new List<double>();
            List<double> values = input.DivideByCount(count, ends).ToList();
            if ((count > 1)&(t>0)&(t<1))
            {
                outputs.Add(values[0]);
                for (int i = 0; i < count; i++) outputs.Add(values[i] + (values[i + 1] - values[i]) * t);
                outputs.Add(values[values.Count()-1]);
                return outputs;
            }
        else
            {
            return values;
        }
        }

        public static List<double> Repeat(this List<double> input, int count)
        {
            int c = input.Count;
            List<double> output = new List<double>();
            for(int i = 0; i < count; i++)
            {
                int j = (i % c);
                output.Add(input[j]);
            }
            return output;
        }

        public static List<Surface> CornerQuads(this Surface input, SurfaceDirection direction, int countU, int countV)
        {
            return input.CornerQuads(direction, countU, countV, new List<double> { 0 });
        }

        public static List<Surface> CornerQuads(this Surface input, SurfaceDirection direction, int countU, int countV, List<double> shift)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            double v = 1.0 / countV;
            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);
            
            List<double> u = isocurve.DivideByCount(countU, true).ToList();

            List<double> t = shift.Repeat(countU);
            for (int i = 1; i < countU+1; i++)
            {
                Curve crvA = surface.IsoCurve(1-x, u[i-1]);
                Curve crvB = surface.IsoCurve(1-x, u[i]);
                List<double> va = crvA.DivideByCount(countV, true,t[i-1]);
                List<double> vb = crvB.DivideByCount(countV, true, t[i - 1]);
                for (int j = 1; j < va.Count; j++) surfaces.Add(NurbsSurface.CreateFromCorners(crvA.PointAt(va[j - 1]), crvA.PointAt(va[j]), crvB.PointAt(vb[j]), crvB.PointAt(vb[j - 1])));
            }

            return surfaces;
        }

        public static List<Surface> LoftQuads(this Surface input, SurfaceDirection direction, int countU, int countV)
        {
            return input.LoftQuads(direction, countU, countV, new List<double> { 0 });
        }

        public static List<Surface> LoftQuads(this Surface input, SurfaceDirection direction, int countU, int countV, List<double> shift)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            List<double> u = isocurve.DivideByCount(countU, true).ToList();

            List<double> t = shift.Repeat(countU);
            for (int i = 1; i < countU+1; i++)
            {
                Curve crvA = surface.IsoCurve(1 - x, u[i - 1]);
                Curve crvB = surface.IsoCurve(1 - x, u[i]);
                List<double> va = crvA.DivideByCount(countV, true,t[i-1]).ToList();
                Curve[] crvsA = crvA.Split(va);
                List<double> vb = crvB.DivideByCount(countV, true, t[i - 1]).ToList();
                Curve[] crvsB = crvB.Split(vb);
                for (int j = 0; j < va.Count()-1; j++)
                {
                    surfaces.Add(Brep.CreateFromLoft(new Curve[] { crvsA[j], crvsB[j] }, Point3d.Unset, Point3d.Unset, LoftType.Straight,false)[0].Surfaces[0]);
                }
            }

            return surfaces;
        }

        public static List<Surface> SplitQuads(this Surface input, SurfaceDirection direction, int countU, int countV)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            List<Surface> srfs = surface.SplitStrips((SurfaceDirection)direction, countU);
            for (int i = 0; i < countU; i++) surfaces.AddRange(srfs[i].SplitStrips(1-direction, countV));

            return surfaces;
        }

        public static List<Surface> SplitQuads(this Surface input, SurfaceDirection direction, int countU, int countV, List<double> shift)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

        List<double> t = shift.Repeat(countU);
        List<Surface> srfs = surface.SplitStrips((SurfaceDirection)direction, countU);
            for (int i = 0; i < countU; i++) surfaces.AddRange(srfs[i].SplitStrips(1 - direction, countV,t[i]));

            return surfaces;
        }

        #endregion

        #region wireframes

            #endregion

            #region random quads

            public static List<Surface> CornerQuads(this Surface input, SurfaceDirection direction, int countU, int countV, int seed, Interval domain)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            List<double> u = isocurve.DivideByCount(countU, true).ToList();
            Random rnd = new Random(seed);

            for (int i = 1; i < countU + 1; i++)
            {
                Curve crvA = surface.IsoCurve(1 - x, u[i - 1]);
                Curve crvB = surface.IsoCurve(1 - x, u[i]);
                List<double> v = RandomList(countV+1, domain, ref rnd);
                for (int j = 1; j < v.Count; j++) surfaces.Add(NurbsSurface.CreateFromCorners(crvA.PointAt(v[j - 1]), crvA.PointAt(v[j]), crvB.PointAt(v[j]), crvB.PointAt(v[j - 1])));
            }

            return surfaces;
        }

        public static List<Surface> LoftQuads(this Surface input, SurfaceDirection direction, int countU, int countV, int seed, Interval domain)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();

            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            List<double> u = isocurve.DivideByCount(countU, true).ToList();
            Random rnd = new Random(seed);

            for (int i = 1; i < countU + 1; i++)
            {
                Curve crvA = surface.IsoCurve(1 - x, u[i - 1]);
                Curve crvB = surface.IsoCurve(1 - x, u[i]);
                List<double> v = RandomList(countV+1, domain, ref rnd);
                Curve[] crvsA = crvA.Split(v);
                Curve[] crvsB = crvB.Split(v);
                for (int j = 0; j < v.Count() - 1; j++)
                {
                    surfaces.Add(Brep.CreateFromLoft(new Curve[] { crvsA[j], crvsB[j] }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0].Surfaces[0]);
                }
            }

            return surfaces;
        }

        public static List<Surface> SplitQuads(this Surface input, SurfaceDirection direction, int countU, int countV, int seed, Interval domain)
        {
            NurbsSurface surface = input.Unitize();
            List<Surface> surfaces = new List<Surface>();
            Random rnd = new Random(seed);

            List<Surface> srfs = surface.SplitStrips((SurfaceDirection)direction, countU);
            for (int i = 0; i < countU; i++)
            {
                List<double> v = RandomList(countV+1, domain, ref rnd);
                surfaces.AddRange(srfs[i].SplitStrips(1 - direction, countV, v));
            }

            return surfaces;
        }

        #endregion

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

        #region tiles

        public static Polyline Shift(this Polyline input, int steps)
        {
            Polyline output = new Polyline(input);
            for(int i = 0; i < steps; i++)
            {
                output.RemoveAt(0);
                output.Add(output[0]);
            }
            return output;
        }

        public static Polyline ControlPolygon(this Curve input)
        {
            NurbsCurve crv = input.ToNurbsCurve();
            return crv.Points.ControlPolygon();
        }

        public static List<Surface> StellateTriangles(this Curve input, out bool status)
        {
            status = false;
            List<Surface> surfaces = new List<Surface>();

            Polyline p = input.ControlPolygon();
            if (p.Count < 3) return surfaces;

            Point3d c = p.CenterPoint();

            for (int i = 0; i < p.Count - 1; i++)
            {
                surfaces.Add(NurbsSurface.CreateFromCorners(p[i + 1], p[i], c));
            }

            status = true;
            return surfaces;
        }

        public static List<Surface> StellateQuads(this Curve input, out bool status)
        {
            status = false;
            List<Surface> surfaces = new List<Surface>();

            Polyline p = input.ControlPolygon();
            if (p.Count < 4)return surfaces;
            if (p.Count.isOdd()) return surfaces;

            Point3d c = p.CenterPoint();

            for (int i = 0; i < p.Count - 2; i+=2)
            {
                surfaces.Add(NurbsSurface.CreateFromCorners(p[i + 2], p[i + 1], p[i], c));
            }

            status = true;
            return surfaces;
        }

        public static List<Surface> StellateDiamonds(this Curve input, out bool status)
        {
            status = false;
            List<Surface> surfaces = new List<Surface>();

            Polyline p = input.ControlPolygon();
            if (p.Count < 3) return surfaces;

            Point3d c = p.CenterPoint();

            for (int i = 0; i < p.Count - 2; i++)
            {
                surfaces.Add(NurbsSurface.CreateFromCorners((p[i + 1] + p[i + 2]) / 2.0, p[i + 1], (p[i + 0] + p[i + 1]) / 2.0, c));
            }
            if (p.IsClosed) surfaces.Add(NurbsSurface.CreateFromCorners((p[0] + p[1]) / 2.0, p[0], (p[p.Count - 1] + p[p.Count - 2]) / 2.0, c));

            status = true;
            return surfaces;
        }

        public static List<Surface> Fan(this Curve input, out bool status)
        {
            status = false;
            List<Surface> surfaces = new List<Surface>();

            Polyline p = input.ControlPolygon();
            if (!p.IsClosed) return surfaces;
            if (p.Count < 4) return surfaces;

            for (int i = 1; i < p.Count - 1; i ++)
            {
                surfaces.Add(NurbsSurface.CreateFromCorners(p[0], p[i + 1], p[i]));
            }

            status = true;
            return surfaces;
        }

        public static List<Surface> StitchQuads(this Curve input, out bool status)
        {
            status = false;
            List<Surface> surfaces = new List<Surface>();

            Polyline p = input.ControlPolygon();
            int c = p.Count;
            if (!p.IsClosed) return surfaces;
            if (c < 4) return surfaces;
            if (c.isOdd()) return surfaces;

            p = p.Shift(1);

            int h = (int)Math.Floor(c / 2.0);
            for (int i = 0; i < h; i ++)
            {
                surfaces.Add(NurbsSurface.CreateFromCorners(p[i], p[c - 2 - i], p[i + 1], p[c - 3 - i]));
            }

            status = true;
            return surfaces;
        }

        public static List<Surface> StitchTriangles(this Curve input, out bool status)
        {
            status = false;
            List<Surface> surfaces = new List<Surface>();

            Polyline p = input.ControlPolygon();
            int c = p.Count;
            if (!p.IsClosed) return surfaces;
            if (c < 4) return surfaces;

            int h = (int)Math.Floor(c / 2.0);
            surfaces.Add(NurbsSurface.CreateFromCorners(p[0], p[c - 2], p[1]));
            for (int i = 1; i < h-1; i ++)
            {
                surfaces.Add(NurbsSurface.CreateFromCorners(p[c - 1 - i], p[i + 1], p[i]));
                surfaces.Add(NurbsSurface.CreateFromCorners(p[i + 1], p[c - 1 - i], p[c - 2 - i]));
            }
            if(!c.isOdd()) surfaces.Add(NurbsSurface.CreateFromCorners(p[h-1], p[h + 1], p[h]));

            status = true;
            return surfaces;
        }

        #endregion
    }
}