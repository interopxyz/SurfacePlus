using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_Inset : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Inset class.
        /// </summary>
        public GH_Inset()
          : base("Inset Surface", "Inset",
              "Creates a new Surface inset from the original surface",
              Constants.CatSurface, Constants.SubUtilities)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddNumberParameter("Parameter", "P", "A unitized parameter between 0.0-1.0", GH_ParamAccess.item, 0.25);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter(Constants.Brep.Name, Constants.Brep.NickName, Constants.Brep.Output, GH_ParamAccess.item);
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
            surface1.SetDomain(0, new Interval(0, 1));
            surface1.SetDomain(1, new Interval(0, 1));
            Brep brep = Brep.CreateFromSurface(surface1);

            double t = 0.25;
            DA.GetData(1, ref t);

            if (t <= 0)
            {
                DA.SetData(0, brep);
                return;
            }
            if (t >= 1)
            {
                DA.SetData(0, null);
                return;
            }
            t = t / 2;

            DA.SetData(0, surface1.Trim(new Interval(t, 1 - t), new Interval(t, 1 - t)));
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
                return Properties.Resources.Sp_Util_Inset;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fd161645-1824-45a5-a7b2-2d287c1aeca6"); }
        }
    }
}