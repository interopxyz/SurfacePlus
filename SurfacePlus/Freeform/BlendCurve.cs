using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Freeform
{
    public class BlendCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BlendCurve class.
        /// </summary>
        public BlendCurve()
          : base("Blend Curve", "Blend Crv",
              "Blends a curve between two breps",
              Constants.CatCurve, "Spline")
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
            pManager.AddNumberParameter("Start Parameter", "P0", "The starting edge parameter", GH_ParamAccess.item, 0.5);
            pManager[2].Optional = false;
            pManager.AddIntegerParameter("Start Type", "T0", "The start ege blend type", GH_ParamAccess.item, 2);
            pManager[3].Optional = false;


            pManager.AddBrepParameter("End Brep", "B1", "The ending brep of the blend", GH_ParamAccess.item);
            pManager[4].Optional = false;
            pManager.AddIntegerParameter("End Edge", "E1", "The ending edge index from the end Brep", GH_ParamAccess.item, 0);
            pManager[5].Optional = false;
            pManager.AddNumberParameter("End Parameter", "P1", "The end edge parameter", GH_ParamAccess.item, 0.5);
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
            pManager.AddCurveParameter("Curve", "C", "The resulting blend curve", GH_ParamAccess.item);
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

            double tA = 0.5;
            DA.GetData(2, ref tA);

            double tB = 0.5;
            DA.GetData(6, ref tB);

            int typeA = 2;
            DA.GetData(3, ref typeA);

            int typeB = 2;
            DA.GetData(7, ref typeA);

            BrepEdge edgeA = brepA.Edges[indexA];
            BrepFace faceA = brepA.Faces[edgeA.AdjacentFaces()[0]];
            double paramA = edgeA.Domain.Evaluate(tA);

            BrepEdge edgeB = brepB.Edges[indexB];
            BrepFace faceB = brepB.Faces[edgeB.AdjacentFaces()[0]];
            double paramB = edgeB.Domain.Evaluate(tB);

            Curve curve = Brep.CreateBlendShape(faceA, edgeA, paramA, false, (BlendContinuity)typeA, faceB, edgeB, paramB, false, (BlendContinuity)typeB);

            DA.SetData(0, curve);
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
            get { return new Guid("4025e96a-9aa5-49f5-bb13-ea7921ad7ead"); }
        }
    }
}