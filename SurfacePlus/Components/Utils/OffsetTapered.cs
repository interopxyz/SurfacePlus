using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurfacePlus.Components.Utils
{
    public class OffsetTapered : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the OffsetTapered class.
        /// </summary>
        public OffsetTapered()
          : base("Tapered Solid", "Taper",
              "Creates a new tapered offset from a surface",
              Constants.CatSurface, Constants.SubUtilities)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddNumberParameter("Parameter", "P", "A unitized parameter between 0.0-1.0", GH_ParamAccess.item, 0.25);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Distance", "D", "The offset distance", GH_ParamAccess.item, 1.0);
            pManager[2].Optional = true;
            pManager.AddBooleanParameter("Keep Source", "A", "If true, the source surface will be kept", GH_ParamAccess.item,true);
            pManager[3].Optional = true;
            pManager.AddBooleanParameter("Keep Target", "B", "If true, the target surface will be kept", GH_ParamAccess.item, true);
            pManager[4].Optional = true;
            pManager.AddBooleanParameter("Keep Walls", "W", "If true, the wall surfaces will be kept", GH_ParamAccess.item, true);
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter(Constants.Brep.Name, Constants.Brep.NickName, Constants.Brep.Output, GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            if (!DA.GetData(0, ref surface)) return;

            NurbsSurface surface1 = surface.ToNurbsSurface();
            surface1.SetDomain(0, new Interval(0, 1));
            surface1.SetDomain(1, new Interval(0, 1));
            Brep brep = Brep.CreateFromSurface(surface1);
            Brep origin = brep.DuplicateBrep();

            double t = 0.25;
            bool isPyramid = false;
            DA.GetData(1, ref t);

            if (t <= 0)
            {
            }
            if (t >= 1)
            {
                isPyramid = true;
            }
            else
            {
            t = t / 2;
                brep = Brep.CreateFromSurface(surface1.Trim(new Interval(t, 1 - t), new Interval(t, 1 - t)));
            }

            double d = 0.25;
            DA.GetData(2, ref d);

            bool a = true;
            DA.GetData(3, ref a);

            bool b = true;
            DA.GetData(4, ref b);

            bool c = true;
            DA.GetData(5, ref c);

            Brep offset = Brep.CreateFromOffsetFace(brep.Faces[0], d, 0.001, false, false);

            List<Brep> breps = new List<Brep>();
            Curve[] e0 = origin.DuplicateNakedEdgeCurves(true, false);
            if (isPyramid)
            {
                surface1.SetDomain(0, new Interval(0, 1));
                surface1.SetDomain(1, new Interval(0, 1));
                Point3d p = surface1.PointAt(0.5, 0.5);
                Vector3d v = surface1.NormalAt(0.5, 0.5);
                v.Unitize();
                v = v * d;
                p = p + v;
                for (int i = 0; i < 4; i++)
                {
                    breps.AddRange(Brep.CreateFromLoft(new Curve[] { e0[i] }, Point3d.Unset, p, LoftType.Normal, false));
                }
            }
            else
            {
                Curve[] e1 = offset.DuplicateNakedEdgeCurves(true, false);

                if (c)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        breps.AddRange(Brep.CreateFromLoft(new Curve[] { e0[i], e1[i] }, Point3d.Unset, Point3d.Unset, LoftType.Normal, false));
                    }
                }
            if (b) breps.Add(offset);
            }

            if (a) breps.Add(origin);

            if (breps.Count != 0) DA.SetData(0, Brep.JoinBreps(breps, 0.001)[0]);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.Sp_Util_Taper;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d75a7528-f207-42c6-ae82-f25dd3bc8d47"); }
        }
    }
}