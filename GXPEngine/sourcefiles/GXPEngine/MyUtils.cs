using System;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    public class MyUtils
    {
        public static double DegreeToRadian(double angle)
        {
            return Math.PI*angle/180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle*(180.0/Math.PI);
        }

        public static Vector2 Lerp(Vector2 from, Vector2 to, float fraction)
        {
            var output = new Vector2(
                MathHelper.Lerp(from.X, to.X, fraction),
                MathHelper.Lerp(from.Y, to.Y, fraction)
                );
            return output;
        }
    }
}