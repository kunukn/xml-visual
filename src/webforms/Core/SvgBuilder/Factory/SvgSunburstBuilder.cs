using System;
using System.Drawing;
using System.Text;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Utilities;
using Kunukn.XmlVisual.Core.Extensions;

namespace Kunukn.XmlVisual.Core.SvgBuilder.Factory
{
    public class SvgSunburstBuilder : SvgBuilder
    {
        // singleton
        private static SvgBuilder _obj;
        private static readonly object Padlock = new object();
        private SvgSunburstBuilder() { }
        public static SvgBuilder Instance //thread safe
        {
            get
            {
                lock (Padlock)
                {
                    return _obj ?? (_obj = new SvgSunburstBuilder());
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
                var d = new SunburstData();
                d.ThetaLeftRestrict = 0;
                d.ThetaPoint = Math.PI; //not used for root
                d.ThetaRightRestrict = MathTool.AngleToRadian(359);
                d.RootCenter = new Point(A, B);
                d.Radius = 10;
                d.Radius2 = SvgTool.ElementSunburstSize;
                d.Init(); //important
                root.Svg.ViewData = d;
                root.Svg.X = A-25;
                root.Svg.Y = B-25;

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
            if (node == null)
                return;

            // pre recursive work            
            sb.Append(CreateElementGroup(node));
            bool hasChildren = !node.IsLeaf();
            if (hasChildren)
                sb.Append(string.Format("<g id='subgn{0}' style=''>{1}", node.Id, NL));

            // recursive work
            foreach (var child in node.Childs)
                RecursiveCreateElementGroup(child, sb);

            // post recursive work
            if (hasChildren)
                sb.Append(string.Format("</g>" + NL));

        }


        static double GetRadiusNeeded(SunburstData data)
        {
            double def = data.Radius + SvgTool.ElementSunburstSize;// default length between parent and child              
            return def; // seems prettier, using this

            var thetaHalf = MathTool.DistanceRadian(data.ThetaLeftRestrict, data.ThetaRightRestrict) / 2;
            var degreeHalf = MathTool.RadiansToAngle(thetaHalf);
            int minimumspan = SvgTool.ElementSunburstSize/2;

            if (degreeHalf > 20)
                return def;

            if (degreeHalf < 1)
                thetaHalf = MathTool.AngleToRadian(1);//set minimum


            // triangle trigonometri
            var sin = Math.Abs(Math.Sin(thetaHalf));
                        
            double c = def;
            var b = sin * c;

            int i = 0;
            while (b * 2 < minimumspan)
            {
                if (i++ >= 20) // stop here reduce len even angle is to small to display evenly
                    break;
                c += SvgTool.ElementSunburstSize;
                b = sin * c;
            }
            
            if(c<0)
                throw new Exception();

            return c;            
        }



        static void DoCalculation(Element root, XmlMetadata data)
        {
            var childs = root.Childs;
            var count = childs.Count;
            var rootData = (SunburstData)root.Svg.ViewData;// as SunburstData; //not null

            // first child points downward, even distribute

            double beg = MathTool.AngleToRadian(90); //downwards
            double thetaRootStep = count == 0 ? 0 : MathTool.Pi2 / count;

            // init root children, layer 1
            for (int i = 0; i < count; i++)
            {
                var c = childs[i];

                var d = new SunburstData();
                d.ThetaPoint = i * thetaRootStep + beg;

                if (count >= 3)
                {
                    d.ThetaLeftRestrict = d.ThetaPoint - thetaRootStep / 2;
                    d.ThetaRightRestrict = d.ThetaPoint + thetaRootStep / 2;
                }
                else if (count == 2)
                {
                    d.ThetaLeftRestrict = d.ThetaPoint - MathTool.Pihalf;
                    d.ThetaRightRestrict = d.ThetaPoint + MathTool.Pihalf;
                }
                else
                {
                    d.ThetaLeftRestrict = rootData.ThetaLeftRestrict;
                    d.ThetaRightRestrict = rootData.ThetaRightRestrict;
                }

                d.RootCenter = rootData.RootCenter;
                d.Radius = rootData.Radius2+SvgTool.ElementSunburstSpan;
                d.Radius2 = GetRadiusNeeded(d);
                d.Init(); //important

                var debug = c.Name + " " + d.Debug;
                c.Svg.ViewData = d;
                c.Svg.X = d.ContentPoint.X; //always after Init()
                c.Svg.Y = d.ContentPoint.Y;

            }

            // init rest layers
            for (int i = 2; i <= data.Height; i++)
            {
                var level = data.GetLevel(i);
              
                // INIT LAYER and set children sunburst data
                for (int j = 0; j < level.Count; j++)
                {
                    Element current = level[j];
                    if (current.Childs.Count == 0)
                        continue;

                    var pd = current.Svg.ViewData as SunburstData; //not null
                    double thetaStep = MathTool.DistanceRadian(pd.ThetaLeftRestrict, pd.ThetaRightRestrict);
                    thetaStep /= current.Childs.Count;

                    for (int k = 0; k < current.Childs.Count; k++)
                    {
                        var c = current.Childs[k];
                        var d = new SunburstData();

                        // placement
                        if (current.Childs.Count == 1)
                        {
                            d.ThetaPoint = pd.ThetaPoint;
                            d.ThetaLeftRestrict = pd.ThetaLeftRestrict;
                            d.ThetaRightRestrict = pd.ThetaRightRestrict;
                        }
                        else //count > 1
                        {
                            d.ThetaPoint = k * thetaStep + pd.ThetaLeftRestrict + thetaStep / 2;
                            d.ThetaLeftRestrict = d.ThetaPoint - thetaStep / 2;
                            d.ThetaRightRestrict = d.ThetaPoint + thetaStep / 2;
                        }

                        d.Radius = pd.Radius2 + SvgTool.ElementSunburstSpan;
                        d.Radius2 = GetRadiusNeeded(d);
                        d.RootCenter = pd.RootCenter;
                        d.Init(); //important

                        c.Svg.ViewData = d;
                        c.Svg.X = d.ContentPoint.X;
                        c.Svg.Y = d.ContentPoint.Y;
                    }
                }
            }
        }


        string CreateElementGroup(Element e)
        {
            if (e == null)
                return string.Empty;
            var gb = string.Format("<g id='gn{0}' onmouseover='HItem(evt)' onmouseout='UItem(evt)' onclick='g{1}(evt)' _xv_symb='{2}' >" + NL, e.Id, this.UConfig.JsGroupFn, Enums.GetTypeShort(e.Type));

            SvgSunburst b = new SvgSunburst(e, this.UConfig);

            var ge = "</g>" + NL;
            var g = gb + b.Svg + ge;
            
            return g;
        }

        class SvgSunburst
        {
            public string Svg { get; private set; }
            private SunburstData d;
            private Element e;

            public SvgSunburst(Element e, UserConfig c)
            {
                this.e = e;
                d = e.Svg.ViewData as SunburstData;
                MakeSvg(c);
            }

            void MakeSvg(UserConfig c)
            {
                if (e.Type == XmlType.Root)
                    Svg= string.Format("<circle cx='{0}' cy='{1}' r='{2}' class='{3}' _xv_msg='{4}' _xv_type='{5}'  id='n{6}' />", d.RootCenter.X, d.RootCenter.Y, d.Radius2, SvgTool.GetCssClass(e.Type), e.Message, e.Type.GetString(), e.Id);

                else if (d.IsAngleSmall)
                    Svg= string.Format("<path d='M {0} {1} A {2} {3} 0 0 1 {4} {5} L {6} {7} A {8} {9}  0 0 0 {10} {11} z' class='{12}' _xv_msg='{13}'  _xv_type='{14}'  id='n{15}' />", d.LongBeg.X, d.LongBeg.Y, d.Radius2, d.Radius2, d.LongEnd.X, d.LongEnd.Y, d.ShortEnd.X, d.ShortEnd.Y, d.Radius, d.Radius, d.ShortBeg.X, d.ShortBeg.Y, SvgTool.GetCssClass(e.Type), e.Message, e.Type.GetString(), e.Id);

                else Svg= string.Format("<path d='M {0} {1} A {2} {3} 0 1 1 {4} {5} L {6} {7} A {8} {9}  0 1 0 {10} {11} z' class='{12}' _xv_msg='{13}'  _xv_type='{14}'  id='n{15}' />", d.LongBeg.X, d.LongBeg.Y, d.Radius2, d.Radius2, d.LongEnd.X, d.LongEnd.Y, d.ShortEnd.X, d.ShortEnd.Y, d.Radius, d.Radius, d.ShortBeg.X, d.ShortBeg.Y, SvgTool.GetCssClass(e.Type), e.Message, e.Type.GetString(), e.Id);

                if(d.Angle>20)
                {
                    var text = SvgTool.GetText(e, 0, 0 + SvgTool.Yoffset, c);
                    Svg += text;
                }

            }
        }


    }
}
