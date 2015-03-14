using System;
using System.IO;
using System.Text;
using System.Web.UI;

namespace Kunukn.XmlVisual.WebVisualization.HttpTools
{
    public static class Util
    {

        public static string RenderControl(Control ctrl)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            ctrl.RenderControl(hw);
            return sb.ToString();
        }



        public static Control FindControlRecursive(Control Root, string Id)
        {
            //var v = this.Master.FindControl("Content").FindControl("svgLiteral"); // not using master
            if (Root.ID == Id)
                return Root;

            foreach (Control Ctl in Root.Controls)
            {
                Control FoundCtl = FindControlRecursive(Ctl, Id);
                if (FoundCtl != null)
                    return FoundCtl;
            }

            return null;
        }

    }
}