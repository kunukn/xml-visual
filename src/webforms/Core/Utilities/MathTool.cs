using System;
using System.Drawing;
using System.Linq;
using Kunukn.XmlVisual.Core.entities;

namespace Kunukn.XmlVisual.Core.Utilities
{
    /// <summary>
    /// Kunuk Nykjaer
    /// </summary>
    public static class MathTool
    {
        public const int ROUND = 5;
        public const int TwoDecimals = 2;
        public const double Epsilon = 0.00001;
        public const double Pi2 = 2 * Math.PI;
        public const double Pihalf = Math.PI / 2.0;

        // http://en.wikipedia.org/wiki/Polar_coordinate_system        
        public static void CartesianToPolar(double x, double y, out double theta, out double radius)
        {
            theta = -1;
            radius = Math.Sqrt(x * x + y * y);            

            var xIs0 = Math.Abs(x) < Epsilon; //floating point precision issue
            var yIs0 = Math.Abs(y) < Epsilon;

            if (x > 0)
                theta = Math.Atan(y / x);
            else if (x < 0 && y >= 0)
                theta = Math.Atan(y / x) + Math.PI;
            else if (x < 0 && y < 0)
                theta = Math.Atan(y / x) - Math.PI;
            else if (xIs0 && y > 0)
                theta = Pihalf;
            else if (xIs0 && y < 0)
                theta = -Pihalf;
            else if (xIs0 && yIs0)
                theta = 0;

            if (theta == -1)
                throw new Exception("CartesianToPolar() is broken");        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="radius"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void PolarToCartesian(double theta, double radius, out double x, out double y)
        {
            var radian = RadianNormalize(theta);
            x = Math.Round(radius * Math.Cos(radian), TwoDecimals);
            y = Math.Round(radius * Math.Sin(radian), TwoDecimals);

//            if(x<0 || y<0)
//            {
//                Console.WriteLine();
//            }
        }

        private static double Distance(double val1, double val2)
        {
            return Math.Abs(val2 - val1);            
        }

        public static double DistanceRadian(double from, double to)
        {
            var f = RadianNormalize(from);
            var t = RadianNormalize(to);

            if (t >= f)
                return t - f;
            return RadianNormalize(t + Pi2 - f);
        }

        public static double DistanceAngle(double from, double to)
        {
            var f = AngleNormalize(from);
            var t = AngleNormalize(to);

            if (t >= f)
                return t - f;
            return AngleNormalize(t + 360 - f);                                           
        }

        public static double RadiansToAngle(double radian)
        {
            var r = RadianNormalize(radian);
            return AngleNormalize(Math.Round(r * (180 / Math.PI), TwoDecimals));
        }
        public static double AngleToRadian(double degree)
        {
            var d = AngleNormalize(degree);
            return RadianNormalize(  Math.Round(  d / (180 / Math.PI), TwoDecimals));
        }

        // true if diff from to by 180 angle
        public static bool IsRadianToTheLeft(double from, double to)
        {
            return false;
        }
        public static bool IsRadianToTheRight(double from, double to)
        {
            return false;
        }


        // [0; 2pi[
        public static double RadianNormalize(double radian)
        {
            while (radian < 0)            
                radian += Pi2;                            
            while (radian >= Pi2)
                radian -= Pi2;
            return radian;
        }

        // [0; 360[
        public static double AngleNormalize(double degree)
        {
            while (degree < 0)
                degree += 360;
            while (degree >= 360)
                degree -= 360;
            return degree;
        }



        

        public static double Distance(Point a, Point b)
        {
            return  Distance(new XY{X=a.X,Y=a.Y}, new XY{X=b.X,Y=b.Y}) ;            
        }

        private const double Exp = 2; // 2=euclid, 1=manhatten
        public static double Distance(XY a, XY b)
        {
            return Math.Pow(Math.Pow(Math.Abs(a.X - b.X), Exp) + Math.Pow(Math.Abs(a.Y - b.Y), Exp), 1.0 / Exp);
        }

        public static double Distance(double[] vector1, double[] vector2)
        {
            int size = vector1.Count();
            double sum = 0;

            for (int i = 0; i < size; i++)
            {
                var a = vector1[i];
                var b = vector2[i];
                sum += Math.Pow(Math.Abs(a - b), Exp);
            }
            return Math.Pow(sum, 1.0 / Exp);
        }



        public static double Min(double a, double b)
        {
            if (a <= b)
                return a;
            return b;
        }
        public static double Max(double a, double b)
        {
            if (a >= b)
                return a;
            return b;
        }

        public static double Map(double x, double a, double b, double c, double d)
        {
            var r = (x - a) / (b - a) * (d - c) + c;
            return r;
        }

        public static double Norm(double x, double a, double b)
        {
            var r = Map(x, a, b, 0, 1);
            return r;
        }

        public static double Lerp(double x, double a, double b)
        {
            var r = Map(x, 0, 1, a, b);
            return r;
        }

        public static double Constrain(double x, double a, double b)
        {
            var r = Max(a, Min(x, b));
            return r;
        }

    }
}
