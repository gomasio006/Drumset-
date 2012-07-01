using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sender_csharp
{
    class Stick
    {
        private const double DEFAULT_ANGLE = Math.PI / 6;//[rad]
        private const double DEFAULT_LENGTH = 0.3;//[m]
        private static double length = DEFAULT_LENGTH;
        private static double angle = DEFAULT_ANGLE;

        //private static int cnt = 0;

        internal static Position3D GetPositionOfEndOfStickRight(Position3D hand_pos, Position3D wrist_pos)
        {
            /*
            Position3D stick_pos = new Position3D();
            double angleY = System.Math.Atan((hand_pos.Y - wrist_pos.Y) / (hand_pos.Z - wrist_pos.Z));
            double angleX = System.Math.Atan((hand_pos.X - wrist_pos.X) / (hand_pos.Z - wrist_pos.Z));
            stick_pos.Z = hand_pos.Z - System.Math.Cos(angleX - Math.PI/6);
            stick_pos.X = hand_pos.X - System.Math.Sin(angleX - Math.PI/6);
            stick_pos.Y = hand_pos.Y - System.Math.Sin(angleY);
             */

            Position3D stick_pos = new Position3D();
            double angleY = System.Math.Atan((hand_pos.Y - wrist_pos.Y) / (wrist_pos.Z - hand_pos.Z));
            double angleX = System.Math.Atan((wrist_pos.Z - hand_pos.Z) / (hand_pos.X - wrist_pos.X));

            if (angleX < 0) angleX += Math.PI;

            stick_pos.Z = hand_pos.Z - length * System.Math.Sin(angleX + angle);
            stick_pos.X = hand_pos.X + length * System.Math.Cos(angleX + angle);
            stick_pos.Y = hand_pos.Y + length * System.Math.Sin(angleY);
            /*
            if (cnt > 150)
            {
                System.Console.WriteLine("wrist" + wrist_pos);
                System.Console.WriteLine("hand" + hand_pos);
                System.Console.WriteLine("stick" + stick_pos + "\n");
                cnt = 0;
            }
            cnt++;
            */
            return stick_pos;
        }

        internal static Position3D GetPositionOfEndOfStickLeft(Position3D hand_pos, Position3D wrist_pos)
        {

            Position3D stick_pos = new Position3D();
            double angleY = System.Math.Atan((hand_pos.Y - wrist_pos.Y) / (wrist_pos.Z - hand_pos.Z));
            double angleX = System.Math.Atan((wrist_pos.Z - hand_pos.Z) / (hand_pos.X - wrist_pos.X));

            if (angleX < 0) angleX += Math.PI;

            stick_pos.Z = hand_pos.Z - length * System.Math.Sin(angleX - angle);
            stick_pos.X = hand_pos.X + length * System.Math.Cos(angleX - angle);
            stick_pos.Y = hand_pos.Y + length * System.Math.Sin(angleY);

            /*
            if (cnt > 150)
            {
                System.Console.WriteLine("wrist" + wrist_pos);
                System.Console.WriteLine("hand" + hand_pos);
                System.Console.WriteLine("stick" + stick_pos + "\n");
                cnt = 0;
            }
            cnt++;
            */
            return stick_pos;
        }
    }

}

