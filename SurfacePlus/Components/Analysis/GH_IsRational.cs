using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_IsRational : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Rationalize class.
        /// </summary>
        public GH_IsRational()
          : base("Is Rational", "Rational",
              "Get or set if a surface is rational or non rational",
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
            pManager.AddBooleanParameter("Rational", "R", "If true, will attempt to make the surface rational, if false non rational", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Output, GH_ParamAccess.item);
            pManager.AddBooleanParameter("Status", "S", "If true, the surface is rational. Not all surfaces can be made rational or irrational. This value represents the rational status of the surface.", GH_ParamAccess.item);
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

            bool rational = true;
            bool isActive = DA.GetData(1, ref rational);
            
            if (isActive) { 
            if (rational)
            {
                surface1.MakeRational();
            }
            else
            {
                surface1.MakeNonRational();
            }
            }

            DA.SetData(0, surface1);
            DA.SetData(1, surface1.IsRational);
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
                return Properties.Resources.Sp_Ana_Rational;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("67d9b833-fb11-4da0-aaaa-82839ca3dd93"); }
        }
    }
}