using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_OffsetCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the OffsetCurve class.
        /// </summary>
        public GH_OffsetCurve()
          : base("Offset Curve from Surface", "Offset Crv Srf",
              "Offsets a Curve from a Surface",
              Constants.CatCurve, "Util")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddCurveParameter("Curve", "C", "The Curve to offset from the surface", GH_ParamAccess.item);
            pManager[1].Optional = false;
            pManager.AddNumberParameter("Distance", "D", "The offset distance", GH_ParamAccess.item, 1);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "The resulting offset curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            if (!DA.GetData(0, ref surface)) return;

            Curve curve = null;
            if (!DA.GetData(1, ref curve)) return;

            NurbsCurve curve1 = curve.ToNurbsCurve();

            double offset = 1.0;
            DA.GetData(2, ref offset);

            Curve curve2 = curve1.OffsetNormalToSurface(surface, offset);

            DA.SetData(0, curve2);
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
            get { return new Guid("ac41367e-8d91-48f0-8761-371152c7dbe2"); }
        }
    }
}