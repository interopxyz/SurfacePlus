using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Analysis
{
    public class GH_SurfaceSpans : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SurfaceSpans class.
        /// </summary>
        public GH_SurfaceSpans()
          : base("Surface Spans", "Spans",
              "Span count in the U and V direction",
              Constants.CatSurface, Constants.SubAnalysis)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddIntegerParameter("Direction", "D", "Select either the U or V direction", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            paramA.AddNamedValue("U", 0);
            paramA.AddNamedValue("V", 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Count", "U", "Span count in the selected direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Vectors", "V", "Span vector parameter values in the selected direction", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Domains", "D", "Span domains values in the selected direction", GH_ParamAccess.list);
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

            List<Interval> intervals = new List<Interval>();

            NurbsCurve nurbsCurve = surface1.IsoCurve(direction, surface1.Domain(direction).T0).ToNurbsCurve();

            for (int i = 0; i < nurbsCurve.SpanCount; i++)
            {
                intervals.Add(nurbsCurve.SpanDomain(i));
            }

            DA.SetData(0, surface1.SpanCount(direction));
            DA.SetDataList(1, surface1.GetSpanVector(direction));
            DA.SetDataList(2, intervals);
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
            get { return new Guid("b8fce49f-df0f-49de-924e-c51fc7386bb2"); }
        }
    }
}