using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_Cells_Quad_Alternate : GH_Cells__BaseIso
    {
        /// <summary>
        /// Initializes a new instance of the GH_Cells_Quad_Alternate class.
        /// </summary>
        public GH_Cells_Quad_Alternate()
          : base("Quad Alternating Curves", "Alt Quad Crvs",
              "Divide a surface into a series of quad curves shifted by a value every other row",
              Constants.CatCurve, Constants.SubGrid)
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
            pManager.AddNumberParameter("Parameter", "P", "A shift parameter. If no values are provided a default of 0.5 will be used", GH_ParamAccess.item);
            pManager[5].Optional = true;
            pManager.AddBooleanParameter("Flip", "F", "If true the alternating value will be shifted by one row", GH_ParamAccess.item, false);
            pManager[6].Optional = true;
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

            double p = 0.5;
            DA.GetData(5, ref p);

            List<double> t = new List<double> { 0 };

            bool flip = false;
            DA.GetData(6, ref flip);
            if (flip)
            {
                t.Insert(0, p);
            }
            else
            {
                t.Add(p);
            }

            Grid grid = new Grid(surface);
            grid.SetStaggeredQuads((SurfaceDirection)direction, u, v,t);

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
                case BoundaryTypes.Iso:
                    outputs = grid.RenderToIsoBoundaries();
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
            get { return new Guid("f7643df6-5ff3-43db-ac01-12dad74fd158"); }
        }
    }
}