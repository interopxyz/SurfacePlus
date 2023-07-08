using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurfacePlus.Utils
{
    public class GH_WeightControlPoints : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the WeightControlPoints class.
        /// </summary>
        public GH_WeightControlPoints()
          : base("Weight Control Points", "Weight",
              "Weight control points of a surface",
              Constants.CatSurface, Constants.SubUtilities)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddIntegerParameter("Indices", "I", "Control point indices", GH_ParamAccess.list);
            pManager[1].Optional = false;
            pManager.AddNumberParameter("Weights", "W", "Control point weights corresponding to each index", GH_ParamAccess.list);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Output, GH_ParamAccess.item);
            pManager.AddPointParameter("P", "P", "P", GH_ParamAccess.list);
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
            surface1.MakeRational();
            int c = surface1.Points.CountU;

            List<int> indices = new List<int>();
            if (!DA.GetDataList(1, indices)) return;

            List<double> weights = new List<double>();
            DA.GetDataList(2, weights);
            if (weights.Count < 1) weights.Add(0.5);

            weights.AddRange(Enumerable.Repeat(weights[weights.Count - 1], indices.Count - weights.Count).ToList());

            List<Point3d> points = new List<Point3d>();
            for (int i=0;i < indices.Count;i++)
            { 
                int index = indices[i];
            double weight = weights[i];

            int v = (int)Math.Floor((double)index / (double)c);
            int u = index - v*c;
                Point3d p = surface1.Points.GetControlPoint(u,v).Location;
                ControlPoint cp = new ControlPoint(p.X* weight, p.Y* weight, p.Z* weight, weight);
                surface1.Points.SetControlPoint(u,v, cp);
            }

            foreach (ControlPoint pt in surface1.Points)
            {
                points.Add(pt.Location);
            }
            DA.SetData(0, surface1);
            DA.SetDataList(1, points);
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
            get { return new Guid("d07031cc-95f2-4730-8054-1df81be1f007"); }
        }
    }
}