using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_Brep_Surfaces : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Brep_Surfaces class.
        /// </summary>
        public GH_Brep_Surfaces()
          : base("Brep Surfaces", "Brep Srfs",
              "Gets the untrimmed surfaces of each brep face",
              Constants.CatSurface, Constants.SubAnalysis)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter(Constants.Brep.Name, Constants.Brep.NickName, Constants.Brep.Input, GH_ParamAccess.list);
            pManager[0].Optional = false;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Outputs, GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep brep = new Brep();
            if (!DA.GetData(0, ref brep)) return;

            List<Surface> surfaces = new List<Surface>();

            for(int i =0;i<brep.Faces.Count;i++)
            {
                surfaces.Add(brep.Faces[i].UnderlyingSurface());
            }

            DA.SetDataList(0, surfaces);
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
            get { return new Guid("547b5f17-e458-4399-baf6-f41653741a30"); }
        }
    }
}