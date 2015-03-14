using System;
using System.Text;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Utilities;
using Kunukn.XmlVisual.Core.Extensions;

namespace Kunukn.XmlVisual.Core.SvgBuilder.Factory
{
    // Introduction to information visualization page 66
    public class SvgMindMapBuilder : SvgBuilder
    {
        // singleton
        private static SvgBuilder _obj;
        private static readonly object Padlock = new object();
        private SvgMindMapBuilder() { }
        public static SvgBuilder Instance //thread safe
        {
            get
            {
                lock (Padlock)
                {
                    return _obj ?? (_obj = new SvgMindMapBuilder());
                }
            }
        }
        // end singleton

        //position of start
        private static int A = SvgTool.Width / 2;
        private static int B = SvgTool.Height / 2;

        public override SvgFile BuildSvg(XmlMetadata data)
        {
            this.UConfig = data.UConfig;
            data.IsViewImplemented = true;

            // Init element pos
            // The levels, set x,y pos for the elements                        

            Element root = null;
            foreach (var child in data.Root.Childs)
            {
                if (child.Type == XmlType.Node) //only 1 node, else invalid xml file
                    root = child;
            }

            if (root != null)
            {
                root.Type = XmlType.Root;
                double sizex = SvgTool.ElementWidth;
                double sizey = SvgTool.ElementHeight;

                root.Svg.Width = sizex.ToIntRounded();
                root.Svg.Height = sizey.ToIntRounded();
                root.Svg.X = A;
                root.Svg.Y = B;
                var m = new MindmapData();
                m.ThetaLeftRestrict = 0;
                m.ThetaRightRestrict = MathTool.Pi2 - MathTool.AngleToRadian(1);//MathTool.Epsilon;                
                root.Svg.ViewData = m;

                if (root.HasChildren())
                    DoCalculation(root, data);
            }

            // GENERATE SVG
            var svg = new SvgFile(this.UConfig);
            var sb = new StringBuilder();
            var title = string.Format("{0}", UConfig.GetSvgTitle());
            data.Title = title;
            sb.Append(string.Format("<title>{0}</title>" + NL, title));

            if (UConfig.IncludeBigraph)
                data.BigraphInfo = "No Bigraph for this view";


            sb.Append(SvgTool.GetMetaText(data));

            sb.Append(string.Format("<g id='viewport'>")); sb.Append(" <!-- viewport -->" + NL);
            sb.Append(string.Format("<g id='structure'>")); sb.Append(" <!-- structure -->" + NL);

            var recursiveGroupBuilded = new StringBuilder();
            RecursiveCreateElementGroup(root, recursiveGroupBuilded);
            sb.Append(recursiveGroupBuilded.ToString());

            sb.Append("</g>"); sb.Append(" <!-- end structure -->" + NL);

            sb.Append("</g>"); sb.Append(" <!-- end viewport -->" + NL + NL);
            svg.GraphicContent = sb.ToString();

            return svg;
        }


        public void RecursiveCreateElementGroup(Element node, StringBuilder sb)
        {
            if (node == null || node.Svg.Width == 0)
                return;

            // pre recursive work            
            sb.Append(CreateElementGroup(node, 0, 0));
            bool hasChildren = !node.IsLeaf();
            if (hasChildren)
            {
                sb.Append(string.Format("<g id='subgn{0}' style=''>{1}", node.Id, NL));

                // Lines
                var childs = node.Childs;
                if (childs.Count > 0)
                    foreach (var child in childs)
                    {
                        if (child.Svg.Width == 0)
                            continue;

                        var px = node.Svg.X + node.Svg.Width / 2;
                        var py = node.Svg.Y + node.Svg.Height / 2;
                        var cx = child.Svg.X + child.Svg.Width / 2;
                        var cy = child.Svg.Y + child.Svg.Height / 2;
                        sb.Append(String.Format("<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' class='Line2'/>{4}", px, py, cx, cy, NL));
                    }
            }

            // recursive work
            foreach (var child in node.Childs)
                RecursiveCreateElementGroup(child, sb);

            // post recursive work
            if (hasChildren)
            {
                sb.Append(string.Format("</g>" + NL));
            }
        }

        /// <summary>
        /// Heuristic approach
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="items"></param>
        /// <param name="isLeaf"></param>
        /// <returns></returns>
        static double GetRadiusNeeded(double theta, int items, bool isLeaf)
        {
            double def = SvgTool.ElementWidth * 1.9; // default length between parent and child line                                    

            int span = (SvgTool.ElementWidth * 1.4).ToIntRounded(); // default span dist between childs
            var degree = MathTool.RadiansToAngle(theta);
            var degreeHalf = degree / 2.0;
            var thetaHalf = MathTool.RadianNormalize(theta / 2.0);

            if (degree >= 70 || items <= 1) // wide enough, return something smaller than def
                return def / 1.4;

            if (degree >= 40) // wide enough
                return def;

            if (degreeHalf < 2)
                thetaHalf = MathTool.AngleToRadian(2);

            // triangle trigonometri
            var sin = Math.Abs(Math.Sin(thetaHalf)); //MathTool.RadianNormalize(sin);
            
            var c = def;
            var b = sin * c;

            int i = 0;
            while (b * 2 < span)
            {
                if (i++ >= 20) // stop here reduce len even angle is to small to display evenly
                    break;
                c += def;
                b = sin * c;
            }

            if(c<0)
                throw new Exception();

            return c;
        }


        /// <summary>
        /// Heuristic approach, the fn could be simpler but has been tweaked a few places for experimenting
        /// Sunburst imple is much simpler
        /// </summary>
        /// <param name="root"></param>
        /// <param name="data"></param>
        static void DoCalculation(Element root, XmlMetadata data)
        {
            var childs = root.Childs;
            var count = childs.Count;

            //http://en.wikipedia.org/wiki/Polar_coordinate_system
            // first child points downward, even distribute

            double beg = MathTool.AngleToRadian(90);

            double thetaRootStep = count == 0 ? 0 : MathTool.Pi2 / count;


            // init root children
            for (int i = 0; i < count; i++)
            {
                var c = childs[i];

                var d = new MindmapData();
                d.ThetaPoint = i * thetaRootStep + beg;

                if (count >= 3)
                {
                    d.ThetaLeftRestrict = (i - 1) * thetaRootStep + beg;
                    d.ThetaRightRestrict = (i + 1) * thetaRootStep + beg;
                }
                else if (count == 2)
                {
                    d.ThetaLeftRestrict = d.ThetaPoint - MathTool.Pihalf;
                    d.ThetaRightRestrict = d.ThetaPoint + MathTool.Pihalf;
                }
                else
                {
                    d.ThetaLeftRestrict = d.ThetaPoint - MathTool.Pihalf;
                    d.ThetaRightRestrict = d.ThetaPoint + MathTool.Pihalf;
                }

                d.Radius = GetRadiusNeeded(thetaRootStep, count, c.IsLeaf());
                c.Svg.ViewData = d;

                c.Svg.Width = SvgTool.ElementWidth;
                c.Svg.Height = SvgTool.ElementHeight;
                double x, y;
                MathTool.PolarToCartesian(d.ThetaPoint, d.Radius, out x, out y);
                c.Svg.X = x.ToIntRounded() + A;
                c.Svg.Y = y.ToIntRounded() + B;

                var debug = c.Name + " " + d.Debug;   
            }


            // init rest layers
            for (int i = 2; i <= data.Height; i++)
            {

                // UPDATE RESTRICT
                var level = data.GetLevel(i);
                for (int j = 0; j < level.Count; j++)
                {
                    Element current = level[j];
                    Element left = null;
                    Element right = null;
                    const double aloneSpan = MathTool.Pihalf / 1.1;

                    if (level.Count >= 3)
                    {
                        left = j == 0 ? level[level.Count - 1] : level[j - 1];
                        right = j == level.Count - 1 ? level[0] : level[j + 1];
                    }

                    var d = current.Svg.ViewData as MindmapData;

                    if (level.Count == 1)
                    {
                        // alone, increase span
                        d.ThetaLeftRestrict = d.ThetaPoint - aloneSpan;
                        d.ThetaRightRestrict = d.ThetaPoint + aloneSpan;
                    }
                    else if (level.Count == 2)
                    {
                        var other = j == 0 ? level[1] : level[0];
                        if (other.IsLeaf())
                        {
                            d.ThetaLeftRestrict = d.ThetaPoint - aloneSpan;
                            d.ThetaRightRestrict = d.ThetaPoint + aloneSpan;
                        }
                        else
                        {
                            var otherm = other.Svg.ViewData as MindmapData;
                            double distr = MathTool.DistanceRadian(d.ThetaPoint, otherm.ThetaPoint);
                            double distl = MathTool.DistanceRadian(otherm.ThetaPoint, d.ThetaPoint);
                            d.ThetaRightRestrict = d.ThetaPoint + distr / 2;
                            d.ThetaLeftRestrict = d.ThetaPoint - distl / 2;
                        }

                    }
                    else
                    {
                        var leftm = left.Svg.ViewData as MindmapData;
                        var rightm = right.Svg.ViewData as MindmapData;

                        if (left.IsLeaf())
                            // might collide using Math.PI/8 but reduce size
                            d.ThetaLeftRestrict = leftm.ThetaPoint - Math.PI / 8;//leftm.ThetaPoint;
                        else
                        {
                            double distl = MathTool.DistanceRadian(leftm.ThetaPoint, d.ThetaPoint);
                            d.ThetaLeftRestrict = d.ThetaPoint - distl / 2;
                        }

                        if (right.IsLeaf())
                            // might collide using Math.PI/8 but reduce size
                            d.ThetaRightRestrict = rightm.ThetaPoint + Math.PI / 8;//rightm.ThetaPoint
                        else
                        {
                            double distr = MathTool.DistanceRadian(d.ThetaPoint, rightm.ThetaPoint);
                            d.ThetaRightRestrict = d.ThetaPoint + distr / 2;
                        }

                    }

                    // restrict by max span of left, only by 90 angle related to Thetapoint or if alone
                    var dist = MathTool.DistanceRadian(d.ThetaLeftRestrict, d.ThetaPoint);
                    if (dist > MathTool.Pihalf + MathTool.Epsilon)
                        d.ThetaLeftRestrict = d.ThetaPoint - aloneSpan;

                    // restrict by max span of right, only by 90 angle related to Thetapoint or if alone
                    dist = MathTool.DistanceRadian(d.ThetaPoint, d.ThetaRightRestrict);
                    if (dist > MathTool.Pihalf + MathTool.Epsilon)
                        d.ThetaRightRestrict = d.ThetaPoint + aloneSpan;
                }

                // INIT LAYER and set children mindmap data
                for (int j = 0; j < level.Count; j++)
                {
                    Element current = level[j];
                    var pd = current.Svg.ViewData as MindmapData;
                    double thetaStep = MathTool.DistanceRadian(pd.ThetaLeftRestrict, pd.ThetaRightRestrict); //Math.PI; //default
                    if (current.Childs.Count > 1)
                        thetaStep = MathTool.DistanceRadian(pd.ThetaLeftRestrict, pd.ThetaRightRestrict) / (current.Childs.Count - 1);

                    for (int k = 0; k < current.Childs.Count; k++)
                    {
                        var c = current.Childs[k];
                        var d = new MindmapData();

                        // placement
                        if (current.Childs.Count == 1)
                        {
                            d.ThetaPoint = pd.ThetaPoint;
                            d.ThetaLeftRestrict = pd.ThetaLeftRestrict;
                            d.ThetaRightRestrict = pd.ThetaRightRestrict;
                        }
                        else //count > 1
                        {
                            d.ThetaPoint = k * thetaStep + pd.ThetaLeftRestrict;
                            if (k == 0)
                            {
                                d.ThetaLeftRestrict = pd.ThetaLeftRestrict;
                                d.ThetaRightRestrict = pd.ThetaLeftRestrict + thetaStep;
                            }
                            else if (k == current.Childs.Count - 1)
                            {
                                d.ThetaRightRestrict = pd.ThetaRightRestrict;
                                d.ThetaLeftRestrict = pd.ThetaRightRestrict - thetaStep;
                            }
                            else
                            {
                                d.ThetaLeftRestrict = (k - 1) * thetaStep + pd.ThetaLeftRestrict;
                                d.ThetaRightRestrict = (k + 1) * thetaStep + pd.ThetaLeftRestrict;
                            }

                        }

                        d.Radius = GetRadiusNeeded(thetaStep, current.Childs.Count, c.IsLeaf());
                        c.Svg.ViewData = d;

                        c.Svg.Width = SvgTool.ElementWidth;
                        c.Svg.Height = SvgTool.ElementHeight;
                        double x, y;
                        MathTool.PolarToCartesian(d.ThetaPoint, d.Radius, out x, out y);
                        c.Svg.X = x.ToIntRounded() + c.Parent.Svg.X;
                        c.Svg.Y = y.ToIntRounded() + c.Parent.Svg.Y;
                    }
                }


            }
        }

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
