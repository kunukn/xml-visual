using System;
using System.Collections.Generic;
using System.Text;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Extensions;

namespace Kunukn.XmlVisual.Core.SvgBuilder.Factory
{
    // Introduction to information visualization page 77
    public class SvgTreeBuilder : SvgBuilder
    {
        // singleton        
        protected static SvgBuilder _obj;
        private static readonly object Padlock = new object();
        protected SvgTreeBuilder() { }
        public static SvgBuilder Instance  //thread safe
        {
            get
            {
                lock (Padlock)
                {
                    return _obj ?? (_obj = new SvgTreeBuilder());
                }
            }
        }
        // end singleton

        private static int A, B;

        public override SvgFile BuildSvg(XmlMetadata data)
        {
            this.UConfig = data.UConfig;
            data.IsViewImplemented = true;


            // Init element pos
            // The levels, set y pos for the elements
            A = (int)(SvgTool.ElementHeight * 1.8); // step
            B = UConfig.IncludeBigraph ? (2.5 * SvgTool.ElementHeight).ToIntRounded() : 1 * SvgTool.ElementHeight; //start offset


            DoCalculation(data);

            // GENERATE SVG
            var svg = new SvgFile(UConfig);
            var sb = new StringBuilder();
            var title = string.Format("{0}", UConfig.GetSvgTitle());
            data.Title = title;
            sb.Append(string.Format("<title>{0}</title>" + NL, title));

            // calc bigraph, must be before GetMetaText()
            string bigraphText = string.Empty;
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

            sb.Append(string.Format("<g id='viewport'>")); sb.Append(" <!-- viewport -->" + NL);
            sb.Append(string.Format("<g id='structure'>")); sb.Append(" <!-- structure -->" + NL);

            var recursiveGroupBuilded = new StringBuilder(); // the tree
            foreach (var child in data.Root.Childs)
                RecursiveCreateTreeElementGroup(child, recursiveGroupBuilded);

            sb.Append(recursiveGroupBuilded.ToString());
            sb.Append("</g>"); sb.Append(" <!-- end structure -->" + NL);

            sb.Append(bigraphText);
            sb.Append("</g>"); sb.Append(" <!-- end viewport -->" + NL + NL);
            svg.GraphicContent = sb.ToString();

            return svg;
        }


        static void DoCalculation(XmlMetadata data)
        {
            for (int i = 1; i <= data.Height; i++)
            {
                List<Element> level = data.GetLevel(i);
                if (level.Count == 0) // no data at this level, then done
                    break;

                int count = level.Count;
                int width = SvgTool.Width;
                int step = SvgTool.Width / (count * 2);

                // when cannot fit onscreen, move them outside
                int offset = 0;
                while (step < (SvgTool.ElementWidth / 2.0) + SvgTool.ElementWidth * 0.1)
                {
                    offset -= SvgTool.ElementWidth * 2;//width/8
                    width += SvgTool.ElementWidth * 4; //width/4
                    step = width / (count * 2);
                }

                // even distribute elements using the space efficiently
                var y = i * A + B;
                for (int j = 0; j < count; j++)
                {
                    if (j == 0)
                        level[j].Svg.X = step * (j + 1) - SvgTool.ElementWidth / 2 + offset;
                    else
                        level[j].Svg.X = step * (2 * j + 1) - (SvgTool.ElementWidth / 2) + offset;
                    level[j].Svg.Y = y;
                }
            }
        }

        public void RecursiveCreateTreeElementGroup(Element node, StringBuilder sb)
        {
            if (node == null)
                return;

            // pre recursive work
            sb.Append(CreateTreeElementGroup(node));
            if (node.HasChildren())
            {
                sb.Append(string.Format("<g id='subgn{0}' style='' class=''>{1}", node.Id, NL));

                // Lines
                var childs = node.Childs;
                if (childs.Count > 0)
                    foreach (var child in childs)
                    {
                        var px = node.Svg.X + SvgTool.ElementWidth / 2;
                        var py = node.Svg.Y + SvgTool.ElementHeight;
                        var cx = child.Svg.X + SvgTool.ElementWidth / 2;
                        var cy = child.Svg.Y;

                        sb.Append(String.Format("<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' class='Line'/>{4}", px, py, cx, cy, NL));
                    }
            }

            // recursive work
            foreach (var child in node.Childs)
                RecursiveCreateTreeElementGroup(child, sb);

            // post recursive work
            if (node.HasChildren())
            {
                sb.Append(string.Format("</g>" + NL));
            }
        }

        // Specific to Tree and BigraphTree view
        public string CreateTreeElementGroup(Element e)
        {
            if (e == null)
                return string.Empty;

            var gb = string.Format("<g id='gn{0}' onmouseover='HItem(evt)' onmouseout='UItem(evt)' onclick='g{1}(evt)' _xv_symb='{2}'>" + NL, e.Id, this.UConfig.JsGroupFn, Enums.GetTypeShort(e.Type));
            var rect = string.Format("<rect id='n{0}' x='{1}' y='{2}'  width='{3}' height='{4}' rx='{5}' ry='{6}' class='{7}' _xv_type='{8}' _xv_msg='{9}'/>" + NL, e.Id, e.Svg.X, e.Svg.Y, SvgTool.ElementWidth, SvgTool.ElementHeight, SvgTool.ElementRounded, SvgTool.ElementRounded, SvgTool.GetCssClass(e.Type), e.Type.GetString(), e.Message);

            var txt = SvgTool.GetText(e, 0, SvgTool.Yoffset, this.UConfig);
            var ge = "</g>" + NL;
            var g = gb + rect + txt + ge;
            return g;
        }

    }
}
