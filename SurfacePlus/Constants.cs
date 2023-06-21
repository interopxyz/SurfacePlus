using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfacePlus
{
    public class Constants
    {

        #region naming

        public static string UniqueName
        {
            get { return ShortName + "_" + DateTime.UtcNow.ToString("yyyy-dd-M_HH-mm-ss"); }
        }

        public static string LongName
        {
            get { return ShortName + " v" + Major + "." + Minor; }
        }

        public static string ShortName
        {
            get { return "SurfacePlus"; }
        }

        private static string Minor
        {
            get { return typeof(Constants).Assembly.GetName().Version.Minor.ToString(); }
        }
        private static string Major
        {
            get { return typeof(Constants).Assembly.GetName().Version.Major.ToString(); }
        }

        public static string CatCurve
        {
            get { return "Curve"; }
        }

        public static string CatSurface
        {
            get { return "Surface"; }
        }

        public static string SubAnalysis
        {
            get { return "Analysis"; }
        }

        public static string SubDivide
        {
            get { return "Division"; }
        }

        public static string SubFreeform
        {
            get { return "Freeform"; }
        }

        public static string SubUtilities
        {
            get { return "Util"; }
        }

        public static string SubPrimitive
        {
            get { return "Primitive"; }
        }

        public static string SubSubdivide
        {
            get { return "Subdivide"; }
        }

        public static Descriptor Surface
        {
            get { return new Descriptor("Surface", "S", "A Nurbs Surface", "A Nurbs Surface", "Nurbs Surfaces"); }
        }

        public static Descriptor Brep
        {
            get { return new Descriptor("Brep", "B", "A Surface or Polysurface", "A Surface or Polysurface", "Surfaces or Polysurfaces"); }
        }

        public static Descriptor Curve
        {
            get { return new Descriptor("Curve", "C", "A Curve", "Resulting Curve", "Resulting Curves"); }
        }

        #endregion
    }

        public class Descriptor
        {
            private string name = string.Empty;
            private string nickname = string.Empty;
            private string input = string.Empty;
            private string output = string.Empty;
            private string outputs = string.Empty;

            public Descriptor(string name, string nickname, string input, string output, string outputs)
            {
                this.name = name;
                this.nickname = nickname;
                this.input = input;
                this.output = output;
                this.outputs = outputs;
            }

            public virtual string Name
            {
                get { return name; }
            }

            public virtual string NickName
            {
                get { return nickname; }
            }

            public virtual string Input
            {
                get { return input; }
            }

            public virtual string Output
            {
                get { return output; }
            }

            public virtual string Outputs
            {
                get { return outputs; }
            }
        }
    }
