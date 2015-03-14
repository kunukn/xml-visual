using System;
using System.Web;

namespace Kunukn.XmlVisual.WebVisualization.Http
{
    public class MySession
    {
        // private constructor
        private MySession() { }

        // Gets the current session.
        public static MySession Current
        {
            get
            {
                MySession session =
                  (MySession)HttpContext.Current.Session["__MySession__"];
                if (session == null)
                {
                    session = new MySession();
                    HttpContext.Current.Session["__MySession__"] = session;
                }
                return session;
            }
        }

        // session properties
        public string Xml { get; set; }
        public string Svg { get; set; }
        
        // not used
        public int LoginId { get; set; }
        public DateTime LoginTime { get; set; }        
    }
}