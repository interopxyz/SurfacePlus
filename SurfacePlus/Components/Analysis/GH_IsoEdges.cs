using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components.Analysis
{
    public class GH_IsoEdges : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_IsoEdges class.
        /// </summary>
        public GH_IsoEdges()
          : base("Iso Edges", "Iso Edges",
              "Extract the Iso Edges",
              Constants.CatSurface, Constants.SubAnalysis)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }
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
            pManager.AddCurveParameter("South", "S", "The south edge", GH_ParamAccess.item);
            pManager.AddCurveParameter("East", "E", "The east edge", GH_ParamAccess.item);
            pManager.AddCurveParameter("North", "N", "The north edge", GH_ParamAccess.item);
            pManager.AddCurveParameter("West", "W", "The west edge", GH_ParamAccess.item);
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
            NurbsSurface surface2 = surface.ToNurbsSurface();
            surface2.SetDomain(0, new Interval(0, 1));
            surface2.SetDomain(1, new Interval(0, 1));

            DA.SetData(0,surface2.IsoCurve(0, 0));
            DA.SetData(1,surface2.IsoCurve(1, 1));
            DA.SetData(2,surface2.IsoCurve(0, 1));
            DA.SetData(3,surface2.IsoCurve(1, 0));
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
            get { return new Guid("60607eee-7d01-4524-8ba6-68ecf836b240"); }
        }
    }
}