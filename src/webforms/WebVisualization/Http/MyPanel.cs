using System;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: TagPrefix("WebVisualization.Http", "WebVisualization")]
namespace Kunukn.XmlVisual.WebVisualization.Http
{
    public class MyPanel : Panel
    {


        public override string UniqueID
        {
            get
            {
                return this.ID;
            }
        }
        /// <summary>
        /// Override to force simple IDs all around
        /// </summary>
        public override string ClientID
        {
            get
            {
                return this.ID;
            }
        }

    }
}