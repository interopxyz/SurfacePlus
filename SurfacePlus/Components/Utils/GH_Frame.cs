using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_Frame : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Frame class.
        /// </summary>
        public GH_Frame()
          : base("Frame Surface", "Frame",
              "Creates a trimmed frame Brep from a Surface",
              Constants.CatSurface, Constants.SubUtilities)
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
            pManager.AddNumberParameter("Parameter", "P", "A unitized parameter between 0.0-1.0", GH_ParamAccess.item, 0.25);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter(Constants.Brep.Name, Constants.Brep.NickName, Constants.Brep.Output, GH_ParamAccess.item);
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
            surface1.SetDomain(0, new Interval(0, 1));
            surface1.SetDomain(1, new Interval(0, 1));

            double t = 0.25;
            DA.GetData(1, ref t);

            if(t<=0)
            {
                DA.SetData(0, surface1);
                return;
            }
            if (t >= 1)
            {
                DA.SetData(0, null);
                return;
            }
            t = 0.5-t / 2;
            List<Curve> curves = new List<Curve>();
            curves.Add(new Rectangle3d(Plane.WorldXY, new Point3d(-0.5, -0.5,0), new Point3d(0.5, 0.5,0)).ToNurbsCurve());
            curves.Add(new Rectangle3d(Plane.WorldXY, new Point3d(-t, -t, 0), new Point3d(t, t, 0)).ToNurbsCurve());
            Brep brep = Brep.CreatePlanarBreps(curves, 0.01)[0];
            brep.Surfaces[0].SetDomain(0, new Interval(0, 1));
            brep.Surfaces[0].SetDomain(1, new Interval(0, 1));

            Brep brep2 = Brep.CopyTrimCurves(brep.Faces[0], surface1, 1);
            
            DA.SetData(0, brep2);
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
            get { return new Guid("1ab0066f-c511-43c1-a632-7dea6fc33a68"); }
        }
    }
}