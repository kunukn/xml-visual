using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Kunukn.XmlVisual.Core.Extensions;
using Kunukn.XmlVisual.Core.Utilities;

namespace Kunukn.XmlVisual.Core.Entities
{
    public class SunburstData : ViewData
    {        

        public static string TestGetSvgCone(SunburstData e)
        {
            if (e.IsAngleSmall)
                return string.Format("<path d='M {0} {1} A {2} {3} 0 0 1 {4} {5} L {6} {7} z' fill='{8}' stroke='black' stroke-width='1'/>", e.LongBeg.X, e.LongBeg.Y, e.Radius2, e.Radius2, e.LongEnd.X, e.LongEnd.Y, e.RootCenter.X, e.RootCenter.Y, "white");

            return string.Format("<path d='M {0} {1} A {2} {3} 0 1 1 {4} {5} L {6} {7} z' fill='{8}' stroke='black' stroke-width='1'/>", e.LongBeg.X, e.LongBeg.Y, e.Radius2, e.Radius2, e.LongEnd.X, e.LongEnd.Y, e.RootCenter.X, e.RootCenter.Y, "white");
        }
        public static string TestGetSvg(SunburstData e)
        {            
            if (e.IsAngleSmall)
                return string.Format("<path d='M {0} {1} A {2} {3} 0 0 1 {4} {5} L {6} {7} A {8} {9}  0 0 0 {10} {11} z' fill='{12}' stroke='black' stroke-width='1'/>", e.LongBeg.X, e.LongBeg.Y, e.Radius2, e.Radius2, e.LongEnd.X, e.LongEnd.Y, e.ShortEnd.X, e.ShortEnd.Y, e.Radius, e.Radius, e.ShortBeg.X, e.ShortBeg.Y, "white");

            return string.Format("<path d='M {0} {1} A {2} {3} 0 1 1 {4} {5} L {6} {7} A {8} {9}  0 1 0 {10} {11} z' fill='{12}' stroke='black' stroke-width='1'/>", e.LongBeg.X, e.LongBeg.Y, e.Radius2, e.Radius2, e.LongEnd.X, e.LongEnd.Y, e.ShortEnd.X, e.ShortEnd.Y, e.Radius, e.Radius, e.ShortBeg.X, e.ShortBeg.Y, "white");
        }



        private double _thetaPoint = MathTool.AngleToRadian(90); // default downwards, not important to set
        public double ThetaPoint
        {
            get { return _thetaPoint; }
            set { _thetaPoint = MathTool.RadianNormalize(value); }
        }

        private double _thetaLeftRestrict;
        public double ThetaLeftRestrict
        {
            get { return _thetaLeftRestrict; }
            set { _thetaLeftRestrict = MathTool.RadianNormalize(value); }
        }

        private double _thetaRightRestrict;
        public double ThetaRightRestrict
        {
            get { return _thetaRightRestrict; }
            set { _thetaRightRestrict = MathTool.RadianNormalize(value); }
        }
        public double Radius2 { get; set; }
        public double Radius { get; set; }

        public System.Drawing.Point LongBeg { get; private set; }
        public Point LongEnd { get; private set; }
        public Point ShortBeg { get; private set; }
        public Point ShortEnd { get; private set; }

        public Point RootCenter { get; set; }
        public Point ContentPoint { get; private set; }
        public bool IsAngleSmall { get; private set; } // < 180 degree        
        public double Angle
        {
            get
            {
                var dist = MathTool.DistanceRadian(ThetaLeftRestrict, ThetaRightRestrict);
                return MathTool.RadiansToAngle(dist);                                
            }            
        }
        
       
        public string Debug
        {
            get
            {
                var d1 = MathTool.AngleNormalize(Math.Round(MathTool.RadiansToAngle(ThetaPoint))) + " ";
                var d2 = MathTool.AngleNormalize(Math.Round(MathTool.RadiansToAngle(ThetaLeftRestrict))) + " ";
                var d3 = MathTool.AngleNormalize(Math.Round(MathTool.RadiansToAngle(ThetaRightRestrict))) + " ";
                return d1 + d2 + d3;
            }
        }


        public void Init()
        {
            double x, y;
            int a, b;

            MathTool.PolarToCartesian(ThetaLeftRestrict, Radius, out x, out y);
            a = RootCenter.X + x.ToIntRounded();
            b = RootCenter.Y + y.ToIntRounded();
            ShortBeg = new Point(a, b);

            MathTool.PolarToCartesian(ThetaRightRestrict, Radius, out x, out y);
            a = RootCenter.X + x.ToIntRounded();
            b = RootCenter.Y + y.ToIntRounded();
            ShortEnd = new Point(a, b);

            MathTool.PolarToCartesian(ThetaLeftRestrict, Radius2, out x, out y);
            a = RootCenter.X + x.ToIntRounded();
            b = RootCenter.Y + y.ToIntRounded();
            LongBeg = new Point(a, b);

            MathTool.PolarToCartesian(ThetaRightRestrict, Radius2, out x, out y);
            a = RootCenter.X + x.ToIntRounded();
            b = RootCenter.Y + y.ToIntRounded();
            LongEnd = new Point(a, b);

            #region ContentPosHeuristic            
            /*
            // content position, heuristic tweaks
            var wx = 0.5;
            var wy = 0.5;
            var xy1 = LongBeg;
            var xy2 = ShortEnd;
            var direction = MathTool.RadiansToAngle(ThetaPoint);
            if (direction >= 110 && direction <= 271)
            {
                xy2 = LongEnd;
                if (direction >= 240 && direction <= 271)
                    wx=0.3;
            }

            if (direction >= 110 && direction <= 179)
            {
                wx = 0.3;
                wy = 0.3;
            }

            if (direction >= 0 && direction <= 91)
            {
                wx = 0.3;
                wy = 0.3;
            }

            // linear interpolate
            int x0 = MathTool.Min(xy1.X, xy2.X).ToIntRounded();
            int y0 = MathTool.Min(xy1.Y, xy2.Y).ToIntRounded();
            int x1 = MathTool.Max(xy1.X, xy2.X).ToIntRounded();
            int y1 = MathTool.Max(xy1.Y, xy2.Y).ToIntRounded();            
            ContentPoint = new Point(x0 + Math.Abs((x1 - x0) *wx).ToIntRounded(), y0 + Math.Abs((y1 - y0) *wy).ToIntRounded() );
            */
            #endregion ContentPosHeuristic

            // simple
            double cx, cy;
            double offset = Radius*0.1; //avoid on same angle 0 or 180 text overlapping
            MathTool.PolarToCartesian(ThetaPoint, Radius+ (Radius2-Radius)/2,out cx,out cy);
            ContentPoint = new Point((RootCenter.X + cx - offset).ToIntRounded(), (RootCenter.Y + cy - offset).ToIntRounded());

            var diff = MathTool.DistanceRadian(ThetaLeftRestrict, ThetaRightRestrict);
            IsAngleSmall = false;
            if (diff < Math.PI)
                IsAngleSmall = true;            
        }


        public SunburstData()
        {        
        }
    }
}
