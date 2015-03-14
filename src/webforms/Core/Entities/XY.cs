using System;
using Kunukn.XmlVisual.Core.Extensions;
using Kunukn.XmlVisual.Core.Utilities;

namespace Kunukn.XmlVisual.Core.entities
{
    public class XY : IComparable
    {        
        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {
            return X + ";" + Y;
        }

        public double Distance(XY other)
        {
            return MathTool.Distance(this, other);
        }


        public int CompareTo(object o) // if used in sorted list
        {
            if (this.Equals(o))
                return 0;

            var other = (XY)o;
            if (this.X > other.X)
                return -1;
            if (this.X < other.X)
                return 1;

            return 0;
        }

        public override int GetHashCode()
        {
            var x = X*10000;
            var y = Y*10000;
            var r  = x*17 + y*37;
            return (int) r;
        }

        public override bool Equals(Object o)
        {
            if (o == null)
                return false;

            var other = o as XY;
            if (other == null)
                return false;

            var x = this.X.Round() == other.X.Round();
            var y = this.Y.Round() == other.Y.Round();
            return x && y;
        }
    }
}
