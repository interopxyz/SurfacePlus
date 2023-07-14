using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SurfacePlus.Components
{
    public class GH_IsoEdges : GH_Component
    {
        protected List<Surface> prev_srfs = new List<Surface>();

        /// <summary>
        /// Initializes a new instance of the GH_IsoEdges class.
        /// </summary>
        public GH_IsoEdges()
          : base("Iso Edges", "Iso Edges",
              "Extract the Iso Edges",
              Constants.CatSurface, Constants.SubAnalysis)
        {
        }

        protected override void BeforeSolveInstance()
        {
            prev_srfs = new List<Surface>();
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

            prev_srfs.Add(surface2);

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
                return Properties.Resources.Sp_Ana_Iso;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("60607eee-7d01-4524-8ba6-68ecf836b240"); }
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

            foreach (Surface srf in prev_srfs)
            {
                Point3d ps = srf.PointAt(0.5, 0);
                ps.Transform(xform);
                args.Display.Draw2dText("South", activeColor, new Point2d(ps.X, ps.Y), true);

                Point3d pe = srf.PointAt(1, 0.5);
                pe.Transform(xform);
                args.Display.Draw2dText("East", activeColor, new Point2d(pe.X, pe.Y), true);

                Point3d pn = srf.PointAt(0.5, 1);
                pn.Transform(xform);
                args.Display.Draw2dText("North", activeColor, new Point2d(pn.X, pn.Y), true);

                Point3d pw = srf.PointAt(0, 0.5);
                pw.Transform(xform);
                args.Display.Draw2dText("West", activeColor, new Point2d(pw.X, pw.Y), true);
            }

            base.DrawViewportMeshes(args);
        }
    }
}