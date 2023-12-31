﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SurfacePlus.Components
{
    public class GH_ExtendSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ExtendSurface class.
        /// </summary>
        public GH_ExtendSurface()
          : base("Extend Surface", "Extend Srf",
              "Extend a surface edge or edges",
              Constants.CatSurface, Constants.SubUtilities)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Input, GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddIntegerParameter("Edge Direction", "E", "The edge to extend", GH_ParamAccess.item,1);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Distance", "D", "The extension distance", GH_ParamAccess.item,1);
            pManager[2].Optional = true;
            pManager.AddBooleanParameter("Smooth", "S", "If true, the extended surface will be smooth", GH_ParamAccess.item, true);
            pManager[3].Optional = true;


            Param_Integer paramA = (Param_Integer)pManager[1];

            paramA.AddNamedValue("All", 0);
            paramA.AddNamedValue("X", 1);
            paramA.AddNamedValue("Y", 2);
            paramA.AddNamedValue("South", 3);
            paramA.AddNamedValue("East", 4);
            paramA.AddNamedValue("North", 5);
            paramA.AddNamedValue("West", 6);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter(Constants.Surface.Name, Constants.Surface.NickName, Constants.Surface.Output, GH_ParamAccess.item);
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

            int direction = 1;
            DA.GetData(1, ref direction);

            double distance = 1.0;
            DA.GetData(2, ref distance);

            bool smooth = true;
            DA.GetData(3, ref smooth);
            switch(direction)
            {
                default:
                    surface1 = surface1.Extend(IsoStatus.East, distance, smooth).ToNurbsSurface();
                    surface1 = surface1.Extend(IsoStatus.West, distance, smooth).ToNurbsSurface();
                    surface1 = surface1.Extend(IsoStatus.North, distance, smooth).ToNurbsSurface();
                    surface1 = surface1.Extend(IsoStatus.South, distance, smooth).ToNurbsSurface();
                    break;
                case 1://X
                    surface1 = surface1.Extend(IsoStatus.East, distance, smooth).ToNurbsSurface();
                    surface1 = surface1.Extend(IsoStatus.West, distance, smooth).ToNurbsSurface();
                    break;
                case 2://Y
                    surface1 = surface1.Extend(IsoStatus.North, distance, smooth).ToNurbsSurface();
                    surface1 = surface1.Extend(IsoStatus.South, distance, smooth).ToNurbsSurface();
                    break;
                case 3://South
                    surface1 = surface1.Extend(IsoStatus.South, distance, smooth).ToNurbsSurface();
                    break;
                case 4://East
                    surface1 = surface1.Extend(IsoStatus.East, distance, smooth).ToNurbsSurface();
                    break;
                case 5://North
                    surface1 = surface1.Extend(IsoStatus.North, distance, smooth).ToNurbsSurface();
                    break;
                case 6://West
                    surface1 = surface1.Extend(IsoStatus.West, distance, smooth).ToNurbsSurface();
                    break;
            }

            DA.SetData(0, surface1);
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
                return Properties.Resources.Sp_Util_Extend;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ea17d5ae-eeb7-4d08-90af-5db637daec2b"); }
        }
    }
}