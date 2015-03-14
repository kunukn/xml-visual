using System;
using Kunukn.XmlVisual.Core.Utilities;

namespace Kunukn.XmlVisual.Core.Entities
{
    public class MindmapData : ViewData
    {
        private double _thetaPoint = MathTool.AngleToRadian(90); // default downwards, not important to set
        public double ThetaPoint
        {
            get { return _thetaPoint; }
            set { _thetaPoint = MathTool.RadianNormalize(value); }
        }

        private double _thetaLeftRestrict = -1;
        public double ThetaLeftRestrict
        {
            get { return _thetaLeftRestrict; }
            set
            {
                if (_thetaRightRestrict != -1)
                {
                }
                _thetaLeftRestrict = MathTool.RadianNormalize(value);
            }
        }


        private double _thetaRightRestrict = -1;
        public double ThetaRightRestrict
        {
            get { return _thetaRightRestrict; }
            set
            {
                if (_thetaLeftRestrict != -1)
                {
                }
                _thetaRightRestrict = MathTool.RadianNormalize(value);
            }
        }
        public double Radius { get; set; }
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



        public MindmapData()
        {

        }
    }
}
