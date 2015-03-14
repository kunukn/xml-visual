using System;
using System.Web;

namespace Kunukn.XmlVisual.WebVisualization.HttpExtensions
{
    public class SimpleHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {           
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            HttpServerUtility server = context.Server;
            
            response.Write(@"<html><body><h1>SimpleHandler</h1></body></html>");            
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}