using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sender_csharp
{
    interface Gesture
    {
        bool eval(Position3D prePos, Position3D pos);
    }

    class BeatGesture : Gesture
    {
        public bool eval(Position3D prePos, Position3D pos)
        {
            if (prePos.Y > pos.Y + 0.01)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
