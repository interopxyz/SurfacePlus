using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components.Panels
{
    public class GH_Panel_Hexagon : GH_Panel__BaseTri
    {
        /// <summary>
        /// Initializes a new instance of the GH_Panel_Hexagon class.
        /// </summary>
        public GH_Panel_Hexagon()
          : base("Hexagon Surfaces", "Hexagon",
              "Divide a surface into a series of hexagon surfaces",
              Constants.CatSurface, Constants.SubGrid)
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
            pManager[5].Optional = true;
            pManager.AddIntegerParameter("Edges", "E", "Edge filtering mode", GH_ParamAccess.item, 0);
            pManager[6].Optional = true;

            Param_Integer param = (Param_Integer)pManager[6];
            param.AddNamedValue("All", 0);
            param.AddNamedValue("Interior", 1);
            param.AddNamedValue("Edges", 2);

            //pManager.AddIntegerParameter("Type", "T", "The triangulation type"
            //    + Environment.NewLine + "0: Triangular"
            //    + Environment.NewLine + "1: Split 1"
            //    + Environment.NewLine + "2: Split 2"
            //    + Environment.NewLine + "3: Split 3"
            //    + Environment.NewLine + "4: Radial 1"
            //    + Environment.NewLine + "5: Radial 2"
            //    + Environment.NewLine + "6: Radial 3"
            //    , GH_ParamAccess.item, 0);
            //pManager[6].Optional = true;

            //Param_Integer param = (Param_Integer)pManager[6];
            //param.AddNamedValue("Triangular", 0);
            //param.AddNamedValue("Split 1", 1);
            //param.AddNamedValue("Split 2", 2);
            //param.AddNamedValue("Split 3", 3);
            //param.AddNamedValue("Radial 1", 4);
            //param.AddNamedValue("Radial 2", 5);
            //param.AddNamedValue("Radial 3", 6);
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

            int edgeType = 0;
            DA.GetData(6, ref edgeType);

            bool edges = true;
            bool interior = true;
            if (edgeType == 1) edges = false;
            if (edgeType == 2) interior = false;

            Grid grid = new Grid(surface);
            grid.SetHexQuads((SurfaceDirection)direction, u + 1, v + 1, t, flip,interior,edges);

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
            get { return new Guid("65e9b8b5-ab3d-4c41-8458-4d83e967fe2e"); }
        }
    }
}