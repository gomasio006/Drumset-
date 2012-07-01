using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sender_csharp
{
    public class Position3D
    {
        public double X { set; get; }
        public double Y { set; get; }
        public double Z { set; get; }

        public Position3D()
            : this(0, 0, 0)
        {
        }

        public Position3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return "Position: X=" + X + ", Y=" + Y + ", Z=" + Z;
        }
    }
}
