using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sender_csharp
{
    class Drumset : IEnumerable<Target>
    {
        List<Target> elements;

        public Drumset()
        {
            elements = new List<Target>();
        }

        public void SetElements(params Target[] elements)
        {
            foreach (Target e in elements)
            {
                this.elements.Add(e);
            }
        }

        public DetectResult Detect(Tracks tracks)
        {
            if (tracks.hasAction)
            {
                foreach (Target e in elements)
                {
                    double s;
                    if ((s = e.Evaluate(tracks)) != -1)
                    {
                        return new DetectResult(tracks.tracks[tracks.tracks.Count - 1], s, e.Id);
                    }
                }
            }
            return null;
        }


        public class DetectResult
        {
            public Position3D Pos { private set; get; }
            public double Speed { private set; get; }
            public string TargetId { private set; get; }

            public DetectResult(Position3D pos, double speed, string targetId)
            {
                this.Pos = pos;
                this.TargetId = targetId;
                this.Speed = speed;
            }

            public override string ToString()
            {
                return "Target: " + TargetId + " Speed: " + Speed + " X: " + Pos.X + " Z: " + Pos.Z;
            }

            public string ToJSON()
            {
                return "[\"Id\"=\"" + TargetId + "\",\"X\"=\"" + Pos.X + "\",\"Y=\"" + Pos.Y + "\",\"Speed\"=\"" + Speed + "\"]";
            }
        }

        IEnumerator<Target> IEnumerable<Target>.GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }
    }
}
