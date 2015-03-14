using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunukn.XmlVisual.Core.Entities
{
    public class Attribute : IComparable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Key { get { return Name+"="+Value;}  }

        public int CompareTo(object o) // if used in sorted list
        {            
            var other = (Attribute)o;
            return Key.CompareTo(other.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(Object o)
        {
            if (o == null)
                return false;

            var other = o as Attribute;
            if (other == null)
                return false;

            return Key==other.Key;
        }


        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Value);
        }
    }
}
