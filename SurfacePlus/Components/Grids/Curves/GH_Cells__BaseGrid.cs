using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public abstract class GH_Cells__BaseGrid : GH_Cells__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Cells__BaseGrid class.
        /// </summary>
        public GH_Cells__BaseGrid()
          : base("Panel__BaseGrid", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        public GH_Cells__BaseGrid(string Name, string NickName, string Description, string Category, string Subcategory) : base(Name, NickName, Description, Category, Subcategory)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("Panel Types", "T", "Geometry type of the resulting panels", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;

            pManager.AddIntegerParameter("Direction", "D", "The direction of the primary division", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("U Divisions", "U", "Division count in the U direction", GH_ParamAccess.item, 4);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("V Divisions", "V", "Division count in the U direction", GH_ParamAccess.item, 4);
            pManager[4].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            paramA.AddNamedValue(BoundaryTypes.Polygon.ToString(), (int)BoundaryTypes.Polygon);
            paramA.AddNamedValue(BoundaryTypes.Interpolated.ToString(), (int)BoundaryTypes.Interpolated);
            paramA.AddNamedValue(BoundaryTypes.Geodesic.ToString(), (int)BoundaryTypes.Geodesic);

            Param_Integer paramB = (Param_Integer)pManager[2];
            paramB.AddNamedValue("U", 0);
            paramB.AddNamedValue("V", 1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
            get { return new Guid("c22803dd-3a88-4dc8-9f0e-3b416785d0d2"); }
        }
    }
}