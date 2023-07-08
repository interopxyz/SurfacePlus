using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components.Grids.Curves
{
    public class GH_Cells_Hexagon : GH_Cells__BaseGrid
    {
        /// <summary>
        /// Initializes a new instance of the GH_Cells_Hexagon class.
        /// </summary>
        public GH_Cells_Hexagon()
          : base("Hexagon Curves", "Hex Crvs",
              "Divide a surface into a series of hexagon curves",
              Constants.CatCurve, Constants.SubGrid)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.septenary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddNumberParameter("Paramter", "P", "The shifted ", GH_ParamAccess.item, 0.333);
            pManager.AddBooleanParameter("Flip", "F", "Flip the orientation of the triangulation panel", GH_ParamAccess.item, false);
            pManager[6].Optional = true;
            pManager.AddIntegerParameter("Edges", "E", "Edge filtering mode", GH_ParamAccess.item, 0);
            pManager[7].Optional = true;

            Param_Integer param = (Param_Integer)pManager[7];
            param.AddNamedValue("All", 0);
            param.AddNamedValue("Interior", 1);
            param.AddNamedValue("Edges", 2);

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

            int type = 0;
            DA.GetData(1, ref type);

            int direction = 0;
            DA.GetData(2, ref direction);

            int u = 4;
            DA.GetData(3, ref u);
            u = Math.Max(1, u);

            int v = 4;
            DA.GetData(4, ref v);
            v = Math.Max(1, v);

            double t = 0.5;
            DA.GetData(5, ref t);

            bool flip = false;
            DA.GetData(6, ref flip);

            int edgeType = 0;
            DA.GetData(7, ref edgeType);

            bool edges = true;
            bool interior = true;
            if (edgeType == 1) edges = false;
            if (edgeType == 2) interior = false;

            Grid grid = new Grid(surface);
            grid.SetHexagons((SurfaceDirection)direction, u + 1, v + 1, t, flip, interior, edges);

            List<Curve> outputs = new List<Curve>();
            switch ((BoundaryTypes)type)
            {
                default:
                    outputs = grid.RenderToPolygonBoundaries();
                    break;
                case BoundaryTypes.Interpolated:
                    outputs = grid.RenderToInterpolatedBoundaries();
                    break;
                case BoundaryTypes.Geodesic:
                    outputs = grid.RenderToGeodesicBoundaries();
                    break;
            }

            DA.SetDataList(0, outputs);
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
            get { return new Guid("5687e6e4-58f6-4d5f-94cd-236ed7cf61e8"); }
        }
    }
}