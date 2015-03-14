using System;
using Kunukn.XmlVisual.Core.Extensions;
using Kunukn.XmlVisual.Core.SvgBuilder;

namespace Kunukn.XmlVisual.Core.Entities
{
    /// <summary>
    /// Used by views: Rectangle, ChineseBox, Mindmap
    /// </summary>
    public class Box
    {
        public Element DataLink { get; private set; }
        private int Xoffset { get; set; }
        private int Yoffset { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }        

        public Box(int xoffset, int yoffset, Element e)
        {
            DataLink = e;
            Xoffset = xoffset;
            Yoffset = yoffset;
            Width = e.Svg.Width;
            Height = e.Svg.Height;
        }

        public string GetSvg(UserConfig userConfig)
        {            
            if (Config.Debug && DataLink.Svg.ViewData != null)
            {
                if (DataLink.Svg.ViewData is MindmapData)
                {
                    var d = DataLink.Svg.ViewData as MindmapData;
                    DataLink.Name = d.Debug + DataLink.Level +" "+DataLink.Id+" "+DataLink.Name;
                    DataLink.SetValue(d.Debug + DataLink.Level + " " + DataLink.Id + " " + DataLink.Value,userConfig);
                }                
            }

            var rect = string.Format("<rect x='{0}' y='{1}'  width='{2}' height='{3}' rx='{4}' ry='{5}' class='{6}' id='n{7}' _xv_type='{8}' _xv_msg='{9}' />" + Environment.NewLine, DataLink.Svg.X + Xoffset, DataLink.Svg.Y + Yoffset, Width, Height, SvgTool.ElementRounded, SvgTool.ElementRounded, SvgTool.GetCssClass(DataLink.Type), DataLink.Id, DataLink.Type.GetString(), DataLink.Message);            
            var text = SvgTool.GetText(DataLink, Xoffset, Yoffset + SvgTool.Yoffset, userConfig);
            return rect + text;
        }
    }
}
