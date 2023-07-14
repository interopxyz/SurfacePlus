using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfacePlus
{
    public class Grid
    {

        #region members

        Dictionary<int, List<IsoFace>> faces = new Dictionary<int, List<IsoFace>>();
        NurbsSurface surface = null;

        int[][] a = new int[2][];
        int[][] b = new int[2][];

        private SurfaceDirection direction = SurfaceDirection.U;

        #endregion

        #region constructors

        public Grid(Surface surface)
        {
            this.Defaults();
            this.surface = surface.Unitize();
        }

        public Grid(Grid grid)
        {
            this.Defaults();
            foreach (int key in faces.Keys)
            {
                this.faces.Add(key, new List<IsoFace>());
                foreach (IsoFace face in grid.faces[key])
                {
                    this.faces[key].Add(new IsoFace(face));
                }
            }
            this.direction = grid.direction;
            this.surface = new NurbsSurface(grid.surface);
        }

        #endregion

        #region properties

        public virtual List<IsoFace> Faces
        {
            get
            {
                List<IsoFace> output = new List<IsoFace>();
                int[] keys = this.faces.Keys.ToArray();
                Array.Sort(keys);
                foreach (int key in keys)
                {
                    foreach (IsoFace face in this.faces[key])
                    {
                        output.Add(new IsoFace(face));
                    }
                }

                return output;
            }
        }

        #endregion

        #region methods
        private void Defaults()
        {
            a[0] = new int[] { 0, 1, 2 };
            b[0] = new int[] { 2, 3, 0 };
            a[1] = new int[] { 0, 1, 3 };
            b[1] = new int[] { 2, 3, 1 };
        }

        #region rendering

        public List<Curve> RenderToUV()
        {
            List<Curve> curves = new List<Curve>();
            foreach (IsoFace face in this.Faces) curves.Add(face.RenderUVPolyline());
            return curves;
        }

        public List<Surface> RenderToFacets()
        {
            List<Surface> surfaces = new List<Surface>();
            foreach (IsoFace face in this.Faces) surfaces.Add(face.RenderFacet(this.surface,direction));
            return surfaces;
        }

        public List<Surface> RenderToLoftedSurfaces()
        {
            List<Surface> surfaces = new List<Surface>();
            foreach (IsoFace face in this.Faces) surfaces.Add(face.RenderLoftedSurface(this.surface, direction));
            return surfaces;
        }

        public List<Surface> RenderToIsoSurfaces()
        {
            List<Surface> surfaces = new List<Surface>();
            foreach (IsoFace face in this.Faces) surfaces.Add(face.RenderIsoSurface(this.surface, direction));
            return surfaces;
        }

        public List<Curve> RenderToPolygonBoundaries()
        {
            List<Curve> curves = new List<Curve>();
            foreach (IsoFace face in this.Faces) curves.Add(face.RenderPolygon(this.surface));
            return curves;
        }

        public List<Curve> RenderToInterpolatedBoundaries()
        {
            List<Curve> curves = new List<Curve>();
            foreach (IsoFace face in this.Faces) curves.Add(face.RenderInterpolatedBoundary(this.surface));
            return curves;
        }

        public List<Curve> RenderToGeodesicBoundaries()
        {
            List<Curve> curves = new List<Curve>();
            foreach (IsoFace face in this.Faces) curves.Add(face.RenderGeodesicBoundary(this.surface));
            return curves;
        }

        public List<Curve> RenderToIsoBoundaries()
        {
            List<Curve> curves = new List<Curve>();
            foreach (IsoFace face in this.Faces) curves.Add(face.RenderIsoBoundary(this.surface,direction));
            return curves;
        }

        #endregion

        public List<List<Point2d>> UnderlyingGrid(SurfaceDirection direction, int countU, int countV)
        {
            this.direction = direction;
            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            int cu = countU + 1;
            int cv = countV + 1;

            List<double> u = isocurve.DivideByCount(countU, true).ToList();

            Point2d[] p = new Point2d[2];
            List<List<Point2d>> uv = new List<List<Point2d>>();

            for (int i = 0; i < cu; i++)
            {
                Curve crv = surface.IsoCurve(1 - x, u[i]);
                List<double> v = crv.DivideByCount(countV, true).ToList();

                uv.Add(new List<Point2d>());
                for (int j = 0; j < cv; j++)
                {
                    p[0] = new Point2d(u[i], v[j]);
                    p[1] = new Point2d(v[j], u[i]);
                    uv[i].Add(p[x]);
                }
            }
            return uv;
        }

        #region pattern

        public void SetStaggeredTriangle(SurfaceDirection direction, int countU, int countV, bool flip)
        {
            this.faces.Clear();
            this.direction = direction;
            int c = countV * 2;
            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            List<double> u = isocurve.DivideByCount(countU, true).ToList();

            int[] a = new int[] { 0, 1, 2 };
            int offset = 0;
            if (flip) offset = 1;

            Point2d[][] p = new Point2d[2][];
            for (int i = 1; i < countU + 1; i++)
            {
                int k = (i + offset) % 2;
                int m = 1 - k;
                Curve crvA = surface.IsoCurve(1 - x, u[i - 1]);
                Curve crvB = surface.IsoCurve(1 - x, u[i]);
                List<double> va = crvA.DivideByCount(c, true).ToList();
                List<double> vb = crvB.DivideByCount(c, true).ToList();
                this.faces.Add(i - 1, new List<IsoFace>());
                for (int j = 0; j < va.Count() - 2; j += 2)
                {
                    p[0] = new Point2d[6] { new Point2d(u[i - 1], va[j]), new Point2d(u[i - 1], va[j + 2]), new Point2d(u[i], vb[j + 1]), new Point2d(u[i], vb[j + 2]), new Point2d(u[i], vb[j]), new Point2d(u[i - 1], va[j + 1]) };
                    p[1] = new Point2d[6] { new Point2d(va[j + 2], u[i - 1]), new Point2d(va[j], u[i - 1]), new Point2d(vb[j + 1], u[i]), new Point2d(vb[j], u[i]), new Point2d(vb[j + 2], u[i]), new Point2d(va[j + 1], u[i - 1]) };

                    this.faces[i - 1].Add(new IsoFace(p[x][a[1] + k * 3], p[x][a[0] + k * 3], p[x][a[2] + k * 3]));
                }

                p[0] = new Point2d[6] { new Point2d(u[i - 1], va[0]), new Point2d(u[i - 1], va[1]), new Point2d(u[i], vb[0]), new Point2d(u[i], vb[1]), new Point2d(u[i], vb[0]), new Point2d(u[i - 1], va[0]) };
                p[1] = new Point2d[6] { new Point2d(va[1], u[i - 1]), new Point2d(va[0], u[i - 1]), new Point2d(vb[0], u[i]), new Point2d(vb[0], u[i]), new Point2d(vb[1], u[i]), new Point2d(va[0], u[i - 1]) };
                this.faces[i - 1].Add(new IsoFace(p[x][a[1] + m * 3], p[x][a[0] + m * 3], p[x][a[2] + m * 3]));
                for (int j = 1; j < va.Count() - 3; j += 2)
                {
                    p[0] = new Point2d[6] { new Point2d(u[i - 1], va[j]), new Point2d(u[i - 1], va[j + 2]), new Point2d(u[i], vb[j + 1]), new Point2d(u[i], vb[j + 2]), new Point2d(u[i], vb[j]), new Point2d(u[i - 1], va[j + 1]) };
                    p[1] = new Point2d[6] { new Point2d(va[j + 2], u[i - 1]), new Point2d(va[j], u[i - 1]), new Point2d(vb[j + 1], u[i]), new Point2d(vb[j], u[i]), new Point2d(vb[j + 2], u[i]), new Point2d(va[j + 1], u[i - 1]) };

                    faces[i - 1].Add(new IsoFace(p[x][a[1] + m * 3], p[x][a[0] + m * 3], p[x][a[2] + m * 3]));
                }
                p[0] = new Point2d[6] { new Point2d(u[i - 1], va[c - 1]), new Point2d(u[i - 1], va[c]), new Point2d(u[i], vb[c]), new Point2d(u[i], vb[c]), new Point2d(u[i], vb[c - 1]), new Point2d(u[i - 1], va[c]) };
                p[1] = new Point2d[6] { new Point2d(va[c], u[i - 1]), new Point2d(va[c - 1], u[i - 1]), new Point2d(vb[c], u[i]), new Point2d(vb[c - 1], u[i]), new Point2d(vb[c], u[i]), new Point2d(va[c], u[i - 1]) };
                this.faces[i - 1].Add(new IsoFace(p[x][a[1] + m * 3], p[x][a[0] + m * 3], p[x][a[2] + m * 3]));
            }
        }

        public void SetDenseTriangles(SurfaceDirection direction, int countU, int countV)
        {
            this.SetBasicQuads(direction, countU, countV);

            Dictionary<int, List<IsoFace>> newFaces = new Dictionary<int, List<IsoFace>>();
            foreach (int i in this.faces.Keys)
            {
                int c = this.faces[i].Count;
                newFaces.Add(i, new List<IsoFace>());
                for (int j = 0; j < c; j++) newFaces[i].AddRange(this.faces[i][j].QuadToDense());
            }

            this.faces = newFaces;
        }

        public void SetBasicTriangles(SurfaceDirection direction, int countU, int countV, bool flip)
        {
            this.SetBasicQuads(direction, countU, countV);

            Dictionary<int, List<IsoFace>> newFaces = new Dictionary<int, List<IsoFace>>();
            foreach (int i in this.faces.Keys)
            {
                int c = this.faces[i].Count;
                newFaces.Add(i, new List<IsoFace>());
                for (int j = 0; j < c; j++) newFaces[i].AddRange(this.faces[i][j].QuadToTri(flip));
            }

            this.faces = newFaces;
        }

        public void SetWaveTriangles(SurfaceDirection direction, int countU, int countV, bool flip)
        {
            this.SetBasicQuads(direction, countU, countV);
            int f = Convert.ToInt32(flip);

            Dictionary<int, List<IsoFace>> newFaces = new Dictionary<int, List<IsoFace>>();
            foreach (int i in this.faces.Keys)
            {
                int c = this.faces[i].Count;
                newFaces.Add(i, new List<IsoFace>());
                bool v = ((i + f) % 2) == 0;
                for (int j = 0; j < c; j++)
                {
                    newFaces[i].AddRange(this.faces[i][j].QuadToTri(v));
                }
            }

            this.faces = newFaces;
        }

        public void SetCrossTriangles(SurfaceDirection direction, int countU, int countV, bool flip)
        {
            this.SetBasicQuads(direction, countU, countV);
            int f = Convert.ToInt32(flip);

            Dictionary<int, List<IsoFace>> newFaces = new Dictionary<int, List<IsoFace>>();
            foreach (int i in this.faces.Keys)
            {
                int c = this.faces[i].Count;
                newFaces.Add(i, new List<IsoFace>());
                for (int j = 0; j < c; j++)
                {
                    bool v = ((i + j + f) % 2) == 0;
                    newFaces[i].AddRange(this.faces[i][j].QuadToTri(v));
                }
            }

            this.faces = newFaces;
        }

        public void SetRingTriangles(SurfaceDirection direction, int countU, int countV, bool flip)
        {
            this.SetBasicQuads(direction, countU, countV);
            int f = Convert.ToInt32(flip);

            Dictionary<int, List<IsoFace>> newFaces = new Dictionary<int, List<IsoFace>>();
            foreach (int i in this.faces.Keys)
            {
                int c = this.faces[i].Count;
                newFaces.Add(i, new List<IsoFace>());
                for (int j = 0; j < c; j++)
                {
                    bool v = (((i % 2) * j + f) % 2) == 0;
                    newFaces[i].AddRange(this.faces[i][j].QuadToTri(v));
                }
            }

            this.faces = newFaces;
        }

        public void SetLengthTriangles(SurfaceDirection direction, int countU, int countV, bool flip)
        {
            this.SetBasicQuads(direction, countU, countV);

            Dictionary<int, List<IsoFace>> newFaces = new Dictionary<int, List<IsoFace>>();
            foreach (int i in this.faces.Keys)
            {
                int c = this.faces[i].Count;
                newFaces.Add(i, new List<IsoFace>());
                for (int j = 0; j < c; j++)
                {
                    newFaces[i].AddRange(this.faces[i][j].QuadToTriByLength(surface, flip));
                }
            }

            this.faces = newFaces;
        }

        public void SetAreaTriangles(SurfaceDirection direction, int countU, int countV, bool flip)
        {
            this.SetBasicQuads(direction, countU, countV);

            Dictionary<int, List<IsoFace>> newFaces = new Dictionary<int, List<IsoFace>>();
            foreach (int i in this.faces.Keys)
            {
                int c = this.faces[i].Count;
                newFaces.Add(i, new List<IsoFace>());
                for (int j = 0; j < c; j++)
                {
                    newFaces[i].AddRange(this.faces[i][j].QuadToTriByArea(surface, flip));
                }
            }

            this.faces = newFaces;
        }

        public void SetStaggeredQuads(SurfaceDirection direction, int countU, int countV, List<double> shifts)
        {
            this.direction = direction;
            int x = (int)direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            int cu = countU + 1;
            int cv = countV + 1;
                IsoFace[] face = new IsoFace[2];

            List<double> u = isocurve.DivideByCount(countU, true).ToList();
            List<double> t = shifts.Repeat(cu);

            for (int i = 1; i < countU+1; i++)
            {
                Curve crvA = surface.IsoCurve(1 - x, u[i - 1]);
                Curve crvB = surface.IsoCurve(1 - x, u[i]);
                List<double> va = crvA.DivideByCount(countV, true, t[i - 1]);
                List<double> vb = crvB.DivideByCount(countV, true, t[i - 1]);
                faces.Add(i-1, new List<IsoFace>());

                for (int j = 1; j < va.Count; j++)
                {
                    face[0] = new IsoFace(new Point2d(u[i - 1], va[j - 1]), new Point2d(u[i - 1], va[j]), new Point2d(u[i], vb[j]), new Point2d(u[i], vb[j - 1]));
                    face[1] = new IsoFace(new Point2d(va[j - 1], u[i - 1]), new Point2d(va[j], u[i - 1]), new Point2d(vb[j], u[i]), new Point2d(vb[j - 1], u[i]));
                    faces[i-1].Add(face[x]);
                }
            }
        }

        public void SetRandomQuads(SurfaceDirection direction, int countU, int countV, int seed, Interval domain)
        {
            this.direction = direction;
            int x = (int)direction;
            Random rnd = new Random(seed);
            Curve isocurve = surface.IsoCurve(x, 0);

            int cu = countU + 1;
            int cv = countV + 1;
            IsoFace[] face = new IsoFace[2];

            List<double> u = isocurve.DivideByCount(countU, true).ToList();

            for (int i = 0; i < countU; i++)
            {
                List<double> v = Utilities.RandomList(cv, domain, ref rnd);
                faces.Add(i, new List<IsoFace>());
                for (int j = 0; j < countV; j++)
                {
                    face[0] = new IsoFace(new Point2d(u[i], v[j]), new Point2d(u[i], v[j + 1]), new Point2d(u[i + 1], v[j + 1]), new Point2d(u[i + 1], v[j]));
                    face[1] = new IsoFace(new Point2d(v[j], u[i]), new Point2d(v[j + 1], u[i]), new Point2d(v[j + 1], u[i + 1]), new Point2d(v[j], u[i + 1]));
                    faces[i].Add(face[x]);
                }
            }
        }

        public void SetRandomSubQuads(SurfaceDirection direction, int countU, int countV, int seed, Interval domain, int increment)
        {
            Random rnd = new Random(seed);
            List<List<Point2d>> uv = this.UnderlyingGrid(direction, countU, countV * increment);

            for (int i = 0; i < countU; i++)
            {
                faces.Add(i, new List<IsoFace>());
                List<int> v = Utilities.RandomSteps(countV, domain, ref rnd, increment);
                for (int j = 0; j < countV-1; j++)
                {
                    faces[i].Add(new IsoFace(uv[i][v[j]], uv[i][v[j+1]], uv[i + 1][v[j+1]], uv[i + 1][v[j]]));
                }
            }

        }

        public void SetBasicQuads(SurfaceDirection direction, int countU, int countV)
        {
            List<List<Point2d>> uv = this.UnderlyingGrid(direction, countU, countV);

            for (int i = 0; i < countU; i++)
            {
                faces.Add(i, new List<IsoFace>());
                for (int j = 0; j < countV; j ++)
                {
                    faces[i].Add(new IsoFace(uv[i][j], uv[i][j + 1], uv[i + 1][j + 1], uv[i + 1][j]));
                }
            }
        }

        public void SetShearQuads(SurfaceDirection direction, int countU, int countV, double t, bool flip)
        {
            int f = Convert.ToInt32(flip);
            List<List<Point2d>> uv = this.UnderlyingGrid(direction, countU, countV);
            double v = t;
            if (flip) v = 1 - t;
            for (int i = 0; i < countU; i++)
            {
                faces.Add(i, new List<IsoFace>());
                if (t > 0)
                {
                    if (flip)
                    {
                        if (t < 1)
                        {
                            faces[i].Add(new IsoFace(uv[i][0], uv[i][1], uv[i + 1][0].Tween(uv[i + 1][1], v), uv[i + 1][0]));
                        }
                        else
                        {
                            faces[i].Add(new IsoFace(uv[i][0], uv[i][1], uv[i + 1][0]));

                        }
                    }
                    else
                    {
                        faces[i].Add(new IsoFace(uv[i][0], uv[i + 1][0].Tween(uv[i + 1][1], v), uv[i + 1][0]));
                    }
                }
                for (int j = f; j < countV-(1-f); j++)
                {
                    Point2d a = uv[i+1][j-f].Tween(uv[i+1][j + 1-f], v);
                    Point2d b = uv[i+1][j+1-f].Tween(uv[i+1][j + 2-f], v);
                    faces[i].Add(new IsoFace(uv[i][j], uv[i][j + 1], b, a));
                }
                if (flip)
                {
                    if (t > 0) faces[i].Add(new IsoFace(uv[i][countV], uv[i + 1][countV], uv[i + 1][countV - 1].Tween(uv[i + 1][countV], v)));
                }
                else
                { 
                    if (t < 1)
                    {
                        faces[i].Add(new IsoFace(uv[i][countV - 1], uv[i][countV], uv[i + 1][countV], uv[i + 1][countV-1].Tween(uv[i + 1][countV],v)));
                    }
                    else
                    {
                        faces[i].Add(new IsoFace(uv[i][countV-1], uv[i][countV], uv[i + 1][countV]));
                    }
                }
            }
        }

        public void SetCairoQuads(SurfaceDirection direction, int countU, int countV, double t, bool flip)
        {
            int x = (int)direction;
            int f = Convert.ToInt32(flip);

            List<List<Point2d>> uv = this.UnderlyingGrid(direction, countU, countV);
            IsoFace[][] p = new IsoFace[2][];
            p[0] = new IsoFace[4];
            p[1] = new IsoFace[4];
            for (int i = 0; i < countU; i++)
            {
                int k = i * 2;
                faces.Add(k, new List<IsoFace>());
                faces.Add(k + 1, new List<IsoFace>());
                for (int j = 0; j < countV; j++)
                {
                    int s =(i + j + f) % 2;
                    int a = ((j+f) % 2);
                    Point2d pU0 = uv[i + 0][j + 0 + s].Tween(uv[i + 0][j + 1 - s], t);
                    Point2d pU1 = uv[i + 1][j + 1 - s].Tween(uv[i + 1][j + 0 + s], t);
                    Point2d pV0 = uv[i + 0][j + 1 - s].Tween(uv[i + 1][j + 1 - s], t);
                    Point2d pV1 = uv[i + 1][j + 0 + s].Tween(uv[i + 0][j + 0 + s], t);
                    Point2d pX = (uv[i][j]+ uv[i][j+1]+ uv[i+1][j]+ uv[i+1][j+1])/4.0;

                    p[0][0] = new IsoFace(pV0, pX, pU0, uv[i][j + 1 - s]);
                    p[0][1] = new IsoFace(uv[i][j + s], pU0, pX, pV1);
                    p[0][2] = new IsoFace(pV1, pX, pU1, uv[i + 1][j + s]);
                    p[0][3] = new IsoFace(uv[i + 1][j + 1 - s], pU1, pX, pV0);

                    p[1][0] = new IsoFace(pX, pV0, uv[i][j + 1 - s], pU0);
                    p[1][1] = new IsoFace(pU0, uv[i][j + s], pV1, pX);
                    p[1][2] = new IsoFace(pX, pV1, uv[i + 1][j + s], pU1);
                    p[1][3] = new IsoFace(pU1, uv[i + 1][j + 1 - s], pV0, pX);

                    faces[k].Add(p[s][1-a]);
                    faces[k].Add(p[s][a]);
                    faces[k+1].Add(p[s][2+a]);
                    faces[k+1].Add(p[s][3-a]);
                }
            }
        }

        public void SetShearQuadsa(SurfaceDirection direction, int countU, int countV, double t, bool flip)
        {

            int x = (int)direction;
            this.direction = direction;
            Curve isocurve = surface.IsoCurve(x, 0);

            List<double> u = isocurve.DivideByCount(countU, true).ToList();

            Point2d[][] p = new Point2d[4][];
            for (int i = 1; i < countU + 1; i++)
            {
                Curve crvA = surface.IsoCurve(1 - x, u[i - 1]);
                Curve crvB = surface.IsoCurve(1 - x, u[i]);
                List<double> va = crvA.DivideByCount(countV, true).ToList();
                List<double> vb = crvB.DivideByCount(countV, true).ToList();
                int c = va.Count() - 1;

                faces.Add(i - 1, new List<IsoFace>());

                if (t > 0)
                {
                    double ta = va[0] + (va[1] - va[0]) * t;
                    double tb = vb[0] + (vb[1] - vb[0]) * t;
                    if (flip)
                    {
                        p[0] = new Point2d[4] { new Point2d(u[i - 1], va[1]), new Point2d(u[i - 1], 0), new Point2d(u[i], 0), new Point2d(u[i], tb) };
                        p[1] = new Point2d[4] { new Point2d(0, u[i - 1]), new Point2d(va[1], u[i - 1]), new Point2d(tb, u[i]), new Point2d(vb[0], u[i]) };

                        faces[i - 1].Add(new IsoFace(p[x][2], p[x][1], p[x][3]));
                    }
                    else
                    {
                        p[0] = new Point2d[4] { new Point2d(u[i - 1], ta), new Point2d(u[i - 1], 0), new Point2d(u[i], 0), new Point2d(u[i], vb[1]) };
                        p[1] = new Point2d[4] { new Point2d(0, u[i - 1]), new Point2d(ta, u[i - 1]), new Point2d(vb[1], u[i]), new Point2d(vb[0], u[i]) };

                        faces[i - 1].Add(new IsoFace(p[x][1], p[x][0], p[x][2]));
                    }
                }

                for (int j = 1; j < c; j++)
                {
                    if (flip)
                    {
                        double t0 = vb[j] + (vb[j + 1] - vb[j]) * t;
                        double t1 = vb[j - 1] + (vb[j] - vb[j - 1]) * t;
                        p[0] = new Point2d[4] { new Point2d(u[i - 1], va[j]), new Point2d(u[i - 1], va[j - 1]), new Point2d(u[i], t0), new Point2d(u[i], t1) };
                        p[1] = new Point2d[4] { new Point2d(va[j - 1], u[i - 1]), new Point2d(va[j], u[i - 1]), new Point2d(t1, u[i]), new Point2d(t0, u[i]) };
                    }
                    else
                    {
                        double t0 = va[j] + (va[j + 1] - va[j]) * t;
                        double t1 = va[j - 1] + (va[j] - va[j - 1]) * t;
                        p[0] = new Point2d[4] { new Point2d(u[i - 1], t1), new Point2d(u[i - 1], t0), new Point2d(u[i], vb[j - 1]), new Point2d(u[i], vb[j]) };
                        p[1] = new Point2d[4] { new Point2d(t1, u[i - 1]), new Point2d(t0, u[i - 1]), new Point2d(vb[j - 1], u[i]), new Point2d(vb[j], u[i]) };
                    }
                    faces[i - 1].Add(new IsoFace(p[x][1], p[x][0], p[x][2], p[x][3]));
                }

                if (t < 1)
                {
                    if (flip)
                    {
                        double tb = vb[c - 1] + (vb[c] - vb[c - 1]) * t;
                        p[0] = new Point2d[4] { new Point2d(u[i - 1], 1), new Point2d(u[i - 1], va[c - 1]), new Point2d(u[i], tb), new Point2d(u[i], 1) };
                        p[1] = new Point2d[4] { new Point2d(va[c - 1], u[i - 1]), new Point2d(1, u[i - 1]), new Point2d(1, u[i]), new Point2d(tb, u[i]) };
                    }
                    else
                    {
                        double ta = va[c - 1] + (va[c] - va[c - 1]) * t;
                        p[0] = new Point2d[4] { new Point2d(u[i - 1], 1), new Point2d(u[i - 1], ta), new Point2d(u[i], vb[c - 1]), new Point2d(u[i], 1) };
                        p[1] = new Point2d[4] { new Point2d(ta, u[i - 1]), new Point2d(1, u[i - 1]), new Point2d(1, u[i]), new Point2d(vb[c - 1], u[i]) };
                    }
                    faces[i - 1].Add(new IsoFace(p[x][1], p[x][0], p[x][3], p[x][2]));
                }
                else
                {
                    p[0] = new Point2d[4] { new Point2d(u[i - 1], 1), new Point2d(u[i - 1], vb[c - 1]), new Point2d(u[i], vb[c - 1]), new Point2d(u[i], 1) };
                    p[1] = new Point2d[4] { new Point2d(vb[c - 1], u[i - 1]), new Point2d(1, u[i - 1]), new Point2d(1, u[i]), new Point2d(vb[c - 1], u[i]) };
                    faces[i - 1].Add(new IsoFace(p[x][2], p[x][0], p[x][3]));
                }

            }
        }

        public void SetDiamondTriangles(SurfaceDirection direction, int countU, int countV, bool flip, bool interior, bool edges)
        {
            int x = (int)direction;
            int f = Convert.ToInt32(flip);

            int cu = countU + 1;
            int cv = countV + 1;
            List<List<Point2d>> uv = this.UnderlyingGrid(direction, countU, countV);

            int k = 0;

            if (interior)
            {
                for (int i = 0; i < countU - 1; i++)
                {
                    k = ((i + f) % 2);
                    int a = (i * 2);

                    faces.Add(a + 1, new List<IsoFace>());
                    faces.Add(a + 2, new List<IsoFace>());
                    for (int j = 0; j < cv - 2 - k; j += 2)
                    {

                        Point2d uv0 = uv[i + 0][j + k + 1];
                        Point2d uv1 = uv[i + 0 + 1][j + k + 2];
                        Point2d uv2 = uv[i + 0 + 2][j + k + 1];
                        Point2d uv3 = uv[i + 0 + 1][j + k];

                        faces[a + 1].Add(new IsoFace(uv0, uv1, uv3));
                        faces[a + 2].Add(new IsoFace(uv3, uv1, uv2));
                    }
                }
            }

            k = 1 - f;
            if (edges)
            {
                faces.Add(0, new List<IsoFace>());
                if (!flip) faces[0].Add(new IsoFace(uv[0][0], uv[0][1], uv[1][0]));
                for (int j = 0; j < cv - 2 - k; j += 2)
                {
                    Point2d uv0 = uv[0][j + k];
                    Point2d uv1 = uv[0][j + k + 2];
                    Point2d uv2 = uv[1][j + k + 1];

                    faces[0].Add(new IsoFace(uv0, uv1, uv2));
                }
                if (((f + countV) % 2) == 0) faces[0].Add(new IsoFace(uv[0][cv - 2], uv[0][cv - 1], uv[1][cv - 1]));


                k = ((1 - f) + countU) % 2;
                faces.Add(countU*2, new List<IsoFace>());
                if (((f + countU) % 2) == 0) faces[countU*2].Add(new IsoFace(uv[cu - 2][0], uv[cu - 1][1], uv[cu - 1][0]));
                for (int j = 0; j < cv - 2 - k; j += 2)
                {
                    Point2d uv0 = uv[countU][j + k];
                    Point2d uv1 = uv[countU - 1][j + k + 1];
                    Point2d uv2 = uv[countU][j + k + 2];

                    faces[countU*2].Add(new IsoFace(uv0, uv1, uv2));
                }
                if (((f + countU + countV) % 2) == 0) faces[countU*2].Add(new IsoFace(uv[cu - 2][cv - 1], uv[cu - 1][cv - 1], uv[cu - 1][cv - 2]));

                k = 1 - f;
                for (int i = 0; i < countU - 1 - k; i += 2)
                {
                    int a = (i * 2);
                    if (!faces.ContainsKey(a + 1)) faces.Add(a + 1, new List<IsoFace>());
                    if (!faces.ContainsKey(a + 2)) faces.Add(a + 2, new List<IsoFace>());
                    Point2d uv0 = uv[i + k][0];
                    Point2d uv1 = uv[i + k + 1][1];
                    Point2d uv2 = uv[i + k + 2][0];
                    Point2d uv3 = uv[i + k + 1][0];

                    faces[a + 1 + k*2].Insert(0, new IsoFace(uv0, uv1, uv3));
                    faces[a + 2 + k*2].Insert(0, new IsoFace(uv3, uv1, uv2));
                }

                k = ((1 - f) + countV) % 2;
                for (int i = 0; i < countU - 1 - k; i += 2)
                {
                    int a = (i * 2);
                    if (!faces.ContainsKey(a + 1 + k)) faces.Add(a + 1 + k, new List<IsoFace>());
                    if (!faces.ContainsKey(a + 2 + k)) faces.Add(a + 2 + k, new List<IsoFace>());
                    Point2d uv0 = uv[i + k][cv - 1];
                    Point2d uv1 = uv[i + k + 2][cv - 1];
                    Point2d uv2 = uv[i + k + 1][cv - 2];
                    Point2d uv3 = uv[i + k + 1][cv-1];

                    faces[a + 1 + k*2].Add(new IsoFace(uv0, uv3, uv2));
                    faces[a + 2 + k*2].Add(new IsoFace(uv2, uv3, uv1));
                }
            }

        }

        public void SetDiamondQuads(SurfaceDirection direction, int countU, int countV, bool flip, bool interior, bool edges)
        {
            int x = (int)direction;
            int f = Convert.ToInt32(flip);

            int cu = countU + 1;
            int cv = countV + 1;
            List<List<Point2d>> uv = this.UnderlyingGrid(direction, countU, countV);

            int k = 0;

            if (interior)
            {
                for (int i = 0; i < countU - 1; i++)
                {
                    k = ((i + f) % 2);

                    faces.Add(i + 1, new List<IsoFace>());
                    for (int j = 0; j < cv - 2 - k; j += 2)
                    {

                        Point2d uv0 = uv[i + 0][j + k + 1];
                        Point2d uv1 = uv[i + 0 + 1][j + k + 2];
                        Point2d uv2 = uv[i + 0 + 2][j + k + 1];
                        Point2d uv3 = uv[i + 0 + 1][j + k];

                        faces[i + 1].Add(new IsoFace(uv0, uv1, uv2, uv3));
                    }
                }
            }

             k = 1 - f;
            if (edges)
            {
                faces.Add(0, new List<IsoFace>());
                if (!flip) faces[0].Add(new IsoFace(uv[0][0], uv[0][1], uv[1][0]));
                for (int j = 0; j < cv - 2 - k; j += 2)
                {
                    Point2d uv0 = uv[0][j + k];
                    Point2d uv1 = uv[0][j + k + 2];
                    Point2d uv2 = uv[1][j + k + 1];

                    faces[0].Add(new IsoFace(uv0, uv1, uv2));
                }
                if (((f + countV) % 2) == 0) faces[0].Add(new IsoFace(uv[0][cv - 2], uv[0][cv - 1], uv[1][cv - 1]));


                k = ((1 - f) + countU) % 2;
                faces.Add(countU, new List<IsoFace>());
                if (((f + countU) % 2) == 0) faces[countU].Add(new IsoFace(uv[cu - 2][0], uv[cu - 1][1], uv[cu - 1][0]));
                for (int j = 0; j < cv - 2 - k; j += 2)
                {
                    Point2d uv0 = uv[countU][j + k];
                    Point2d uv1 = uv[countU - 1][j + k + 1];
                    Point2d uv2 = uv[countU][j + k + 2];

                    faces[countU].Add(new IsoFace(uv0, uv1, uv2));
                }
                if (((f + countU + countV) % 2) == 0) faces[countU].Add(new IsoFace(uv[cu - 2][cv - 1], uv[cu - 1][cv - 1], uv[cu - 1][cv - 2]));

                k = 1 - f;
                for (int i = 0; i < countU - 1 - k; i += 2)
                {
                    if (!faces.ContainsKey(i + 1)) faces.Add(i + 1, new List<IsoFace>());
                    Point2d uv0 = uv[i + k][0];
                    Point2d uv1 = uv[i + k + 1][1];
                    Point2d uv2 = uv[i + k + 2][0];

                    faces[i +1+k].Insert(0, new IsoFace(uv0, uv1, uv2));
                }

                k = ((1 - f) + countV) % 2;
                for (int i = 0; i < countU - 1 - k; i += 2)
                {
                    if (!faces.ContainsKey(i + 1+k)) faces.Add(i + 1+k, new List<IsoFace>());
                    Point2d uv0 = uv[i + k][cv - 1];
                    Point2d uv1 = uv[i + k + 2][cv - 1];
                    Point2d uv2 = uv[i + k + 1][cv - 2];

                    faces[i + 1 + k].Add(new IsoFace(uv0, uv1, uv2));
                }
            }

        }

        public void SetHexagons(SurfaceDirection direction, int countU, int countV, double t, bool flip, bool interior, bool edges)
        {
            this.direction = direction;
            int x = (int)direction;
            int f = Convert.ToInt32(flip);
            Curve isocurve = surface.IsoCurve(x, 0);

            int c = (int)Math.Floor(countU / 2.0)+1;
            int cu = countU + 1;
            int cv = countV * 2 + 1;
            int oddU = (countU + f + 1) % 2;
            int oddV = (countV + f + 1) % 2;
            bool isOddU = (oddU % 2) == 0;
            bool isOddV = (oddV % 2) == 0;

            List<double> u = isocurve.DivideByCount(countU, true).ToList();

            Point2d[] p = new Point2d[2];
            List<List<Point2d>> uv = new List<List<Point2d>>();

            for (int i = 0; i < cu; i++)
            {
                Curve crv = surface.IsoCurve(1 - x, u[i]);
                List<double> v = crv.DivideByCount(countV * 2, true).ToList();

                uv.Add(new List<Point2d>());
                for (int j = 0; j < cv; j++)
                {
                    p[0] = new Point2d(u[i], v[j]);
                    p[1] = new Point2d(v[j], u[i]);
                    uv[i].Add(p[x]);
                }
            }
            int k = 0;

            if (interior) {
            for (int i = 0; i < cu - 2; i += 2)
            {
                faces.Add(i/2 + 1, new List<IsoFace>());
                for (int j = 1; j < cv - 2; j += 2)
                {
                    k = ((j / 2 + f) % 2);

                    if ((i + k) < (cu - 2))
                    {

                        Point2d uvA0 = uv[i + k][j + 1].Tween(uv[i + k][j - 1], t);
                        Point2d uvA1 = uv[i + k][j + 1].Tween(uv[i + k][j + 3], t);

                        Point2d uvB0 = uv[i + k + 1][j - 1].Tween(uv[i + k + 1][j + 1], t);
                        Point2d uvB1 = uv[i + k + 1][j + 3].Tween(uv[i + k + 1][j + 1], t);

                        Point2d uvC0 = uv[i + k + 2][j + 1].Tween(uv[i + k + 2][j - 1], t);
                        Point2d uvC1 = uv[i + k + 2][j + 1].Tween(uv[i + k + 2][j + 3], t);

                        faces[i/2 + 1].Add(new IsoFace(uvA0, uvA1, uvB1, uvC1, uvC0, uvB0));
                    }
                }
            }
            }

            if(edges)
            { 
            k = (1 - f) * 2;
            faces.Add(0, new List<IsoFace>());
            if (!flip) faces[0].Add(new IsoFace(uv[0][0], uv[0][2].Tween(uv[0][0], t), uv[1][0].Tween(uv[1][2], t), uv[1][0]));

            for (int j = 1; j < cv - 2 - k; j += 4)
            {
                Point2d uvA0 = uv[1][j + k + 1].Tween(uv[1][j + k + 3], t);
                Point2d uvA1 = uv[1][j + k + 1].Tween(uv[1][j + k - 1], t);

                Point2d uvB0 = uv[0][j + k + 3].Tween(uv[0][j + k + 1], t);
                Point2d uvB1 = uv[0][j + k - 1].Tween(uv[0][j + k + 1], t);

                faces[0].Add(new IsoFace(uvA0, uvA1, uvB1, uvB0));
            }
            if (!isOddV) faces[0].Add(new IsoFace(uv[0][cv - 3].Tween(uv[0][cv - 1], t), uv[0][cv - 1], uv[1][cv - 1], uv[1][cv - 1].Tween(uv[1][cv - 3], t)));

            k = ((1-f+countU)%2) * 2;
            faces.Add(c, new List<IsoFace>());
            for (int j = 1; j < cv - 2 - k; j += 4)
            {
                Point2d uvA0 = uv[cu - 2][j + k + 1].Tween(uv[cu - 2][j + k - 1], t);
                Point2d uvA1 = uv[cu - 2][j + k + 1].Tween(uv[cu - 2][j + k + 3], t);

                Point2d uvB0 = uv[cu - 1][j + k - 1].Tween(uv[cu - 1][j + k + 1], t);
                Point2d uvB1 = uv[cu - 1][j + k + 3].Tween(uv[cu - 1][j + k + 1], t);

                faces[c].Add(new IsoFace(uvA0, uvA1, uvB1, uvB0));
            }
            if (!isOddU) faces[c].Insert(0,new IsoFace(uv[cu-1][2].Tween(uv[cu-1][0], t), uv[cu-1][0], uv[cu-2][0], uv[cu - 2][0].Tween(uv[cu - 2][2], t)));

            k = (1-f);
            for (int i = 0; i < cu - 2-oddU; i += 2)
            {
                    if (!faces.ContainsKey(i / 2 + 1)) faces.Add(i / 2 + 1, new List<IsoFace>());
                Point2d uvA0 = uv[i + k][0];
                Point2d uvA1 = uv[i + k][0].Tween(uv[i + k][2], t);

                Point2d uvB0 = uv[i + k + 1][0];
                Point2d uvB1 = uv[i + k + 1][2].Tween(uv[i + k + 1][0], t);

                Point2d uvC0 = uv[i + k + 2][0];
                Point2d uvC1 = uv[i + k + 2][0].Tween(uv[i + k + 2][2], t);

                faces[i/2+1].Insert(0,new IsoFace(uvA0, uvA1, uvB1, uvC1, uvC0, uvB0));
            }

            k = (f+1+countV)%2;
            for (int i = 0; i < cu - 2 - k; i += 2)
            {

                Point2d uvA0 = uv[i + k][cv - 1].Tween(uv[i + k][cv - 3], t);
                Point2d uvA1 = uv[i + k][cv - 1];

                Point2d uvB0 = uv[i + k + 1][cv - 3].Tween(uv[i + k + 1][cv - 1], t);
                Point2d uvB1 = uv[i + k + 1][cv - 1];

                Point2d uvC0 = uv[i + k + 2][cv - 1].Tween(uv[i + k + 2][cv - 3], t);
                Point2d uvC1 = uv[i + k + 2][cv - 1];

                faces[i / 2 + 2 - k].Insert(0, new IsoFace(uvA0, uvA1, uvB1, uvC1, uvC0, uvB0));
            }
            if(((oddU+oddV)%2)==f) faces[c].Add(new IsoFace(uv[cu - 1][cv-1], uv[cu - 1][cv-3].Tween(uv[cu - 1][cv-1], t), uv[cu - 2][cv-1].Tween(uv[cu - 2][cv-3], t), uv[cu - 2][cv-1]));
            }
        }

        public void SetHexQuads(SurfaceDirection direction, int countU, int countV, double t, bool flip, bool interior, bool edges, int shift = 0)
        {
            this.SetHexagons(direction, countU, countV, t, flip,interior,edges);

            Dictionary<int, List<IsoFace>> newFaces = new Dictionary<int, List<IsoFace>>();
            foreach (int i in this.faces.Keys)
            {
                int c = this.faces[i].Count;
                newFaces.Add(i, new List<IsoFace>());
                for (int j = 0; j < c; j++)
                {
                    if (this.faces[i][j].IsQuad) newFaces[i].Add(new IsoFace(this.faces[i][j]));
                    newFaces[i].AddRange(this.faces[i][j].HexToSplitQuads(shift));
                }
            }

            this.faces = newFaces;
        }

        #endregion

        #endregion

    }
}
