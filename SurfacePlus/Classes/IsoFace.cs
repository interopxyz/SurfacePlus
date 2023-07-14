using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfacePlus
{
    public class IsoFace
    {
        #region members

        public enum HexTriTypes { Center, Side}
        public HexTriTypes HexTriType = HexTriTypes.Center;

        public List<Point2d> Coordinates = new List<Point2d>();

        #endregion

        #region constructors

        public IsoFace()
        {

        }

        public IsoFace(IsoFace isoFace)
        {
            foreach (Point2d pt in isoFace.Coordinates) this.Coordinates.Add(new Point2d(pt));
            this.HexTriType = isoFace.HexTriType;
        }

        public IsoFace(List<Point2d> coordinates)
        {
            foreach (Point2d pt in coordinates) this.Coordinates.Add(new Point2d(pt));
        }

        public IsoFace(Point2d a, Point2d b, Point2d c)
        {
            this.Coordinates.Add(a);
            this.Coordinates.Add(b);
            this.Coordinates.Add(c);
        }

        public IsoFace(Point2d a, Point2d b, Point2d c, Point2d d)
        {
            this.Coordinates.Add(a);
            this.Coordinates.Add(b);
            this.Coordinates.Add(c);
            this.Coordinates.Add(d);
        }

        public IsoFace(Point2d a, Point2d b, Point2d c, Point2d d, Point2d e, Point2d f)
        {
            this.Coordinates.Add(a);
            this.Coordinates.Add(b);
            this.Coordinates.Add(c);
            this.Coordinates.Add(d);
            this.Coordinates.Add(e);
            this.Coordinates.Add(f);
        }

        #endregion

        #region properties

        public virtual bool IsHex
        {
            get { return this.Coordinates.Count == 6; }
        }

        public virtual bool IsQuad
        {
            get { return this.Coordinates.Count == 4; }
        }
        public virtual bool IsTri
        {
            get { return this.Coordinates.Count == 3; }
        }

        #endregion

        #region methods

        #region render

        public Curve RenderUVPolyline()
        {
            Polyline output = new Polyline();
            foreach (Point2d uv in this.Coordinates) output.Add(uv.X, uv.Y, 0);
            return output.ToNurbsCurve();
        }

        public Curve RenderPolygon(Surface surface)
        {
            Polyline output = new Polyline();
            foreach(Point2d uv in this.Coordinates)
            {
                output.Add(surface.PointAt(uv.X,uv.Y));
            }
            output.Add(output[0]);

            return output.ToNurbsCurve();
        }

        public Curve RenderIsoBoundary(Surface surface, SurfaceDirection direction)
        {
            if (this.IsQuad)
            {
                NurbsSurface srf = null;
                
                switch (direction)
                {
                    case SurfaceDirection.U:
                        srf = surface.Trim(new Interval(this.Coordinates[0].X, this.Coordinates[2].X), new Interval(this.Coordinates[0].Y, this.Coordinates[1].Y)).ToNurbsSurface();
                        break;
                    case SurfaceDirection.V:
                        srf = surface.Trim(new Interval(this.Coordinates[0].X, this.Coordinates[1].X), new Interval(this.Coordinates[0].Y, this.Coordinates[2].Y)).ToNurbsSurface();
                        break;
                }
                
                   Curve[] curves = srf.ToBrep().DuplicateNakedEdgeCurves(true, false);
                return NurbsCurve.JoinCurves(curves)[0];

            }
            return null;
        }

        public Curve RenderInterpolatedBoundary(Surface surface)
        {
            int c = this.Coordinates.Count;
            List<Curve> segments = new List<Curve>();
            for(int i = 0;i<c;i++)
            {
                int j = (i + 1) % c;
                segments.Add(surface.InterpolatedCurveOnSurfaceUV(new Point2d[] { this.Coordinates[i], this.Coordinates[j] }, 0.1));
            }

            return NurbsCurve.JoinCurves(segments)[0];
        }

        public Curve RenderGeodesicBoundary(Surface surface)
        {
            int c = this.Coordinates.Count;
            List<Curve> segments = new List<Curve>();
            for (int i = 0; i < c; i++)
            {
                int j = (i + 1) % c;
                segments.Add(surface.ShortPath(this.Coordinates[i], this.Coordinates[j], 1));
            }

            return NurbsCurve.JoinCurves(segments)[0];
        }

        public NurbsSurface RenderFacet(Surface surface, SurfaceDirection direction)
        {
            if (this.IsQuad)
            {
                switch (direction)
                {
                    default:
                        return NurbsSurface.CreateFromCorners(surface.PointAt(Coordinates[1].X, Coordinates[1].Y), surface.PointAt(Coordinates[0].X, Coordinates[0].Y), surface.PointAt(Coordinates[3].X, Coordinates[3].Y), surface.PointAt(Coordinates[2].X, Coordinates[2].Y));
                    case SurfaceDirection.V:
                        return NurbsSurface.CreateFromCorners(surface.PointAt(Coordinates[0].X, Coordinates[0].Y), surface.PointAt(Coordinates[1].X, Coordinates[1].Y), surface.PointAt(Coordinates[2].X, Coordinates[2].Y), surface.PointAt(Coordinates[3].X, Coordinates[3].Y));
                }
            }
            if (this.IsTri)
            {
                switch (direction)
                {
                    default:
                        return NurbsSurface.CreateFromCorners(surface.PointAt(Coordinates[1].X, Coordinates[1].Y), surface.PointAt(Coordinates[0].X, Coordinates[0].Y), surface.PointAt(Coordinates[2].X, Coordinates[2].Y));
                    case SurfaceDirection.V:
                        return NurbsSurface.CreateFromCorners(surface.PointAt(Coordinates[0].X, Coordinates[0].Y), surface.PointAt(Coordinates[1].X, Coordinates[1].Y), surface.PointAt(Coordinates[2].X, Coordinates[2].Y));
                }
            }
            return null;
        }

        public NurbsSurface RenderLoftedSurface(Surface surface, SurfaceDirection direction)
        {
            if (this.IsQuad)
            {
                int d = (int)direction;

                double[] t0 = new double[] { Coordinates[0].X, Coordinates[0].Y };
                double[] t1 = new double[] { Coordinates[1].X, Coordinates[1].Y };
                double[] t2 = new double[] { Coordinates[2].X, Coordinates[2].Y };
                double[] t3 = new double[] { Coordinates[3].X, Coordinates[3].Y };

                NurbsCurve a = surface.IsoCurve(1-d, t0[d]).ToNurbsCurve();
                NurbsCurve b = surface.IsoCurve(1-d, t2[d]).ToNurbsCurve();
                Curve[] c = new Curve[2];

                double[] ta = new double[] { Math.Min(t0[1-d], t1[1 - d]), Math.Max(t1[1 - d], t1[1 - d]) };
                double[] tb = new double[] { Math.Min(t2[1 - d], t3[1 - d]), Math.Max(t2[1 - d], t3[1 - d]) };


                Curve[] sa = a.Split(ta);
                Curve[] sb = b.Split(tb);

                if ((ta[1] - ta[0]) == 1)
                {
                    c[0] = a;
                }
                else if (ta[0] == 0)
                {
                    c[0] = sa[0];
                }
                else
                {
                    c[0] = sa[1];
                }

                if ((tb[1] - tb[0]) == 1)
                {
                    c[1] = b;
                }
                else if (tb[0] == 0)
                {
                    c[1] = sb[0];
                }
                else
                {
                    c[1] = sb[1];
                }

                return Brep.CreateFromLoft(c, Point3d.Unset, Point3d.Unset, LoftType.Normal, false)[0].Surfaces[0].ToNurbsSurface();
            }

            return null;
        }

        public Surface RenderIsoSurface(Surface surface, SurfaceDirection direction)
        {
            if (this.IsQuad)
            {
                NurbsSurface srf = null;

                switch (direction)
                {
                    case SurfaceDirection.U:
                        srf = surface.Trim(new Interval(this.Coordinates[0].X, this.Coordinates[2].X), new Interval(this.Coordinates[0].Y, this.Coordinates[1].Y)).ToNurbsSurface();
                        break;
                    case SurfaceDirection.V:
                        srf = surface.Trim(new Interval(this.Coordinates[0].X, this.Coordinates[1].X), new Interval(this.Coordinates[0].Y, this.Coordinates[2].Y)).ToNurbsSurface();
                        break;
                }

                return srf;

            }
            return null;
        }

        #endregion

        public List<Point3d> Evaluate (Surface surface)
        {
            List<Point3d> outputs = new List<Point3d>();
            foreach (Point2d uv in this.Coordinates) outputs.Add(surface.PointAt(uv.X,uv.Y));
            return outputs;
        }

        public List<IsoFace> HexToSplitQuads(int shift  = 0)
        {
            List<IsoFace> outputs = new List<IsoFace>();
            if (this.IsHex)
            {
                int s = shift % 3;
                outputs.Add(new IsoFace(this.Coordinates[s], this.Coordinates[s + 1], this.Coordinates[s + 2], this.Coordinates[(s + 5) % 6]));
                outputs.Add(new IsoFace(this.Coordinates[(s + 5) % 6], this.Coordinates[s + 2], this.Coordinates[(s + 3) % 6], this.Coordinates[(s + 4) % 6]));
            }
            return outputs;
        }

        public List<IsoFace> HexToRadialQuads(int shift = 0)
        {
            List<IsoFace> outputs = new List<IsoFace>();
            if (this.IsHex)
            {
                Point2d center = (this.Coordinates[2] + this.Coordinates[5]) / 2.0;
                int s = shift % 3;
                outputs.Add(new IsoFace(this.Coordinates[s], this.Coordinates[s + 1], this.Coordinates[s + 2], center));
                outputs.Add(new IsoFace(this.Coordinates[s + 2], this.Coordinates[(s + 3) % 6], this.Coordinates[(s + 4) % 6], center));
                outputs.Add(new IsoFace(this.Coordinates[(s + 4) % 6], this.Coordinates[(s + 5) % 6], this.Coordinates[s], center));
            }
            return outputs;
        }

        public List<IsoFace> HexToTris()
        {
            List<IsoFace> outputs = new List<IsoFace>();
            if (this.IsHex)
            {
                switch (this.HexTriType)
                {
                    default:
                        break;
                        Point2d center = (this.Coordinates[2] + this.Coordinates[5]) / 2.0;
                        outputs.Add(new IsoFace(this.Coordinates[0], this.Coordinates[1], center));
                        outputs.Add(new IsoFace(this.Coordinates[1], this.Coordinates[2], center));
                        outputs.Add(new IsoFace(this.Coordinates[2], this.Coordinates[3], center));
                        outputs.Add(new IsoFace(this.Coordinates[3], this.Coordinates[4], center));
                        outputs.Add(new IsoFace(this.Coordinates[4], this.Coordinates[5], center));
                        outputs.Add(new IsoFace(this.Coordinates[5], this.Coordinates[0], center));
                    case HexTriTypes.Side:
                        outputs.Add(new IsoFace(this.Coordinates[0], this.Coordinates[1], this.Coordinates[2]));
                        outputs.Add(new IsoFace(this.Coordinates[3], this.Coordinates[4], this.Coordinates[2]));
                        outputs.Add(new IsoFace(this.Coordinates[4], this.Coordinates[5], this.Coordinates[2]));
                        outputs.Add(new IsoFace(this.Coordinates[5], this.Coordinates[0], this.Coordinates[2]));
                        break;
                }
            }
            return outputs;
        }

        public List<IsoFace> QuadToDense()
        {
            List<IsoFace> outputs = new List<IsoFace>();
            if (this.IsQuad)
            {
                Point2d uv = (this.Coordinates[0] + this.Coordinates[1] + this.Coordinates[2] + this.Coordinates[3]) / 4.0;
                outputs.Add(new IsoFace(this.Coordinates[0], this.Coordinates[1], uv));
                outputs.Add(new IsoFace(this.Coordinates[1], this.Coordinates[2], uv));
                outputs.Add(new IsoFace(this.Coordinates[2], this.Coordinates[3], uv));
                outputs.Add(new IsoFace(this.Coordinates[3], this.Coordinates[0], uv));
            }
            return outputs;
        }

        public List<IsoFace> QuadToTri(bool flip)
        {
            List<IsoFace> outputs = new List<IsoFace>();
            if (this.IsQuad)
            {
                if(flip)
                {
                    outputs.Add(new IsoFace(this.Coordinates[2], this.Coordinates[3], this.Coordinates[0]));
                    outputs.Add(new IsoFace(this.Coordinates[0], this.Coordinates[1], this.Coordinates[2]));
                }
                else
                {
                    outputs.Add(new IsoFace(this.Coordinates[0], this.Coordinates[1], this.Coordinates[3]));
                    outputs.Add(new IsoFace(this.Coordinates[2], this.Coordinates[3], this.Coordinates[1]));
                }
            }
            return outputs;
        }

        public List<IsoFace> QuadToTriByLength(Surface surface, bool flip)
        {
            List<IsoFace> outputs = new List<IsoFace>();
            if (this.IsQuad)
            {
            List<Point3d> points = this.Evaluate(surface);
                double a = points[0].DistanceTo(points[2]);
                double b = points[1].DistanceTo(points[3]);
                outputs.AddRange(this.QuadToTri((a > b) == flip));
            }
            return outputs;
        }

        public List<IsoFace> QuadToTriByArea(Surface surface, bool flip)
        {
            List<IsoFace> outputs = new List<IsoFace>();
            if (this.IsQuad)
            {
                List<Point3d> points = this.Evaluate(surface);
                double a = Utilities.TriangleArea(points[0], points[1], points[2])+ Utilities.TriangleArea(points[2], points[3], points[0]);
                double b = Utilities.TriangleArea(points[0], points[1], points[3]) + Utilities.TriangleArea(points[2], points[3], points[1]);
                outputs.AddRange(this.QuadToTri((a > b) == flip));
            }
            return outputs;
        }

        #endregion
    }
}
