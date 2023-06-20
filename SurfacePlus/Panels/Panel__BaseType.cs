using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Panels
{
    public abstract class Panel__BaseType : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Panel__Base class.
        /// </summary>
        public Panel__BaseType()
          : base("Panel__Base", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        public Panel__BaseType(string Name, string NickName, string Description, string Category, string Subcategory) : base(Name, NickName, Description, Category, Subcategory)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddIntegerParameter("Panel Types", "T", "Geometry type of the resulting panels", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;


            Param_Integer paramA = (Param_Integer)pManager[1];
            foreach (PanelTypes value in Enum.GetValues(typeof(PanelTypes)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Outputs, GH_ParamAccess.list);
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
            get { return new Guid("84687cb0-e8ef-42c4-b1a8-7f68f0705b89"); }
        }
    }
}