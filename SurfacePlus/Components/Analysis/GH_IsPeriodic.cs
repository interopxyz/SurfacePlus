using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_IsPeriodic : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MakePeriodic class.
        /// </summary>
        public GH_IsPeriodic()
          : base("Is Periodic", "Periodic",
              "Get or set if a surface periodic in one direction",
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
            pManager.AddIntegerParameter("Direction", "D", "Set the U or V direction", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager.AddBooleanParameter("Smooth", "S", "If true, the resulting surfaces with be smoothed", GH_ParamAccess.item);
            pManager[2].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            paramA.AddNamedValue("U", 0);
            paramA.AddNamedValue("V", 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Outputs, GH_ParamAccess.list);
            pManager.AddBooleanParameter("Status", "S", "The status of the suface", GH_ParamAccess.item);
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

            bool smooth = true;
            bool isActive = DA.GetData(2, ref smooth);

            Surface surface2 = surface1;
            if(isActive)surface2 = NurbsSurface.CreatePeriodicSurface(surface1, direction, smooth);
            
            DA.SetData(0, surface2);
            if(surface2!=null)DA.SetData(1, surface2.IsPeriodic(direction));
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
                return Properties.Resources.Sp_Ana_IsPeriodic;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6b49f9f2-8add-474d-a856-48853200333c"); }
        }
    }
}