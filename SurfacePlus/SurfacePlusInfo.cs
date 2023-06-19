using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace SurfacePlus
{
    public class SurfacePlusInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "SurfacePlus";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Properties.Resources.SurfacePlus_Logo_24;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Additional curve editing utilities for Grasshopper 3d";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("8e7b34b0-1c70-47cd-8e3d-ce0a1fbd20a8");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "David Mans";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "interopxyz@gmail.com";
            }
        }
    }
}
