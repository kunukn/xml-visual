using System;
using System.Collections.Generic;
using System.Text;
using Kunukn.XmlVisual.Core.Entities;


namespace Kunukn.XmlVisual.Core.SvgBuilder
{
    static class SvgTool
    {
        private static readonly string NL = Environment.NewLine;


        // SVG values relative to Width and Height value 
        public static readonly int Width = 1024;
        public static readonly int Height = 768;
        public static readonly int ElementWidth = 80;
        public static readonly int ElementHeight = 60;
        public static readonly int ElementFileSystemWidthSpan = 100; // filesystem view
        public static readonly int ElementFileSystemHeight = 20; // filesystem view        
        public static readonly int ElementFileSystemHeightSpan = 30; // filesystem view     
        public static readonly int ElementSunburstSize = 60; // sunburst view     
        public static readonly int ElementSunburstSpan = 0; // sunburst view     
        public static readonly int Yoffset = 10;
        public static readonly int Xoffset = 10;
        public static readonly int ElementRounded = 3;
        public static readonly int MenuButtonYPos = 32;
        const int MaxStrlen = 9;

        public static string GetMetaText(XmlMetadata data)
        {
            var sb = new StringBuilder();
            int x = Xoffset * 9;
            int dy = (int)(Yoffset * 1.3);
            sb.Append(string.Format("<g id='metainfo' class='Metadata'>{0}", NL));
            sb.Append(string.Format("<text x='{0}' y='{1}' class='FontMetadata'>METADATA{2}", x, MenuButtonYPos, NL));
            if (data.IsViewImplemented == false)
                sb.Append(string.Format("<tspan x='{0}' dy='{1}'>View is not implemented</tspan>{2}", x, dy, NL));
            sb.Append(string.Format("<tspan x='{0}' dy='{1}'>Title: {2}</tspan>{3}", x, dy, data.Title, NL));
            sb.Append(string.Format("<tspan x='{0}' dy='{1}'>Height: {2}</tspan>{3}", x, dy, data.Height, NL));
            sb.Append(string.Format("<tspan x='{0}' dy='{1}'>MaxWidth: {2}</tspan>{3}", x, dy, data.MaxWidth, NL));
            sb.Append(string.Format("<tspan x='{0}' dy='{1}'>ElementsCount: {2}</tspan>{3}", x, dy, data.ElementsCount, NL));
            if (data.Info != null)
                sb.Append(string.Format("<tspan x='{0}' dy='{1}' style='fill:#ff0000;'>Info: {2}</tspan>{3}", x, dy, data.Info, NL));
            if (data.XmlDeclaration != null)
                sb.Append(string.Format("<tspan x='{0}' dy='{1}' style='fill:#000000;'>Declaration: {2}</tspan>{3}", x, dy, data.XmlDeclaration, NL));
            if (data.BigraphInfo != null)
                sb.Append(string.Format("<tspan x='{0}' dy='{1}'>BigraphItemInfo: {2}</tspan>{3}", x, dy, data.BigraphInfo, NL));
            if (data.BigraphItemCount != null)
                sb.Append(string.Format("<tspan x='{0}' dy='{1}'>BigraphItemCount: {2}</tspan>{3}", x, dy, data.BigraphItemCount, NL));
            if (data.BigraphTotalCount != null)
                sb.Append(string.Format("<tspan x='{0}' dy='{1}'>BigraphTotalCount: {2}</tspan>{3}", x, dy, data.BigraphTotalCount, NL));
            if (data.BigraphMaxCount != null)
                sb.Append(string.Format("<tspan x='{0}' dy='{1}'>BigraphMaxCount: {2}</tspan>{3}", x, dy, data.BigraphMaxCount, NL));
            sb.Append(string.Format("</text>{0}", NL));
            sb.Append(string.Format("</g>{0}", NL + NL));
            return sb.ToString();
        }

        public static string GetText(Element e, int xoffset, int yoffset, UserConfig userConfig)
        {
            string s = string.Empty;
            switch (e.Type)
            {
                case XmlType.Node: // fall through                    
                case XmlType.Root:
                    s = GetTextHelperNode(e, xoffset, yoffset, userConfig);
                    break;
                case XmlType.Text:
                    s = GetTextHelperText(e, xoffset, yoffset);
                    break;
                case XmlType.Cdata:
                    s = GetTextHelperCdata(e, xoffset, yoffset);
                    break;
                case XmlType.Comment:
                    s = GetTextHelperComment(e, xoffset, yoffset);
                    break;
                case XmlType.DocType:
                    s = GetTextHelperDocType(e, xoffset, yoffset);
                    break;
                case XmlType.ProcessingInstruction:
                    s = GetTextHelperProcessingInstruction(e, xoffset, yoffset, userConfig);
                    break;
                case XmlType.Unknown:
                    s = "XmlType is Unknown";
                    break;
                default: break;
            }
            return s;
        }
       
        public static string TruncateText(string str, UserConfig userConfig)
        {
            const string append = "..";
            if (userConfig.FullText)
                return str;

            if (str != null && str.Length > MaxStrlen + append.Length)
            {
                var s = string.Concat(str.Substring(0, MaxStrlen), append);
                if (s.Contains("&") || s.Contains(":"))
                {
                    return FixEntityRefError(str, MaxStrlen, append);
                }
                return s;
            }
            return str;
        }

        public static string TruncateText(string str, int maxlen, string append)
        {
            var s = str;
            if (s.Length > maxlen+append.Length)
                s = string.Concat(s.Substring(0, maxlen), append);

            if (s.Contains("&") || s.Contains(":"))            
                s = FixEntityRefError(str, maxlen,append);
            
            return s;
        }

        // quick fix, avoid  Entity Ref error introduced by truncating string
        // if performance issues by using this, alternatively look at regex
        private static string FixEntityRefError(string str,int maxlen, string append)
        {
            try
            {
                var decoded = System.Web.HttpUtility.HtmlDecode(str);
                if (decoded.Length > maxlen)
                {
                    var s = string.Concat(decoded.Substring(0, maxlen), append);
                    var encoded = System.Web.HttpUtility.HtmlEncode(s);
                    return encoded;
                }
                return str;
            }
            catch (Exception ex) //because user hacked the data
            {
                var s = string.Concat(str.Substring(0, maxlen), append);
                return s.Replace("&", "_AND_").Replace(":", "_COLON_"); 
            }
        }

        private static string GetTextHelperNode(Element e, int xoffset, int yoffset, UserConfig userConfig)
        {
            var sb = new StringBuilder();
            var x = e.Svg.X + xoffset;
            var y = e.Svg.Y + yoffset;
            var s = string.Format("<g><text x='{0}' y='{1}' class='FontNode'>{2}</text>{3}", x, y, TruncateText(e.Name, userConfig), NL);
            sb.Append(s);
            if (e.Attributes.Count > 0)
            {
                var beg = string.Format("<text x='{0}' y='{1}' class='FontAttribute'>#" + NL, x, y + Yoffset);
                sb.Append(beg);

                int i = 0;
                int max = 3;
                foreach (var attr in e.Attributes)
                {
                    if (++i >= max && !userConfig.FullText)
                        break;
                    sb.Append(string.Format("<tspan x='{0}' dy='{1}'>{2}</tspan>{3}", x, Yoffset, SvgTool.TruncateText(attr.Key, userConfig), NL));
                }
                if (i >= max && !userConfig.FullText)
                    sb.Append(string.Format("<tspan x='{0}' dy='{1}'>{2}</tspan>{3}", x, Yoffset, "...", NL));


                var end = string.Format("</text>{0}", NL);
                sb.Append(end);
            }
            sb.Append("</g>");
            return sb.ToString();
        }

        private static string GetTextHelperText(Element e, int xoffset, int yoffset)
        {
            var x = e.Svg.X + xoffset;
            var y = e.Svg.Y + yoffset;
            var txt = e.ValueShort;

            var str = string.Format("<text x='{0}' y='{1}' class='FontText'>{2}</text>{3}", x, y, txt, NL);
            return str;

        }
        private static string GetTextHelperCdata(Element e, int xoffset, int yoffset)
        {
            var x = e.Svg.X + xoffset;
            var y = e.Svg.Y + yoffset;
            var txt = e.ValueShort;

            var sb = new StringBuilder();
            sb.Append(string.Format("<text x='{0}' y='{1}' class='FontCdata'>{2}{3}", x, y, "&lt;![CDATA[  ]]&gt;", NL));
            sb.Append(string.Format("<tspan x='{0}' dy='{1}'>{2}</tspan>{3}", x, Yoffset, txt, NL));
            sb.Append(string.Format("</text>{0}", NL));

            return sb.ToString();
        }
        private static string GetTextHelperDocType(Element e, int xoffset, int yoffset)
        {
            var x = e.Svg.X + xoffset;
            var y = e.Svg.Y + yoffset;
            var txt = e.ValueShort;

            var sb = new StringBuilder();
            sb.Append(string.Format("<text x='{0}' y='{1}' class='FontDocType'>{2}{3}", x, y, "&lt;![DOCTYPE  ]&gt;", NL));
            sb.Append(string.Format("<tspan x='{0}' dy='{1}'>{2}</tspan>{3}", x, Yoffset, txt, NL));
            sb.Append(string.Format("</text>{0}", NL));

            return sb.ToString();
        }

        private static string GetTextHelperProcessingInstruction(Element e, int xoffset, int yoffset, UserConfig userConfig)
        {
            var x = e.Svg.X + xoffset;
            var y = e.Svg.Y + yoffset;
            var txt = e.ValueShort;

            var sb = new StringBuilder();
            sb.Append(string.Format("<text x='{0}' y='{1}' class='FontProcessingInstruction'>{2}{3}", x, y, "&lt;? PI &gt;", NL));
            sb.Append(string.Format("<tspan x='{0}' dy='{1}'>{2}</tspan>{3}", x, Yoffset, txt, NL));
            sb.Append(string.Format("</text>{0}", NL));
          
            return sb.ToString();
        }

        private static string GetTextHelperComment(Element e, int xoffset, int yoffset)
        {
            var x = e.Svg.X + xoffset;
            var y = e.Svg.Y + yoffset;
            var txt = e.ValueShort;

            var sb = new StringBuilder();
            sb.Append(string.Format("<text x='{0}' y='{1}' class='FontComment'>{2}{3}", x, y, "&lt;!-- --&gt;", NL));
            sb.Append(string.Format("<tspan x='{0}' dy='{1}'>{2}</tspan>{3}", x, Yoffset, txt, NL));
            sb.Append(string.Format("</text>{0}", NL));

            return sb.ToString();
        }
        public static string GetCssClass(XmlType type)
        {
            string cssClass;
            switch (type)
            {
                case XmlType.Node:
                    cssClass = "ElementNode";
                    break;
                case XmlType.Cdata:
                    cssClass = "ElementCdata";
                    break;
                case XmlType.Comment:
                    cssClass = "ElementComment";
                    break;
                case XmlType.Text:
                    cssClass = "ElementText";
                    break;
                case XmlType.Root:
                    cssClass = "ElementRoot";
                    break;
                case XmlType.DocType:
                    cssClass = "ElementDocType";
                    break;
                case XmlType.ProcessingInstruction:
                    cssClass = "ElementProcessingInstruction";
                    break;
                case XmlType.Unknown:
                    cssClass = "ElementUnknown";
                    break;

                default:
                    throw new Exception("XmlType css class not supported " + type);
            }
            return cssClass;
        }
    }
}
