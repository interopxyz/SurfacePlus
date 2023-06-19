using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus
{
    public class Rebuild : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Rebuild class.
        /// </summary>
        public Rebuild()
          : base("Rebuild", "Rebuild",
              "Description",
              Constants.CatSurface, Constants.SubUtilities)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager.AddIntegerParameter("Degree U", "U", "The new degree in the U direction", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Degree V", "V", "The new degree in the V direction", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Count U", "A", "The new control point count in the U direction", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Count V", "B", "The new control point count in the V direction", GH_ParamAccess.item); 
            pManager[2].Optional = true;

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

            int Ud = 3;
            DA.GetData(1, ref Ud);

            int Vd = 3;
            DA.GetData(2, ref Vd);

            int Up = 4;
            DA.GetData(3, ref Up);

            int Vp = 4;
            DA.GetData(4, ref Vp);

            NurbsSurface surface2 = surface1.Rebuild(Ud,Vd,Up,Vp);

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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3b437761-edd2-4aa4-9d3e-000d41a499fb"); }
        }
    }
}