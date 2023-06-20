using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Panels
{
    public class Panel_Strips : Panel__BaseStrips
    {
        /// <summary>
        /// Initializes a new instance of the Panel_Strips class.
        /// </summary>
        public Panel_Strips()
          : base("Panel Strips", "Strips",
              "Divide a surface into a series of Strips",
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

            int count = 4;
            DA.GetData(3, ref count);

            switch ((PanelTypes)type)
            {
                case PanelTypes.Corner:
                    DA.SetDataList(0, surface1.CornerStrips((SurfaceDirection)direction, count));
                    break;
                case PanelTypes.Loft:
                    DA.SetDataList(0, surface1.LoftStrips((SurfaceDirection)direction, count));
                    break;
                case PanelTypes.Edge:
                    DA.SetDataList(0, surface1.EdgeStrips((SurfaceDirection)direction, count));
                    break;
                case PanelTypes.Split:
            DA.SetDataList(0, surface1.SplitStrips((SurfaceDirection)direction, count));
                    break;
            }
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
            get { return new Guid("73d78964-167f-4006-bf8e-fd008071a8f9"); }
        }
    }
}