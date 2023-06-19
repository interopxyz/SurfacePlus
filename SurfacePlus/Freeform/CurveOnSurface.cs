using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Freeform
{
    public class CurveOnSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CurveOnSurface class.
        /// </summary>
        public CurveOnSurface()
          : base("Extend Surface", "Extend Srf",
              "Extend a Surface Edge",
              Constants.CatCurve, "Spline")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddPointParameter("Points", "P", "3d Points", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Tolerance", "D", "Tolerance", GH_ParamAccess.item, 0.001);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve on Surface", GH_ParamAccess.item);
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

            List<Point3d> points = new List<Point3d>();
            DA.GetDataList(1, points);

            double tolerance = 0.001;
            DA.GetData(2, ref tolerance);

            NurbsCurve curve = surface1.InterpolatedCurveOnSurface(points, tolerance);

            DA.SetData(0, curve);
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("676b8304-0e2d-4812-a606-03326630a5bc"); }
        }
    }
}