using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public abstract class GH_Cells__BaseIso : GH_Cells__BaseType
    {
        /// <summary>
        /// Initializes a new instance of the GH_Cells__BaseTri class.
        /// </summary>
        public GH_Cells__BaseIso()
          : base("GH_Panel__BaseTri", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        public GH_Cells__BaseIso(string Name, string NickName, string Description, string Category, string Subcategory) : base(Name, NickName, Description, Category, Subcategory)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("Direction", "D", "The direction of the primary division", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("U Divisions", "U", "Division count in the U direction", GH_ParamAccess.item, 4);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("V Divisions", "V", "Division count in the U direction", GH_ParamAccess.item, 4);
            pManager[4].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[2];
            paramA.AddNamedValue("U", 0);
            paramA.AddNamedValue("V", 1);
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
            get { return new Guid("01f8483a-abde-47db-8a40-5c3da7f10673"); }
        }
    }
}