using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Extensions;

namespace Kunukn.XmlVisual.Core.SvgBuilder.Factory
{
    // Introduction to information visualization page 79
    public class SvgRectangleBuilder : SvgBuilder
    {
        // singleton
        private static SvgBuilder _obj;
        private static readonly object Padlock = new object();
        private SvgRectangleBuilder() { }
        public static SvgBuilder Instance //thread safe
        {
            get
            {
                lock (Padlock)
                {
                    return _obj ?? (_obj = new SvgRectangleBuilder());
                }
            }
        }
        // end singleton        

        //position of topleft
        private int A, B;


        /// <summary>
        /// O(k*n)
        /// First find max width by iterating tree topdown
        /// Then set size by iterating tree topdown
        /// Then reduce size by iterating tree bottomup
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override SvgFile BuildSvg(XmlMetadata data)
        {
            this.UConfig = data.UConfig;
            data.IsViewImplemented = true;

            // Init element pos            
            A = 20;
            B = UConfig.IncludeBigraph ? (3.5 * SvgTool.ElementHeight).ToIntRounded() : 1 * SvgTool.ElementHeight; //start offset

            var svg = new SvgFile(this.UConfig);
            var sb = new StringBuilder();
            var title = string.Format("{0}", UConfig.GetSvgTitle());
            data.Title = title;
            sb.Append(string.Format("<title>{0}</title>" + NL, title));

            Element startRoot = null;
            foreach (var child in data.Root.Childs)
            {
                if (child.Type == XmlType.Node) //only 1 node, else invalid xml file
                    startRoot = child;
            }

            // Find max width of boxes
            // Topdown
//            var max = new Integer { Value = 0 };
//            RecursiveFindMaxWidth(startRoot, 1, max);

            // Topdown set size
            //            RecursiveSetSize(startRoot, 1, totalWidth);

            // set init size
            var all = data.AllElements;
            foreach (var e in all)
            {
                e.Svg.Height = SvgTool.ElementHeight;
                if (e.IsLeaf())
                    e.Svg.Width = SvgTool.ElementWidth;
            }

            // bottomup trim size, use leaf size as decision point
            for (int i = data.Height; i >= 1; i--)
            {
                List<Element> level = data.GetLevel(i);
                if (level.Count == 0)
                    continue;
                foreach (var e in level)
                {
                    if (e.Childs.Count == 0)
                        continue;
                    int sum = e.Childs.Sum(c => c.Svg.Width);
                    e.Svg.Width = sum;
                }
            }


            if (startRoot != null)
            {
                A = -startRoot.Svg.Width / 2 + SvgTool.Width / 2;
            }

            var boxes = new StringBuilder();
            // Topdown generate SVG
            for (int i = 1; i <= data.Height; i++)
            {
                List<Element> level = data.GetLevel(i);
                if (level.Count == 0) // no data at this level, then done
                    break;

                int offset = 0;
                Element parent = null;
                foreach (var e in level)
                {
                    if (i == 1 && e.Type != XmlType.Node)
                        continue; //only using startRoot

                    if (parent != e.Parent)
                    {
                        offset = 0;
                        parent = e.Parent;
                    }
                    e.Svg.X = e.Parent.Svg.X + offset;
                    e.Svg.Y = i * SvgTool.ElementHeight;
                    boxes.Append(CreateElementGroup(e, A, B));
                    offset += e.Svg.Width;
                }
            }

            //             adjust, bigraph use real pos of element x,y            
            foreach (var e in all)
            {
                e.Svg.X += A;
                e.Svg.Y += B;
            }


            // calc bigraph, must be before GetMetaText()
            string bigraphText = "";
            if (UConfig.IncludeBigraph)
            {
                int bigraphItemCount;
                int bigraphTotalCount;
                int bigraphMaxCount;
                bigraphText = GetBigraphText(data, out bigraphItemCount, out bigraphTotalCount, out bigraphMaxCount, 0);
                data.BigraphItemCount = bigraphItemCount.ToString();
                data.BigraphTotalCount = bigraphTotalCount.ToString();
                data.BigraphMaxCount = bigraphMaxCount.ToString();
            }

            sb.Append(SvgTool.GetMetaText(data));
            sb.Append("<g id='viewport'>"); sb.Append(" <!-- viewport -->" + NL);
            sb.Append(string.Format("<g id='structure'>")); sb.Append(" <!-- structure -->" + NL);
            sb.Append(boxes.ToString());
            sb.Append("</g>"); sb.Append(" <!-- end structure -->" + NL);

            sb.Append(bigraphText);
            sb.Append("</g>"); sb.Append(" <!-- end viewport -->" + NL + NL);
            svg.GraphicContent = sb.ToString();

            return svg;
        }

//        static void RecursiveFindMaxWidth(Element node, int count, Integer max)
//        {
//            if (node == null)
//                return;
//            int childs = node.Childs.Count;
//            if (childs == 0)
//                return;
//
//            int newcount = count * childs;
//            if (newcount > max.Value)
//                max.Value = newcount;
//            foreach (var child in node.Childs)
//                RecursiveFindMaxWidth(child, newcount, max);
//        }

        string CreateElementGroup(Element e, int xoffset, int yoffset)
        {
            if (e == null)
                return string.Empty;

            var gb = string.Format("<g id='gn{0}' onmouseover='HItem(evt)' onmouseout='UItem(evt)' onclick='g{1}(evt)'>{2}", e.Id, this.UConfig.JsGroupFn, NL);
            Box b = new Box(xoffset, yoffset, e);
            var ge = "</g>" + NL;
            var g = gb + b.GetSvg(this.UConfig) + ge;
            return g;
        }


    }
}
