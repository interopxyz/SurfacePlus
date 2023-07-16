using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_SurfaceSide : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_SurfaceSide class.
        /// </summary>
        public GH_SurfaceSide()
          : base("Surface Side", "Side",
              "Test the closest side to a sample point",
              Constants.CatSurface, Constants.SubAnalysis)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddPointParameter("Point", "uv", "The UV coordinate to evaluate", GH_ParamAccess.item);
            pManager[1].Optional = false;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Isostatus Index", "I", "The index of the isostatus state", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "The isostatus name", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve", "C", "The corresponding edge curve", GH_ParamAccess.item);
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
            NurbsSurface surface2 = surface.ToNurbsSurface();
            surface2.SetDomain(0, new Interval(0, 1));
            surface2.SetDomain(1, new Interval(0, 1));

            Point3d p = new Point3d();
            if (!DA.GetData(1, ref p)) return;

            IsoStatus side = surface1.ClosestSide(p.X, p.Y);

            DA.SetData(0, (int)side);
            DA.SetData(1, side.ToString());

            switch (side)
            {
                case IsoStatus.South:
                    DA.SetData(2, surface2.IsoCurve(0, 0));
                    break;
                case IsoStatus.East:
                    DA.SetData(2, surface2.IsoCurve(1, 1));
                    break;
                case IsoStatus.North:
                    DA.SetData(2, surface2.IsoCurve(0, 1));
                    break;
                case IsoStatus.West:
                    DA.SetData(2, surface2.IsoCurve(1, 0));
                    break;
            }

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
                return Properties.Resources.Sp_Ana_Side4;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1f9f3b60-041f-4363-9ade-472f9312cd52"); }
        }
    }
}