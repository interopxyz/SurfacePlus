using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_Panel_Quad_Rnd_Loose : GH_Panel__BaseGrid
    {
        /// <summary>
        /// Initializes a new instance of the GH_Panel_Quad_Rnd_Loose class.
        /// </summary>
        public GH_Panel_Quad_Rnd_Loose()
          : base("Random Parameter Quad Surfaces", "Rnd Quads",
              "Divide a surface into a fixed number of randomly subdivided quads per row",
              Constants.CatSurface, Constants.SubGrid)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("Seed", "S", "The random seed", GH_ParamAccess.item);
            pManager[5].Optional = true;
            pManager.AddIntervalParameter("Domain", "D", "The domain of the divisions", GH_ParamAccess.item, new Interval(0.25, 0.75));
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

            int s = 1;
            DA.GetData(5, ref s);

            Interval d = new Interval(0.25,0.75);
            DA.GetData(6, ref d);


            Grid grid = new Grid(surface);
            grid.SetRandomQuads((SurfaceDirection)direction, u, v, s,d);

            List<Surface> surfaces = new List<Surface>();
            switch ((PanelTypes)type)
            {
                default:
                    surfaces.AddRange(grid.RenderToFacets());
                    break;
                case PanelTypes.Loft:
                    surfaces.AddRange(grid.RenderToLoftedSurfaces());
                    break;
                case PanelTypes.Iso:
                    surfaces.AddRange(grid.RenderToIsoSurfaces());
                    break;
            }

            DA.SetDataList(0, surfaces);
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
                return Properties.Resources.Sp_Panel_Quad_Rnd_Param;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("25d5c337-9569-4a6f-8c17-6eafb38f9cd1"); }
        }
    }
}