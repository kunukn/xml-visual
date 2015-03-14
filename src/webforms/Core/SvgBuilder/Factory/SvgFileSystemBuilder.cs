using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Extensions;

namespace Kunukn.XmlVisual.Core.SvgBuilder.Factory
{
    // Introduction to information visualization page 78
    public class SvgFileSystemBuilder : SvgBuilder
    {
        // singleton        
        protected static SvgBuilder _obj;
        private static readonly object Padlock = new object();
        protected SvgFileSystemBuilder() { }
        public static SvgBuilder Instance //thread safe
        {
            get
            {
                lock (Padlock)
                {
                    return _obj ?? (_obj = new SvgFileSystemBuilder());
                }
            }
        }
        // end singleton

        private static int X, Y;


        public override SvgFile BuildSvg(XmlMetadata data)
        {
            this.UConfig = data.UConfig;
            data.IsViewImplemented = true;


            // Init start element pos            
            X = 100;
            Y = 140;

            DoCalculation(data);

            // GENERATE SVG
            var svg = new SvgFile(this.UConfig);
            var sb = new StringBuilder();
            var title = string.Format("{0}", UConfig.GetSvgTitle());
            data.Title = title;
            sb.Append(string.Format("<title>{0}</title>" + NL, title));

            // calc bigraph, must be before GetMetaText()
            string bigraphText = string.Empty;
            if (UConfig.IncludeBigraph)
                data.BigraphInfo = "No Bigraph for this view";
            
            sb.Append(SvgTool.GetMetaText(data));

            sb.Append(string.Format("<g id='viewport'>")); sb.Append(" <!-- viewport -->" + NL);
            sb.Append(string.Format("<g id='structure'>")); sb.Append(" <!-- structure -->" + NL);

            var recursiveGroupBuilded = new StringBuilder(); // the structure
            foreach (var e in data.Root.Childs)                
                RecursiveCreateFSElementGroup(e, recursiveGroupBuilded);
            

            sb.Append(recursiveGroupBuilded.ToString());
            sb.Append("</g>"); sb.Append(" <!-- end structure -->" + NL);

            sb.Append(bigraphText);
            sb.Append("</g>"); sb.Append(" <!-- end viewport -->" + NL + NL);

            svg.GraphicContent = sb.ToString();


            // debug
            //            FileInfo fileInfo;
            //            if (UConfig.ApplicationMode == ApplicationMode.ConsoleApp) fileInfo = new FileInfo(UConfig.RootPath + @"svg\test.svg.xml");
            //            else fileInfo = new FileInfo(UConfig.RootPath + @"svg/test.svg.xml");
            //            var graphContent = FileUtil.ReadFile(fileInfo).GetString();
            //            svg.GraphicContent = graphContent; //debug
            // 

            return svg;
        }


        static void DoCalculation(XmlMetadata data)
        {
            int i = 0;
            foreach (var e in data.Root.Childs)
            {
                e.Svg.Y = Y + i * SvgTool.ElementFileSystemHeightSpan;
                RecursiveSetFSElementPosition(e);
                i++;
            }
        }


        static void RecursiveSetFSElementPosition(Element node)
        {
            if (node == null)
                return;

            node.Svg.X = node.Level * SvgTool.ElementFileSystemWidthSpan + X;

            int yoffset = node.Svg.Y;
            int i = 1;
            foreach (var child in node.Childs)
            {
                child.Svg.Y = yoffset + SvgTool.ElementFileSystemHeightSpan * i++;
                yoffset += child.TotalChilds * SvgTool.ElementFileSystemHeightSpan;

            }

            foreach (var child in node.Childs)
                RecursiveSetFSElementPosition(child);
        }

        public void RecursiveCreateFSElementGroup(Element node, StringBuilder sb)
        {
            if (node == null)
                return;

            // pre recursive work
            sb.Append(CreateFSElementGroup(node));
            if (node.HasChildren())
            {
                sb.Append(string.Format("<g id='subgn{0}'>" + NL, node.Label));
            }

            // recursive work
            foreach (var child in node.Childs)
                RecursiveCreateFSElementGroup(child, sb);

            // post recursive work
            if (node.HasChildren())
            {
                sb.Append(string.Format("</g>" + NL));
            }
        }

        // Specific to FS view
        public string CreateFSElementGroup(Element e)
        {
            if (e == null)
                return string.Empty;

            var gb = string.Format("<g id='gn{0}' onmouseover='HItem(evt)' onmouseout='UItem(evt)'>" + NL, e.Label);
            var rect = string.Format("<rect id='n{0}' _xv_c='{1}' onclick='fs(evt)' x='{2}' y='{3}'  width='{4}' height='{5}' rx='{6}' ry='{7}' class='{8}' _xv_type='{9}' _xv_msg='{10}' />" + NL, e.Label, e.TotalChilds, e.Svg.X, e.Svg.Y, SvgTool.ElementWidth, SvgTool.ElementFileSystemHeight, SvgTool.ElementRounded, SvgTool.ElementRounded, SvgTool.GetCssClass(e.Type), e.Type.GetString(), e.Message);
            var use = "";
            if (e.HasChildren())
                use = string.Format("<use id='tn{0}' x='{1}' y='{2}' xlink:href='#symbolExpanded'/>" + NL, e.Label, e.Svg.X, e.Svg.Y);

            var txt = string.Empty;
            if (e.Type == XmlType.Node) txt = GetTextHelperNode(e, 90, 12, this.UConfig);
            else txt = SvgTool.GetText(e, 90, 12, this.UConfig);
            var ge = "</g>" + NL;
            var g = gb + rect + use + txt + ge;
            return g;
        }


        private static string GetTextHelperNode(Element e, int xoffset, int yoffset, UserConfig userConfig)
        {
            var sb = new StringBuilder();
            var x = e.Svg.X + xoffset;
            var y = e.Svg.Y + yoffset;
            var t = SvgTool.TruncateText(e.Name, userConfig);
            var s = string.Format("<text x='{0}' y='{1}' class='FontAttribute'>{2}" + NL, x, y, t);
            sb.Append(s);
            if (e.Attributes.Count > 0)
            {
                int i = 0;
                int max = 3;
                foreach (var attr in e.Attributes)
                {
                    if (++i >= max && !userConfig.FullText)
                        break;
                    t = SvgTool.TruncateText(attr.Key, userConfig);
                    sb.Append(string.Format("<tspan dx='{0}' dy='{1}'>{2}</tspan>{3}", 6, 0, t, NL));
                }
                if (i >= max && !userConfig.FullText)
                    sb.Append(string.Format("<tspan dx='{0}' dy='{1}'>{2}</tspan>{3}", 1, 0, "...", NL));

            }
            sb.Append("</text>" + NL);
            return sb.ToString();
        }

    }
}
