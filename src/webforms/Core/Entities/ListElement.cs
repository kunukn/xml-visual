using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunukn.XmlVisual.Core.Entities
{
    public class ListElement : List<Element>
    {
        public ListElement() : base() { }

        //        public new void Add(Element e)
        public void Add(Element element) //hide List.Aadd()
        {            
            base.Add(element);
            if(element.Parent.Label==null)
                element.Label = element.Parent.Childs.Count.ToString();
            else
                element.Label = element.Parent.Label + "_" + element.Parent.Childs.Count;

            Element e = element;
            while (e.Parent != null)
            {
                e = e.Parent;
                e.TotalChilds++;
            }
        }

    }
}
