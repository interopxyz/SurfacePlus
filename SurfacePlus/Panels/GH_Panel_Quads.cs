using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Panels
{
    public class GH_Panel_Quads : GH_Panel__BaseGrid
    {
        /// <summary>
        /// Initializes a new instance of the GH_Panels_Quads class.
        /// </summary>
        public GH_Panel_Quads()
          : base("Panel Quads", "Quads",
              "Divide a surface into a series of Quads",
              Constants.CatSurface, Constants.SubSubdivide)
        {
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

            int reverse = 1 - direction;
            List<Surface> surfaces= new List<Surface>();
            List<Surface> outputs = new List<Surface>();
            switch ((PanelTypes)type)
            {
                case PanelTypes.Corner:
                    surfaces = surface1.CornerQuads((SurfaceDirection)direction, u,v);
                    break;
                case PanelTypes.Loft:
                    surfaces = surface1.LoftQuads((SurfaceDirection)direction, u,v);
                    break;
                case PanelTypes.Edge:
                    surfaces = surface1.EdgeStrips((SurfaceDirection)direction, u);
                    foreach (Surface srf in surfaces) outputs.AddRange(srf.EdgeStrips((SurfaceDirection)reverse, v));
                    break;
                case PanelTypes.Split:
                    surfaces = surface1.SplitStrips((SurfaceDirection)direction, u);
                    foreach (Surface srf in surfaces) outputs.AddRange(srf.SplitStrips((SurfaceDirection)reverse, v));
                    break;
            }
            DA.SetDataList(0, surfaces);
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
            get { return new Guid("cafcf875-8db6-4098-b547-dff1b86d81b0"); }
        }
    }
}