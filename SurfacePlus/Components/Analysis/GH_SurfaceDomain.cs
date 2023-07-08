using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Analysis
{
    public class GH_SurfaceDomain : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GH_SurfaceDomain()
          : base("Surface Domain", "Srf Dom",
              "Get or Set the the Surface Domain",
              Constants.CatSurface, Constants.SubAnalysis)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddIntervalParameter("Domain U", "U", "Optionally sets the Surface Domain in the U direction", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntervalParameter("Domain V", "V", "Optionally sets the Surface Domain in the V direction", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Output, GH_ParamAccess.item);
            pManager.AddIntervalParameter("Domain U", "U", "The Surface Domain in the U direction", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Domain V", "V", "The Surface Domain in the V direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            if (!DA.GetData(0, ref surface))return;

            NurbsSurface surface1 = surface.ToNurbsSurface();
            Interval u = new Interval(0, 1);
            if(DA.GetData(1,ref u)) surface1.SetDomain(0, u);
            Interval v = new Interval(0, 1);
            if (DA.GetData(2, ref v)) surface1.SetDomain(1, v);
            
            DA.SetData(0, surface1);
            DA.SetData(1, surface1.Domain(0));
            DA.SetData(2, surface1.Domain(1));
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
            get { return new Guid("0531b564-8b29-4836-82e1-2674a321f0cb"); }
        }
    }
}