using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SurfacePlus.Components
{
    public class GH_SurfaceSpans : GH_Component
    {
        List<Curve> prev_crvs = new List<Curve>();

        /// <summary>
        /// Initializes a new instance of the SurfaceSpans class.
        /// </summary>
        public GH_SurfaceSpans()
          : base("Surface Spans", "Spans",
              "Span count in the U and V direction",
              Constants.CatSurface, Constants.SubAnalysis)
        {
        }

        protected override void BeforeSolveInstance()
        {
            prev_crvs = new List<Curve>();
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary | GH_Exposure.obscure; }
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
            pManager.AddIntegerParameter("Count", "C", "Span count in the selected direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Parameters", "P", "Span vector parameter values in the selected direction", GH_ParamAccess.list);
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

            int count = surface1.SpanCount(direction);
            List<double> parameters = surface1.GetSpanVector(direction).ToList();
            List<Interval> intervals = new List<Interval>();

            for (int i = 0; i < parameters.Count-1; i++)
            {
                intervals.Add(new Interval(parameters[i],parameters[i+1]));
                prev_crvs.Add(surface1.IsoCurve(1-direction, parameters[i]));
            }
            prev_crvs.Add(surface1.IsoCurve(1-direction, parameters[count]));

            DA.SetData(0, count);
            DA.SetDataList(1, parameters);
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
                return Properties.Resources.Sp_Ana_Srf_Spans;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b8fce49f-df0f-49de-924e-c51fc7386bb2"); }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            if (Hidden) return;
            if (Locked) return;
            Transform xform = args.Viewport.GetTransform(Rhino.DocObjects.CoordinateSystem.World, Rhino.DocObjects.CoordinateSystem.Screen);

            Rhino.Display.DisplayMaterial mat = new Rhino.Display.DisplayMaterial();
            if (Attributes.Selected)
            {
                mat = args.ShadeMaterial_Selected;
            }
            else
            {
                mat = args.ShadeMaterial;
            }

            Color activeColor = mat.Diffuse;

            foreach (Curve crv in prev_crvs)
            {
                args.Display.DrawCurve(crv, activeColor);
            }

            base.DrawViewportMeshes(args);
        }
    }
}