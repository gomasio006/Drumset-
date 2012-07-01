using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sender_csharp
{
    class PositionRatio
    {
        public double X { private set; get; }
        public double Z { private set; get; }

        public static double maxX = 1.3;
        public static double maxZ = 2.8;
        public static double dist = 1.7;


        public static void setProp(double x, double z, double d)
        {
            dist = d;
            maxX = x;
            maxZ = z;
        }

        public PositionRatio(double x, double z)
        {
            X = (maxX + x) / (2 * maxX);
            Z = (z - dist) / (maxZ - dist);
        }

        public PositionRatio(Position3D p)
            : this(p.X, p.Z)
        {
        }

        public override string ToString()
        {
            return "(X=" + X + ", Z=" + Z + ")";
        }

        public string ToJSON()
        {
            return "{\"X\":\"" + this.X + "\", \"Z\":\"" + Z + "\"}";
        }
    }

}
