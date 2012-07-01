using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sender_csharp
{
    class Tracks
    {
        public List<Position3D> tracks { get; private set; }
        public Gesture action;
        public bool hasAction { get; private set; }
        private Position3D curPos = new Position3D(0, 0, 0);
        private int cnt;

        public Tracks(Gesture action)
        {
            this.tracks = new List<Position3D>(100);
            this.action = action;
        }

        public void Trail(Position3D newPos)
        {
            if (this.tracks.Count >= this.tracks.Capacity)
            {
                this.tracks.RemoveAt(0);
            }
            if (action.eval(curPos, newPos))
            {
                cnt++;
                if (cnt > 1)
                {
                    hasAction = true;
                }
            }
            else
            {
                cnt = 0;
                hasAction = false;
            }
            this.tracks.Add(newPos);
            curPos = newPos;
        }

    }
}
