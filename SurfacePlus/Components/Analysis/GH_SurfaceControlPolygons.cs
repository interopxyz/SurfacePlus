using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_SurfaceControlPolygons : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_SurfaceHulls class.
        /// </summary>
        public GH_SurfaceControlPolygons()
          : base("Surface Control Polygons", "Surface Ctrl Pgon",
              "The control point polygon in teh U or V direction",
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
            pManager.AddCurveParameter("Polylines", "P", "Control Polygons in the selected direction", GH_ParamAccess.list);
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

            int u = surface1.Points.CountU;
            int v = surface1.Points.CountV;

            List<Polyline> polylines = new List<Polyline>();

            if (direction == 0)
            {
                for (int i = 0; i < u; i++)
                {
                    Polyline polyline = new Polyline();
                    for (int j = 0; j < v; j++)
                    {
                        polyline.Add(surface1.Points.GetControlPoint(i, j).Location);
                    }
                    polylines.Add(polyline);
                }
            }

            if (direction == 1)
            {
                for (int i = 0; i < v; i++)
                {
                    Polyline polyline = new Polyline();
                    for (int j = 0; j < u; j++)
                    {
                        polyline.Add(surface1.Points.GetControlPoint(j, i).Location);
                    }
                    polylines.Add(polyline);
                }
            }

            DA.SetDataList(0, polylines);
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
            get { return new Guid("04ac1933-2ded-47df-9058-d8b56eeef837"); }
        }
    }
}