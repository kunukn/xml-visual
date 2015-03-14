using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunukn.XmlVisual.Core.Extensions;
using Kunukn.XmlVisual.Core.SvgBuilder;

namespace Kunukn.XmlVisual.Core.Entities
{
    public class Element : IComparable
    {
        // Metadata
        public int Id { get; private set; }
        public int Level { get; set; } // depth in tree        
        public string Label { get; set; } //used by File System view
        public int TotalChilds { get; set; } //used by File System view

        // SVG related
        public SvgData Svg { get; set; }

        // Properties
        public XmlType Type { get; set; }
        public string Name { get; set; }
        private string _value;
        public string Value
        {
            get { return _value; }
            private set
            {
                _value = value;
            }
        }
        public string ValueShort { get; private set; } // shortened version for truncated display

        //restrict, else very slow in browser due to js tspan loop create/delete
        private const int MessageMaxLength = 800; 
        private const int MessageMaxAttributes = 400;
        private const string Truncated = " (TRUNCATED...)";
        public string Message // full info storage
        {     
            get
            {
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(Name))
                {                                        
                    string str = SvgTool.TruncateText(Name, MessageMaxLength, Truncated);                    
                    sb.Append(string.Concat("Name=", str));
                    if (Attributes.Count > 0 || !string.IsNullOrEmpty(Value))
                        sb.Append("   ");
                }
                if (Attributes.Count > 0)
                {                    
                    var sbAttr = new StringBuilder();
                    sbAttr.Append(string.Concat("Attributes:   "));
                    foreach (var a in Attributes)
                    {                        
                        sbAttr.Append(string.Concat(a.ToString(), "  "));
                        if (sbAttr.ToString().Length > MessageMaxAttributes)
                        {                            
                            sbAttr.Append(string.Concat(Truncated));
                            break;
                        } 
                    }                                            
                    sb.Append(sbAttr.ToString() );
                }
                if (!string.IsNullOrEmpty(Value))
                {
                    string str = SvgTool.TruncateText(Value, MessageMaxLength, Truncated);                                      
                    sb.Append(string.Concat("Value=", str));
                }

                return sb.ToString();
            }
        }
        
        public ListElement Childs { get; private set; }
        public List<Attribute> Attributes { get; private set; }
        public Element Parent { get; private set; }

        public Element(Element parent, XmlType type, int level, int elementId)
        {
            Id = elementId;
            Childs = new ListElement();
            Attributes = new List<Attribute>();
            Parent = parent;
            Type = type;
            Level = level;
            Svg = new SvgData();
            Name = string.Empty;
        }

        public void SetValue(string s, UserConfig userConfig)
        {
            Value = s;
            ValueShort = SvgTool.TruncateText(s, userConfig);
        }

        public bool IsLeaf()
        {
            return Childs == null || Childs.Count == 0;
        }

        public bool HasChildren()
        {
            return !IsLeaf();
        }

        public bool IsSingleChild()
        {
            return this.Parent.Childs.Count == 1;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case XmlType.Node:
                    return "Node=" + Name; //XmlType.Node.GetString()
                case XmlType.Cdata:
                    return "[CDATA]";
                case XmlType.Comment:
                    return "[Comment]";
                case XmlType.Text:
                    return "Text=" + Value;
                case XmlType.DocType:
                    return "DocType=" + Value;
                case XmlType.ProcessingInstruction:
                    return "PI=" + Value;
                default:
                    throw new Exception("Node invalid id=" + Id);
            }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var other = obj as Element;
            if (other == null)
                return -1;

            return this.Id.CompareTo(other.Id);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(Object o)
        {
            var other = o as Element;
            if (other == null)
                return false;

            return this.Id == other.Id;
        }

        #endregion
    }
}
