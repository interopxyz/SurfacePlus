using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public abstract class GH_Cells__Base : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Cells__Base class.
        /// </summary>
        public GH_Cells__Base()
          : base("GH_Panel_Base", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        public GH_Cells__Base(string Name, string NickName, string Description, string Category, string Subcategory) : base(Name, NickName, Description, Category, Subcategory)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter(Constants.Curve.Name, Constants.Curve.NickName, Constants.Curve.Outputs, GH_ParamAccess.list);
            pManager.AddCurveParameter("UV Polylines", "P", "The unitized surface UV coordinates of the cells as a hidden polyline", GH_ParamAccess.list);
            pManager.HideParameter(1);

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
            get { return new Guid("f0a21382-853e-42d5-af34-7de615d40e98"); }
        }
    }
}