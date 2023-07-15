using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_IsSingular : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_IsSingular class.
        /// </summary>
        public GH_IsSingular()
          : base("Is Singular", "Singular",
              "Test if a surfaces edge length has been collapsed to 0",
              Constants.CatSurface, Constants.SubAnalysis)
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
            pManager.AddIntegerParameter("Direction", "D", "The direction to test."
                + Environment.NewLine + "-1 = Any"
                + Environment.NewLine + " 0 = South"
                + Environment.NewLine + " 1 = East"
                + Environment.NewLine + " 2 = North"
                + Environment.NewLine + " 3 = West", GH_ParamAccess.item, -1);
            pManager[1].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            paramA.AddNamedValue("Any", -1);
            paramA.AddNamedValue("South", 0);
            paramA.AddNamedValue("East", 1);
            paramA.AddNamedValue("North", 2);
            paramA.AddNamedValue("West", 3);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Is Singular", "S", "True if the edge is collapsed.", GH_ParamAccess.item);
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

            int direction = -1;
            DA.GetData(1, ref direction);

            bool isSingular = false;
            if (direction == -1)
            {
                bool south = surface1.IsSingular(0);
                if (south) isSingular = true;
                bool east = surface1.IsSingular(1);
                if (east) isSingular = true;
                bool north = surface1.IsSingular(2);
                if (north) isSingular = true;
                bool west = surface1.IsSingular(3);
                if (west) isSingular = true;

                DA.SetData(0, isSingular);
            }
            else
            {
                if (direction == 0) isSingular = surface1.IsSingular(direction);
                if (direction == 1) isSingular = surface1.IsSingular(direction);
                if (direction == 2) isSingular = surface1.IsSingular(direction);
                if (direction == 3) isSingular = surface1.IsSingular(direction);

                DA.SetData(0, isSingular);
            }

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
                return Properties.Resources.Sp_Ana_IsSingle;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0f0d9740-0e2d-437e-80ff-44b22e491af1"); }
        }
    }
}