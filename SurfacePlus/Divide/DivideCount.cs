﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Divide
{
    public class DivideCount : Divide__Base
    {
        /// <summary>
        /// Initializes a new instance of the DivideCount class.
        /// </summary>
        public DivideCount()
          : base("Divide Count", "Div Count",
              "Divide a surface into evenly spaced isocurve spans by count",
              Constants.CatSurface, Constants.SubDivide)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("Count", "C", "The primary division count", GH_ParamAccess.item, 4);
            pManager[2].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            base.RegisterOutputParams(pManager);
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

            int count = 4;
            DA.GetData(2, ref count);

            DA.SetDataList(0, surface1.DivideCount((SurfaceDirection)direction, count));
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
            get { return new Guid("bdd03d9e-1a9b-4bfa-8f5d-b822e53785ff"); }
        }
    }
}