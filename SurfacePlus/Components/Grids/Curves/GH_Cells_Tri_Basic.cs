using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_Cells_Tri_Basic : GH_Cells__BaseGrid
    {
        /// <summary>
        /// Initializes a new instance of the GH_Cells_Tri_Basic class.
        /// </summary>
        public GH_Cells_Tri_Basic()
          : base("Triangle Pattern Curves", "Tri Patt Crvs",
              "Divide a surface into a series of triangular curves from basic patterns of a UV quad grid",
              Constants.CatCurve, Constants.SubGrid)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("Type", "T", "The triangulation type"
                + Environment.NewLine + "0: Simple"
                + Environment.NewLine + "1: Wave"
                + Environment.NewLine + "2: Cross"
                + Environment.NewLine + "3: Rings"
                + Environment.NewLine + "4: Length"
                + Environment.NewLine + "5: Area"
                , GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Flip", "F", "Flip the orientation of the triangulation panel", GH_ParamAccess.item, false);
            pManager[6].Optional = true;

            Param_Integer param = (Param_Integer)pManager[5];
            param.AddNamedValue("Simple", 0);
            param.AddNamedValue("Wave", 1);
            param.AddNamedValue("Cross", 2);
            param.AddNamedValue("Rings", 3);
            param.AddNamedValue("Length", 4);
            param.AddNamedValue("Area", 5);
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

            int pattern = 0;
            DA.GetData(5, ref pattern);

            bool flip = false;
            DA.GetData(6, ref flip);

            Grid grid = new Grid(surface);
            switch (pattern)
            {
                default:
                    grid.SetBasicTriangles((SurfaceDirection)direction, u, v, flip);
                    break;
                case 1:
                    grid.SetWaveTriangles((SurfaceDirection)direction, u, v, flip);
                    break;
                case 2:
                    grid.SetCrossTriangles((SurfaceDirection)direction, u, v, flip);
                    break;
                case 3:
                    grid.SetRingTriangles((SurfaceDirection)direction, u, v, flip);
                    break;
                case 4:
                    grid.SetLengthTriangles((SurfaceDirection)direction, u, v, flip);
                    break;
                case 5:
                    grid.SetAreaTriangles((SurfaceDirection)direction, u, v, flip);
                    break;
            }

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
                return Properties.Resources.Sp_Grid_Tri_Pattern;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8a8e0e03-a472-40d5-8285-4036f296cfaa"); }
        }
    }
}