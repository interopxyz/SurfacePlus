using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_SubSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_SubSurface class.
        /// </summary>
        public GH_SubSurface()
          : base("Sub Surface", "Sub Srf",
              "Splits a sub surface from an interval",
              Constants.CatSurface, Constants.SubUtilities)
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
            pManager.AddIntegerParameter("Direction", "D", "Set the U or V direction", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager.AddIntervalParameter("Interval", "I", "The surface interval to extract", GH_ParamAccess.item, new Interval(0.25, 0.75));
            pManager[2].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            paramA.AddNamedValue("U", 0);
            paramA.AddNamedValue("V", 1);
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

            NurbsSurface surface1 = surface.Unitize();

            int direction = 0;
            DA.GetData(1, ref direction);

            Interval t = new Interval(0.25,0.75);
            DA.GetData(2, ref t);

            Surface splitA = surface1.ToNurbsSurface();
            if(t.Min > 0) splitA = surface1.Split(direction, surface1.Domain(direction).Evaluate(t.Min))[1];
            Surface splitB = splitA.ToNurbsSurface();
            if (t.Max < 1) splitB = splitA.Split(direction, surface1.Domain(direction).Evaluate(t.Max))[1];

            DA.SetData(0, splitB);
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
                return Properties.Resources.Sp_Util_SubSurface3;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("67d39f08-0be6-4371-a465-f04ddb7232a3"); }
        }
    }
}