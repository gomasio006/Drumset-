using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sender_csharp
{
    abstract class Target
    {
        public Position3D Pos { protected set; get; }
        public string Id { protected set; get; }

        public abstract double Evaluate(Tracks tracks);
        public abstract string ToJSON();
    }

    class CircleTarget : Target
    {
        public double radius { private set; get; }

        public CircleTarget(string id, Position3D pos, double radius)
        {
            this.Pos = pos;
            this.Id = id;
            this.radius = radius;
        }

        public override string ToJSON()
        {
            PositionRatio p = new PositionRatio(this.Pos);
            return "{\"Id\":\"" + Id + "\",\"X\":\"" + p.X + "\",\"Y\":\"" + Pos.Y + "\",\"Z\":\"" + p.Z + "\",\"radius\":\"" + radius/(2*PositionRatio.maxX) + "\"}";
        }

        public override double Evaluate(Tracks tracks)
        {
            List<Position3D> t = tracks.tracks;
            int cur = tracks.tracks.Count - 1;
            int pre = cur - 1;
            if (t[pre].Y > Pos.Y && t[cur].Y < Pos.Y)
            {
                double X = (t[cur].X + t[pre].X) / 2;
                double Z = (t[cur].Z + t[pre].Z) / 2;
                double r = Math.Sqrt(Math.Pow((X - Pos.X), 2) + Math.Pow((Z - Pos.Z), 2));
                if (r < radius)
                {
                    //System.Console.WriteLine("X : " + X + " Z : " + Z);
                    return t[pre].Y - t[cur].Y;
                }
            }
            return -1;
        }

    }
}
