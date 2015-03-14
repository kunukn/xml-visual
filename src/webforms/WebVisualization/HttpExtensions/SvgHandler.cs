using System;
using System.Web;
using System.Web.SessionState;
using Kunukn.XmlVisual.WebVisualization.Http;


namespace Kunukn.XmlVisual.WebVisualization.HttpExtensions
{
    public class SvgHandler : IHttpHandler, IReadOnlySessionState
    {
        const string SvgNoData = @"<?xml version='1.0' encoding='utf-8' standalone='yes'?> " +
                                   @"<svg version='1.1' baseProfile='full' xmlns='http://www.w3.org/2000/svg'>"
                                   + @"<text x='30' y='40'>Svg contains no data</text>"
                                   + @"</svg>";

        public void ProcessRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            HttpServerUtility server = context.Server;

            var svg = MySession.Current.Svg;


            //            response.AddHeader("Connection", "close");
            //            response.AddHeader("Cache-Control", "private");            
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.ContentType = "image/svg+xml";
            response.Write(string.IsNullOrEmpty(svg) ? SvgNoData : svg);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}