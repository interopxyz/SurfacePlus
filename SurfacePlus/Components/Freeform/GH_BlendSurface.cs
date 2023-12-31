﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SurfacePlus.Components
{
    public class GH_BlendSurface : GH_Component
    {
        protected List<Brep> prev_breps = new List<Brep>();

        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GH_BlendSurface()
          : base("Blend Surface", "Blend Srf",
              "Create a blended surface from two brep edges",
              Constants.CatSurface, Constants.SubFreeform)
        {
        }

        protected override void BeforeSolveInstance()
        {
            prev_breps = new List<Brep>();
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
            pManager.AddBrepParameter("Start Brep", "B0", "The starting brep of the blend", GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddIntegerParameter("Start Edge", "E0", "The starting edge index from the start Brep", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Start Type", "T0", "The start ege blend type", GH_ParamAccess.item, 2);
            pManager[2].Optional = true;
            pManager.AddIntervalParameter("Start Domain", "D0", "The starting edge sub domain", GH_ParamAccess.item, new Interval(0,1));
            pManager[3].Optional = true;
            pManager.AddBooleanParameter("Flip Start", "F0", "If true, the starting edge direction will be flipped", GH_ParamAccess.item, false);
            pManager[4].Optional = true;


            pManager.AddBrepParameter("End Brep", "B1", "The ending brep of the blend", GH_ParamAccess.item);
            pManager[5].Optional = false;
            pManager.AddIntegerParameter("End Edge", "E1", "The ending edge index from the end Brep", GH_ParamAccess.item);
            pManager[6].Optional = true;
            pManager.AddIntegerParameter("End Type", "T1", "The end edge blend type", GH_ParamAccess.item, 2);
            pManager[7].Optional = true;
            pManager.AddIntervalParameter("End Domain", "D1", "The ending edge sub domain", GH_ParamAccess.item, new Interval(0, 1));
            pManager[8].Optional = true;
            pManager.AddBooleanParameter("Flip End", "F1", "If true, the ending edge direction will be flipped", GH_ParamAccess.item, true);
            pManager[9].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[2];
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

            int indexA = 0;
            bool hasA = DA.GetData(1, ref indexA);

            int typeA = 2;
            DA.GetData(2, ref typeA);

            Interval domainA = new Interval(0, 1);
            DA.GetData(3, ref domainA);

            bool flipA = true;
            DA.GetData(4, ref flipA);

            Brep brepB = null;
            if (!DA.GetData(5, ref brepB)) return;

            int indexB = 2;
            bool hasB = DA.GetData(6, ref indexB);

            int typeB = 2;
            DA.GetData(7, ref typeB);

            Interval domainB = new Interval(0, 1);
            DA.GetData(8, ref domainB);

            bool flipB = false;
            DA.GetData(9, ref flipB);

            if (!hasA) prev_breps.Add(brepA);
            if (!hasB) prev_breps.Add(brepB);

            BrepEdge edgeA = brepA.Edges[indexA];
            BrepFace faceA = brepA.Faces[edgeA.AdjacentFaces()[0]];
            Interval domA = domainA.Remap(edgeA.Domain);

            BrepEdge edgeB = brepB.Edges[indexB];
            BrepFace faceB = brepB.Faces[edgeB.AdjacentFaces()[0]];
            Interval domB = domainB.Remap(edgeB.Domain);

            List<Brep> breps = Brep.CreateBlendSurface(faceA, edgeA,domA, flipA, (BlendContinuity)typeA, faceB, edgeB,domB, flipB, (BlendContinuity)typeB).ToList();

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
                return Properties.Resources.Sp_Free_Blend_Surface;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bbce9a81-8569-401c-9a59-75426b5b25d4"); }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            if (Hidden) return;
            if (Locked) return;
            Transform xform = args.Viewport.GetTransform(Rhino.DocObjects.CoordinateSystem.World, Rhino.DocObjects.CoordinateSystem.Screen);

            Rhino.Display.DisplayMaterial mat = new Rhino.Display.DisplayMaterial();
            if (Attributes.Selected)
            {
                mat = args.ShadeMaterial_Selected;
            }
            else
            {
                mat = args.ShadeMaterial;
            }

            Color activeColor = mat.Diffuse;

            foreach(Brep brep in prev_breps)
            {
                List<BrepEdge> edges = brep.Edges.ToList();
                int i = 0;
                foreach(BrepEdge edge in edges)
                {
                    Point3d p = new Point3d(edge.EdgeCurve.PointAtNormalizedLength(0.5));
                    p.Transform(xform);
                    args.Display.Draw2dText(i.ToString(), activeColor, new Point2d(p.X, p.Y), true);
                    i++;
                }
            }

            base.DrawViewportMeshes(args);
        }

        }
}