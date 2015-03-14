using System;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Extensions;
using Kunukn.XmlVisual.Core.SvgBuilder.Factory;

namespace Kunukn.XmlVisual.Core.SvgBuilder
{
    public class SvgBuilderFactory
    {
        public static Factory.SvgBuilder Factory(VisualizationType type)
        {
            Factory.SvgBuilder builder;
            switch (type)
            {
                case VisualizationType.Tree:
                    builder = SvgTreeBuilder.Instance;
                    break;

                case VisualizationType.Rectangle:
                    builder = SvgRectangleBuilder.Instance;
                    break;

                case VisualizationType.ChineseBox:
                    builder = SvgChineseBoxBuilder.Instance;
                    break;
                
                case VisualizationType.MindMap:
                    builder = SvgMindMapBuilder.Instance;
                    break;

                case VisualizationType.FileSystem:
                    builder = SvgFileSystemBuilder.Instance;
                    break;

                case VisualizationType.Sunburst:
                    builder = SvgSunburstBuilder.Instance;
                    break;

                default:
                    throw new Exception("VisualizationType not defined " + type.GetString() );
                    break;
            }
            return builder;
        }
    }
}
