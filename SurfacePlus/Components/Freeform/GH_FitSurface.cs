using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_FitSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FitSurface class.
        /// </summary>
        public GH_FitSurface()
          : base("Fit Surface", "Fit Srf",
              "Fits a new surface through an existing surface",
              Constants.CatSurface, Constants.SubFreeform)
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
            pManager.AddIntegerParameter("Degree U", "U", "The Surface Degree in the U direction", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Degree V", "V", "The Surface Degree in the V direction", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Tolerance", "D", "Fitting Tolerance", GH_ParamAccess.item, 1.0);
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Output, GH_ParamAccess.item);
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

            int u = surface1.Degree(0);
            DA.GetData(1, ref u);

            int v = surface1.Degree(1);
            DA.GetData(2, ref v);

            double tolerance = 1.0;
            DA.GetData(3, ref tolerance);

            Surface surface2 = surface1.Fit(u,v, tolerance);

            DA.SetData(0, surface2);
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
                return Properties.Resources.Sp_Free_Fit;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("50184f9b-81b0-4b9b-a0a1-a38a7ee4dfec"); }
        }
    }
}