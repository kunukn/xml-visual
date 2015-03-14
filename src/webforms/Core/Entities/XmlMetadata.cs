using System;
using System.Collections.Generic;
using System.Linq;
using Kunukn.XmlVisual.Core.Extensions;

namespace Kunukn.XmlVisual.Core.Entities
{
    /// <summary>
    /// Kunuk
    /// Metadata info for visual transformation
    /// </summary>
    public class XmlMetadata
    {
        public readonly static string Dummyroot = "Dummyroot " + DateTime.Now; //unique name
        protected Dictionary<int, Element> ElementsLookup { get; private set; } //element.Id, element
        protected List<Element>[] _elementsAtLevel;

        public UserConfig UConfig { get; set; }
        public List<Element> Nodes { get; private set; }
        public List<Element> Texts { get; private set; }
        public List<Element> Comments { get; private set; }
        public List<Element> Cdatas { get; private set; }
        public int ElementsCount {
            get
            {
                return ElementsLookup.Count;
            }
        }
        public Element[] AllElements{get { return ElementsLookup.Values.ToArray(); }}
        public string XmlDeclaration { get; set; }
        public string Title { get; set; }
        public bool IsViewImplemented { get; set; }
        
        public string Info { get; set; }

        // Bigraph metadata
        public string BigraphItemCount { get; set; }
        public string BigraphTotalCount { get; set; }
        public string BigraphMaxCount { get; set; }
        public string BigraphInfo { get; set; }

        public int Height { get; set; }
        public int MaxWidth { get; private set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public Element Root { get; private set; }

        public XmlMetadata(Element root, UserConfig userConfig)
        {
            this.UConfig = userConfig;
            root.Name = Dummyroot;
            Root = root;
            ElementsLookup = new Dictionary<int, Element>();            
            Nodes = new List<Element>();
            Texts = new List<Element>();
            Comments = new List<Element>();
            Cdatas = new List<Element>();
            Title = string.Empty;
            IsViewImplemented = false;
        }

        public void Add(Element e)
        {
            ElementsLookup.Add(e.Id, e);
        }


        void DebugPrintLevels()
        {
            Console.WriteLine("** DebugPrintLevels");
            for (int i = 1; i < _elementsAtLevel.Length; i++)
            {
                Console.WriteLine("** " + i);
                var list = _elementsAtLevel[i];
                foreach (var e in list)
                    Console.WriteLine(string.Format("id={0} type={1} level={2} name={3} childs={4}", e.Id, e.Type.GetString(), e.Level, e.Name, e.Childs.Count));
            }
        }      

        public void CalcMetadata()
        {            
            if(!IsValid)
                return;
                        
            int maxheight = 0;
            var elements = ElementsLookup.Values;
            foreach (var e in elements)            
                if (e.Level > maxheight)
                    maxheight = e.Level;
            
            Height = maxheight;

            _elementsAtLevel = new List<Element>[Height+1];
            for (int i = 1; i < _elementsAtLevel.Length; i++)            
                _elementsAtLevel[i] = new List<Element>();
                        
            foreach (var e in elements)
            {
                if (e.Level==0)
                    continue; // skip dummy "root"
                _elementsAtLevel[e.Level].Add(e);
                switch (e.Type)
                {
                    case XmlType.Node:
                        Nodes.Add(e);
                        break;
                    case XmlType.DocType: // fall through
                    case XmlType.ProcessingInstruction: // fall through
                    case XmlType.Text:
                        Texts.Add(e);
                        break;
                    case XmlType.Comment:
                        Comments.Add(e);
                        break;
                    case XmlType.Cdata:
                        Cdatas.Add(e);
                        break;
                    default:
                        throw new Exception("unknown Type: "+e);              
                }
            }

            int maxWidth = 0;
            for (int i = 1; i < _elementsAtLevel.Length; i++)
            {
                var list = _elementsAtLevel[i];
                list.Sort(); // sort, now all children are aligned in list, simple sibling check possible
                int count = list.Sum(e => e.Childs.Count);
                if (count > maxWidth)
                    maxWidth = count; // update
            }
            MaxWidth = maxWidth;          
        }

        public List<Element> GetLevel(int i)
        {
            if (_elementsAtLevel==null || i >= _elementsAtLevel.Length || i <= 0)
                return null;
            return _elementsAtLevel[i];
        }
        public int GetLevelCount(int i)
        {
            var list = GetLevel(i);
            if (list != null)
                return list.Count;
            return -1;
        }

        public void RemoveMixedTextElement()
        {                        
            var remove = new List<Element>();
            foreach (var e in ElementsLookup.Values)            
                if(e.Type==XmlType.Text && !e.IsSingleChild() )                
                    remove.Add(e);                    
                            
            foreach (var e in remove)
            {
                var p = e.Parent;
                p.Childs.Remove(e);
                ElementsLookup.Remove(e.Id);
            }
        }


        #region :: Debug ::
        void DebugPrintMetadata()
        {
            Console.WriteLine("** DebugPrintMetadata");
            Console.WriteLine("Height=" + Height);
            Console.WriteLine("MaxWidth=" + MaxWidth);
            Console.WriteLine("Elements=" + ElementsCount);
            Console.WriteLine("Nodes=" + Nodes.Count);
            Console.WriteLine("Texts=" + Texts.Count);
            Console.WriteLine("Comments=" + Comments.Count);
            Console.WriteLine("Cdatas=" + Cdatas.Count);
        }
        void DebugPrintTextNotSingle()
        {
            Console.WriteLine("** DebugPrintTextNotSingle");
            // text are always leaf, find text where it's parent only has 1 child
            foreach (var element in Texts)
            {
                var parent = element.Parent;
                if (parent.Childs.Count > 1)
                    Console.WriteLine(element);
            }
        }
        void DebugPrintTextSingle()
        {
            Console.WriteLine("** DebugPrintTextSingle");
            // text,comment,cdata are always leaf, find text where it's parent only has 1 child
            foreach (var element in Texts)
            {
                var parent = element.Parent;
                if (parent.Childs.Count == 1)
                    Console.WriteLine(element);
            }
        }
        #endregion
    }
}
