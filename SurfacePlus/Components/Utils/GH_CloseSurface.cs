using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurfacePlus.Utils
{
    public class GH_CloseSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CloseSurface class.
        /// </summary>
        public GH_CloseSurface()
          : base("Close Surface", "Close",
              "Closes a Surface in the U or V direction",
              Constants.CatSurface, Constants.SubUtilities)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager.AddIntegerParameter("Direction", "D", "Set the U or V direction", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Blend Type", "T", "The closed surface blend type", GH_ParamAccess.item, 2);
            pManager[2].Optional = false;

            Param_Integer paramA = (Param_Integer)pManager[1];
            paramA.AddNamedValue("U", 0);
            paramA.AddNamedValue("V", 1);

            Param_Integer paramB = (Param_Integer)pManager[2];
            foreach (BlendContinuity value in Enum.GetValues(typeof(BlendContinuity)))
            {
                paramB.AddNamedValue(value.ToString(), (int)value);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "The resulting Brep", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            if (!DA.GetData(0, ref surface)) return;

            Brep brep = Brep.CreateFromSurface(surface);

            int direction = 0;
            DA.GetData(1, ref direction);

            int type = 2;
            DA.GetData(2, ref type);

            List<Brep> breps = new List<Brep>();
            if(direction == 1)
            {
                breps = Brep.CreateBlendSurface(brep.Faces[0], brep.Edges[0], brep.Edges[0].Domain, false, (BlendContinuity)type, brep.Faces[0], brep.Edges[2], brep.Edges[2].Domain, true, (BlendContinuity)type).ToList();
            }
            else
            {
                breps = Brep.CreateBlendSurface(brep.Faces[0], brep.Edges[1], brep.Edges[1].Domain, false, (BlendContinuity)type, brep.Faces[0], brep.Edges[3], brep.Edges[3].Domain, true, (BlendContinuity)type).ToList();
            }
            breps.Add(brep);

            Brep brep1 = Brep.JoinBreps(breps, 0.001)[0];

            DA.SetData(0, brep1);
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
            get { return new Guid("583188d7-34f5-40f9-8a78-3315a0a42f71"); }
        }
    }
}