using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_ExtendCrvSrf : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ExtendCrvSrf class.
        /// </summary>
        public GH_ExtendCrvSrf()
          : base("Extend Curve on Surface", "Ext Crv Srf",
              "Extend a Curve on a Surface",
              Constants.CatCurve, "Util")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddCurveParameter("Curve", "C", "The Curve to offset from the surface", GH_ParamAccess.item);
            pManager[1].Optional = false;
            pManager.AddIntegerParameter("End", "E", "The extenion end direction", GH_ParamAccess.item, 0);
            pManager[2].Optional = false;

            Param_Integer paramA = (Param_Integer)pManager[2];
            foreach (CurveEnd value in Enum.GetValues(typeof(CurveEnd)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "The resulting extended curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            if (!DA.GetData(0, ref surface)) return;

            Curve curve = null;
            if (!DA.GetData(1, ref curve)) return;

            NurbsCurve curve1 = curve.ToNurbsCurve();

            int direction = 0;
            DA.GetData(2, ref direction);


            Curve curve2 = curve1;
                if(direction>0) curve2= curve1.ExtendOnSurface((CurveEnd)direction,surface);

            DA.SetData(0, curve2);
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
                return Properties.Resources.Sp_Crv_Extend;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8acd029d-2972-473e-9de3-f952e40811f5"); }
        }
    }
}