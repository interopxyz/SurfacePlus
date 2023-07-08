using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public abstract class GH_Cells__BaseType : GH_Cells__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Cells__BaseType class.
        /// </summary>
        public GH_Cells__BaseType()
          : base("Panel__Base", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        public GH_Cells__BaseType(string Name, string NickName, string Description, string Category, string Subcategory) : base(Name, NickName, Description, Category, Subcategory)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("Boundary Types", "T", "Geometry type of the resulting boundaries", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;


            Param_Integer paramA = (Param_Integer)pManager[1];
            foreach (BoundaryTypes value in Enum.GetValues(typeof(BoundaryTypes)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }
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
            get { return new Guid("75b9c307-6ce0-418f-b932-779287c09b84"); }
        }
    }
}