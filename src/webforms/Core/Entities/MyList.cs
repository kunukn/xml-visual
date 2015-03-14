using System;
using System.Collections.Generic;

namespace Kunukn.XmlVisual.Core.Entities
{
    public class MyList<T> : List<T> where T : new()
    {
//        private T _genericInstance = default(T);
        public MyList() : base()
        {
//            _genericInstance = Activator.CreateInstance<T>();
        }      
    }
}
