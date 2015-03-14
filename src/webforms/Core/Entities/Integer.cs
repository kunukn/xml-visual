using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunukn.XmlVisual.Core.Entities
{
    public class Integer
    {
        public Integer() { }
        public Integer(int i)
        {
            Value = i;
        }

        public int Value { get; set; }
    }
}
