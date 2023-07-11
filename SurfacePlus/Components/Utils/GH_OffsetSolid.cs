using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_OffsetSolid : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the OffsetSolid class.
        /// </summary>
        public GH_OffsetSolid()
          : base("Offset Solid", "Solid",
              "Offsets a solid brep normal to a surface",
              Constants.CatSurface, Constants.SubUtilities)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter(Constants.Brep.Name, Constants.Brep.NickName, Constants.Brep.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddNumberParameter("Distance", "D", "The offset distance", GH_ParamAccess.item, 1.0);
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
            Brep brep = new Brep();
            if (!DA.GetData(0, ref brep)) return;

            double d = 1.0;
            DA.GetData(1, ref d);

            Brep[] breps = Brep.CreateOffsetBrep(brep, d, true, true, 0.001, out Brep[] blends, out Brep[] walls);

            DA.SetData(0, breps[0]);
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
            get { return new Guid("a745b6a0-07e4-43a4-8391-1c253d7c895e"); }
        }
    }
}