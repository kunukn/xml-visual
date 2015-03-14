using System;
using Kunukn.XmlVisual.Core.Entities;

namespace Kunukn.XmlVisual.Core.SvgBuilder.Factory
{    
    public class SvgTemplateBuilder : SvgBuilder
    {        
        // singleton
        private static SvgBuilder _obj;
        private SvgTemplateBuilder() { }
        public static SvgBuilder GetInstance() { return _obj ?? (_obj = new SvgTemplateBuilder()); }
        // end singleton

        public override SvgFile BuildSvg(XmlMetadata data)
        {            
            return base.BuildSvg(data);            
        }       

    }
}
