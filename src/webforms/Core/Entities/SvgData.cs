using System;

namespace Kunukn.XmlVisual.Core.Entities
{
    public class SvgData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ViewData ViewData { get; set; }  // view specific data container     


        public SvgData()
        {
        }

    }
}
