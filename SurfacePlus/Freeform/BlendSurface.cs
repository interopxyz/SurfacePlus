using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurfacePlus.Freeform
{
    public class BlendSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public BlendSurface()
          : base("Blend Surface", "Blend Srf",
              "Description",
              Constants.CatSurface, Constants.SubFreeform)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Start Brep", "B0", "The starting brep of the blend", GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddIntegerParameter("Start Edge", "E0", "The starting edge index from the start Brep", GH_ParamAccess.item, 0);
            pManager[1].Optional = false;
            pManager.AddIntervalParameter("Start Domain", "D0", "The starting edge sub domain", GH_ParamAccess.item, new Interval());
            pManager[2].Optional = false;
            pManager.AddIntegerParameter("Start Type", "T0", "The start ege blend type", GH_ParamAccess.item, 2);
            pManager[3].Optional = false;


            pManager.AddBrepParameter("End Brep", "B1", "The ending brep of the blend", GH_ParamAccess.item);
            pManager[4].Optional = false;
            pManager.AddIntegerParameter("End Edge", "E1", "The ending edge index from the end Brep", GH_ParamAccess.item, 0);
            pManager[5].Optional = false;
            pManager.AddIntervalParameter("End Domain", "D1", "The ending edge sub domain", GH_ParamAccess.item, new Interval());
            pManager[6].Optional = false;
            pManager.AddIntegerParameter("End Type", "T1", "The end edge blend type", GH_ParamAccess.item, 2);
            pManager[7].Optional = false;

            Param_Integer paramA = (Param_Integer)pManager[3];
            foreach (BlendContinuity value in Enum.GetValues(typeof(BlendContinuity)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }

            Param_Integer paramB = (Param_Integer)pManager[7];
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
            pManager.AddBrepParameter("Breps", "B", "The resulting Breps", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep brepA = null;
            if (!DA.GetData(0, ref brepA)) return;

            Brep brepB = null;
            if (!DA.GetData(4, ref brepB)) return;

            int indexA = 0;
            DA.GetData(1, ref indexA);

            int indexB = 0;
            DA.GetData(5, ref indexB);

            Interval domainA = new Interval(0, 1);
            DA.GetData(2, ref domainA);

            Interval domainB = new Interval(0, 1);
            DA.GetData(6, ref domainB);

            int typeA = 2;
            DA.GetData(3, ref typeA);

            int typeB = 2;
            DA.GetData(7, ref typeA);

            BrepEdge edgeA = brepA.Edges[indexA];
            BrepFace faceA = brepA.Faces[edgeA.AdjacentFaces()[0]];
            Interval domA = domainA.Remap(edgeA.Domain);

            BrepEdge edgeB = brepB.Edges[indexB];
            BrepFace faceB = brepB.Faces[edgeB.AdjacentFaces()[0]];
            Interval domB = domainB.Remap(edgeB.Domain);

            List<Brep> breps = Brep.CreateBlendSurface(faceA, edgeA,domA, false, (BlendContinuity)typeA, faceB, edgeB,domB, false, (BlendContinuity)typeB).ToList();

            DA.SetDataList(0, breps);
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
            get { return new Guid("bbce9a81-8569-401c-9a59-75426b5b25d4"); }
        }
    }
}