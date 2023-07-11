using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_Divide_Span : GH_Divide__Base
    {
        /// <summary>
        /// Initializes a new instance of the DivideSpan class.
        /// </summary>
        public GH_Divide_Span()
          : base("Divide Span", "Div Span",
              "Divides a surface in the U or V direction by an aproximated span length at a parameter.",
              Constants.CatSurface, Constants.SubDivide)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddNumberParameter("Parameter", "P", "The unitized surface parameter for division", GH_ParamAccess.item, 0.5);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Span Length", "S", "The target span length", GH_ParamAccess.item, 1.0);
            pManager[3].Optional = true;
            pManager.AddBooleanParameter("Below", "B", "If true, the span length will always stay below the length, if false, above", GH_ParamAccess.item, false);
            pManager[4].Optional = true;
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
            
            int direction = 0;
            DA.GetData(1, ref direction);

            double t = 0.5;
            DA.GetData(2, ref t);

            double d = 1.0;
            DA.GetData(3, ref d);

            bool isBelow = false;
            DA.GetData(4, ref isBelow);

            DA.SetDataList(0, surface1.DivideSpan((SurfaceDirection)direction, t,d, isBelow));
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
            get { return new Guid("c18f0fdf-3f53-49a1-9add-a1959f7d435b"); }
        }
    }
}