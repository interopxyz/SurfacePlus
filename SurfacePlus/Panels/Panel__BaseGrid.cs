using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Panels
{
    public abstract class Panel__BaseGrid : Panel__BaseType
    {
        /// <summary>
        /// Initializes a new instance of the Panel__BaseGrid class.
        /// </summary>
        public Panel__BaseGrid()
          : base("Panel__BaseGrid", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        public Panel__BaseGrid(string Name, string NickName, string Description, string Category, string Subcategory) : base(Name, NickName, Description, Category, Subcategory)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("U Divisions", "U", "Division count in the U direction", GH_ParamAccess.item, 4);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("V Divisions", "V", "Division count in the U direction", GH_ParamAccess.item, 4);
            pManager[3].Optional = true;
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
            get { return new Guid("a7ccf4f8-3926-46e0-99d0-33e325044004"); }
        }
    }
}