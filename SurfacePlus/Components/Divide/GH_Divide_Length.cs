using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_Divide_Length : GH_Divide__Base
    {
        /// <summary>
        /// Initializes a new instance of the DivideLength class.
        /// </summary>
        public GH_Divide_Length()
          : base("Divide Length", "Div Len",
              "Divides a surface in the U or V direction by an approximated length at a parameter",
              Constants.CatSurface, Constants.SubDivide)
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
            base.RegisterInputParams(pManager);
            pManager.AddPointParameter("Parameter", "P", "The unitized surface parameter for division", GH_ParamAccess.item, new Point3d(0.5, 0.5, 0));
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Length", "L", "The target segment length", GH_ParamAccess.item, 1.0);
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            base.RegisterOutputParams(pManager);
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

            Point3d p = new Point3d(0.5,0.5,0);
            DA.GetData(2, ref p);

            double d = 1.0;
            DA.GetData(3, ref d);

            DA.SetDataList(0, surface1.DivideLength((SurfaceDirection)direction, p, d));
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
                return Properties.Resources.Sp_Div_Length;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("51ce2d48-136f-4cc7-93b2-abcc49505cc4"); }
        }
    }
}