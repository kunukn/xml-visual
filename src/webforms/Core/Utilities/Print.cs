using System;
using System.Text;
using Kunukn.XmlVisual.Core.Entities;

namespace Kunukn.XmlVisual.Core.Utilities
{
    /// <summary>
    /// Kunuk Nykjaer
    /// </summary>
    public static class Print
    {
        const string L = "<";
        const string R = ">";
        const string XML = "XML";

        public static string InternalRepresentation(XmlMetadata nodeMetadata)
        {
            string deklaration = string.IsNullOrEmpty(nodeMetadata.XmlDeclaration) ? string.Empty : "v"+L+nodeMetadata.XmlDeclaration+R;
            
            var childs = nodeMetadata.Root.Childs;
            var sb = new StringBuilder();
            var node = nodeMetadata.Root;
            if (node.Childs.Count > 0)
            {
                foreach (var child in childs)                
                    RecursiveInternalRepresentation(child, 1, sb);                
            }
            else
            {
                sb.Append(XML + L + deklaration + R);
            }

            return XML + L + deklaration + sb + R + Environment.NewLine;
        }

        public static void RecursiveInternalRepresentation(Element node, int level, StringBuilder sb)
        {
            if (node == null)
                return;

            string content;
            switch (node.Type)
            {
                case XmlType.Node: //fall through
                case XmlType.Root:
                    content = string.Format("n{0}", L + node.Name + R);
                    break;

                case XmlType.ProcessingInstruction: //fall through
                case XmlType.DocType:
                    content = string.Format("n{0}v{1}", L + node.Name + R, L + node.Value + R);
                    break;

                default:                    
                    content = string.Format("{0}", node.Value);
                    break;
            }


            sb.Append(Environment.NewLine + Indent(level) + Enums.XmlTypeShort[(int)node.Type] + L + content);

            //if (node.Attributes.Count > 0) sb.Append(Environment.NewLine+Indent(level));
            foreach (var attribute in node.Attributes)
                sb.Append("a" + L + L + attribute.Name + R+L + attribute.Value + R+R);
            foreach (var child in node.Childs)            
                RecursiveInternalRepresentation(child, level+1, sb);

            sb.Append(R);// + Environment.NewLine);            
        }

        public static string Indent(int i)
        {
            var sb = new StringBuilder();
            const string indent = "   ";
            for (int j = 0; j < i; j++)            
                sb.Append(indent);            
            return sb.ToString();
        }

        public static string NodeMetadata(XmlMetadata nodeMetadata)
        {
            var sb = new StringBuilder();
            var node = nodeMetadata.Root;
            if (node.Childs.Count > 0)
            {
                RecursivePrint(node, string.Empty, sb);
            }
            else
            {
                sb.Append("Empty xml");
            }

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public static void RecursivePrint(Element node, string indent, StringBuilder sb)
        {
            if (node == null)
                return;

            sb.Append(indent + node + Environment.NewLine);

            foreach (var attribute in node.Attributes)
            {
                sb.Append(indent + "# attr: " + attribute + Environment.NewLine);
            }

            foreach (var child in node.Childs)
            {
                RecursivePrint(child, "---" + indent, sb);
            }
        }
    }
}
