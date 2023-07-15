using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_IsSurfaceClosed : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_IsSurfaceClosed class.
        /// </summary>
        public GH_IsSurfaceClosed()
          : base("Is Closed", "Closed",
              "Check if a surface is closed in either the U or V directions or solid",
              Constants.CatSurface, Constants.SubAnalysis)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Solid", "S", "Is the surface a pseudo solid", GH_ParamAccess.item);
            pManager.AddBooleanParameter("U Direction", "U", "Is the surface closed in the U direction", GH_ParamAccess.item);
            pManager.AddBooleanParameter("V Direction", "V", "Is the surface closed in the V direction", GH_ParamAccess.item);
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

            DA.SetData(0, surface1.IsSolid);
            DA.SetData(1, surface1.IsClosed(0));
            DA.SetData(2, surface1.IsClosed(1));
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
                return Properties.Resources.Sp_Ana_IsClosed;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("46407c82-500f-4f1d-b79b-5794bd64e308"); }
        }
    }
}