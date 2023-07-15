using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_BaseballSphere : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BaseballSphere class.
        /// </summary>
        public GH_BaseballSphere()
          : base("Baseball Sphere", "Baseball",
                "A Brep Sphere made of two Surfaces",
              Constants.CatSurface, Constants.SubPrimitive)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Center", "P", "The center of the sphere", GH_ParamAccess.item, new Point3d(0,0,0));
            pManager[0].Optional = true;
            pManager.AddNumberParameter("Radius", "R", "The radius of the sphere", GH_ParamAccess.item, 1.0);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Tolerance", "T", "The radius of the sphere", GH_ParamAccess.item, 0.001);
            pManager[2].Optional = true;

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
            Point3d point = new Point3d(0,0,0);
            if (!DA.GetData(0, ref point)) return;

            double radius = 1.0;
            DA.GetData(1, ref radius);
            
            double tolerance = 0.001;
            DA.GetData(2, ref tolerance);
            
            Brep brep = Brep.CreateBaseballSphere(point, radius, tolerance);
            
            DA.SetData(0, brep);
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
                return Properties.Resources.Sp_Primative_Sphere_Baseball;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("44e1326d-d08e-48c7-9c5a-4aeb764687f2"); }
        }
    }
}