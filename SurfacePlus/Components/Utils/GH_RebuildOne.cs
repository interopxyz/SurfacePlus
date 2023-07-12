using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class RebuildOne : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RebuildOne class.
        /// </summary>
        public RebuildOne()
          : base("Rebuild One", "Rebuild 1",
              "Rebuild a surface in either the U or V direction",
              Constants.CatSurface, Constants.SubUtilities)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager.AddIntegerParameter("Direction", "D", "Set the U or V direction", GH_ParamAccess.item,0);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Count", "C", "The point count of the new surface", GH_ParamAccess.item,4);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Loft Type", "L", "The surface loft type", GH_ParamAccess.item,0);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Tolerance", "D", "Tolerance value", GH_ParamAccess.item, 0.001);
            pManager[4].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            paramA.AddNamedValue("U", 0);
            paramA.AddNamedValue("V", 1);

            Param_Integer paramB = (Param_Integer)pManager[3];
            foreach (LoftType value in Enum.GetValues(typeof(LoftType)))
            {
                paramB.AddNamedValue(value.ToString(), (int)value);
            }
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

            int direction = 0;
            DA.GetData(1, ref direction);

            int count = 4;
            DA.GetData(2, ref count);

            int type = 0;
            DA.GetData(3, ref type);

            double tolerance = 0.001;
            DA.GetData(4, ref tolerance);

            NurbsSurface surface2 = surface1.RebuildOneDirection(direction, count, (LoftType)type, tolerance);

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
                return Properties.Resources.Sp_Util_Rebuild_One;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4279ad50-d07a-41c3-ad24-aac5d7e915d6"); }
        }
    }
}