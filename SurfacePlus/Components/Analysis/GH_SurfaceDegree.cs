using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_SurfaceDegree : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SurfaceDegree class.
        /// </summary>
        public GH_SurfaceDegree()
          : base("Surface Degree", "Srf Deg",
              "Get or Set the the Surface Degree",
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
            pManager.AddIntegerParameter("Degree U", "U", "Optionally increases the Surface Degree in the U direction. If the provided value is lower than the current degree the current degree will be maintained.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Degree V", "V", "Optionally increases the Surface Degree in the V direction. If the provided value is lower than the current degree the current degree will be maintained.", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Output, GH_ParamAccess.item);
            pManager.AddIntegerParameter("Degree U", "U", "The Surface Degree in the U direction", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Degree V", "V", "The Surface Degree in the V direction", GH_ParamAccess.item);
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
            int u = 1;
            if (DA.GetData(1, ref u))
            {
                if ((u > 0) & (u < 12))
                {
                    if(surface1.Degree(0)!=u) surface1.IncreaseDegreeU(u);
                }
                else
                {

                }
            }

            int v = 1;
            if (DA.GetData(2, ref v))
            {
                if ((v > 0) & (v < 12))
                {
                    if (surface1.Degree(1) != v) surface1.IncreaseDegreeV(v);
                }
                else
                {

                }
            }

            DA.SetData(0, surface1);
            DA.SetData(1, surface1.Degree(0));
            DA.SetData(2, surface1.Degree(1));
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
            get { return new Guid("8ef01c6b-f6e7-4342-b379-a52a8ce01fe8"); }
        }
    }
}