using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_Panel_Quad_Shear : GH_Panel__BaseTri
    {
        /// <summary>
        /// Initializes a new instance of the GH_Panel_QuadShear class.
        /// </summary>
        public GH_Panel_Quad_Shear()
          : base("Quad Sheared Surfaces", "Shear Quad",
              "Divide a surface into a series of sheared quad surfaces",
              Constants.CatSurface, Constants.SubGrid)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddNumberParameter("Paramter", "P", "The shifting value" , GH_ParamAccess.item, 0.5);
            pManager.AddBooleanParameter("Flip", "F", "Flip the orientation of the triangulation panel", GH_ParamAccess.item, false);
            pManager[5].Optional = true;
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

            int direction = 0;
            DA.GetData(1, ref direction);

            int u = 4;
            DA.GetData(2, ref u);
            u = Math.Max(1, u);

            int v = 4;
            DA.GetData(3, ref v);
            v = Math.Max(1, v);

            double t = 0.5;
            DA.GetData(4, ref t);

            bool flip = false;
            DA.GetData(5, ref flip);

            Grid grid = new Grid(surface);
            grid.SetShearQuads((SurfaceDirection)direction, u, v, t,flip);

            DA.SetDataList(0, grid.RenderToFacets());
            DA.SetDataList(1, grid.RenderToUV());
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
                return Properties.Resources.Sp_Panel_Quad_Shear;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a2dc3c08-0784-4609-a89c-1dc67acbd246"); }
        }
    }
}