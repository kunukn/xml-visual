using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunukn.XmlVisual.Core.Entities
{
    
    public enum XmlType { Unknown = -1, Node, Text, Cdata, Comment, Root, DocType, ProcessingInstruction }
    public enum VisualizationType { Unknown = -1, Tree, Rectangle, ChineseBox, MindMap, FileSystem, Sunburst }
    public enum ApplicationMode { Unknown = -1, ConsoleApp, WebApp }
    

    public static class Enums
    {
        public static readonly string[] XmlTypeShort = { "N", "T", "CD", "C", "N", "DT", "PI" }; //root is N could be R but must be desc in report then
        public static string GetTypeShort(XmlType t)
        {
            var i = (int) t;
            if (i >= 0)
                return XmlTypeShort[i];
            return "?";
        }
    }
}
