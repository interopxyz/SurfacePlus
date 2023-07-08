using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_Panel_TriDense : GH_Panel__BaseTri
    {
        /// <summary>
        /// Initializes a new instance of the GH_Panel_TriDense class.
        /// </summary>
        public GH_Panel_TriDense()
          : base("Triangle Stellated Surfaces", "Stellated Quad",
              "Divide a surface into a series of triangular surfaces from a stellated UV grid",
              Constants.CatSurface, Constants.SubGrid)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
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

            Grid grid = new Grid(surface);
            grid.SetDenseTriangles((SurfaceDirection)direction, u, v);

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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9e853498-d0a0-4032-9cc8-e6f1c2f65f94"); }
        }
    }
}