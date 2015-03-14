using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kunukn.XmlVisual.Core;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Extensions;
using Kunukn.XmlVisual.Core.SvgBuilder;
using Kunukn.XmlVisual.Core.SvgBuilder.Factory;
using Kunukn.XmlVisual.Core.Utilities;
using Kunukn.XmlVisual.WebVisualization.Http;


namespace Kunukn.XmlVisual.WebVisualization.AreaXmlV
{
    /// <summary>
    /// @Author Kunuk Nykjaer
    /// </summary>
    public partial class Default : System.Web.UI.Page
    {
        public const int MaxXmlInputChars = 100000;
        public const int MaxSvgInputChars = 200000;
        public const int MaxBGIgnore = 400;

        public UserConfig UConfig { get; set; }
        public string ServerPath
        {
            get { return Server.MapPath("~/bin/"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            svgLiteral.Mode = LiteralMode.Encode;
            svgLiteral.Mode = LiteralMode.PassThrough;
            svgLiteral.Mode = LiteralMode.Transform;

            tbXmlInput.TextMode = TextBoxMode.MultiLine;
            tbXmlInput.MaxLength = MaxXmlInputChars;
            tbSvgInput.TextMode = TextBoxMode.MultiLine;
            tbSymbol.TextMode = TextBoxMode.MultiLine;
            tbSvgInput.MaxLength = MaxSvgInputChars;
            lbVersion.Text = Config.Version;
            tbBGIgnore.MaxLength = MaxBGIgnore;

            if (!IsPostBack)
            {
                btnSvgView.Attributes.Add("OnClick", "window.open('/svg.view'); return false;");
                lb1.Text = "[-]";
                lb2.Text = "[+]";
                divCheckBoxListGeneralConfig.CssClass = "DisplayNone";
                lb3.Text = "[+]";
                divCheckBoxListBigraph.CssClass = "DisplayNone";
                tbBGIgnore.Text = "IamIgnored|IamIgnoredToo";

                var lines = FileUtil.ReadFile(ServerPath + "xml/Note.xml");
                var str = lines.GetString();
                MySession.Current.Svg = string.Empty;
                MySession.Current.Xml = str;
                EnableSvgAction(false);
                tbXmlInput.Text = str;
            }
            else
            {

            }
        }

        public void LinkButton_Click(Object sender, EventArgs e)
        {
            LinkButton lb = sender as LinkButton;

            if (lb.Text == "[+]")
            {
                lb.Text = "[-]";
                if (lb.ID == "lb1")
                    divCheckBoxListParse.CssClass = "DisplayInline";
                else if (lb.ID == "lb2")
                    divCheckBoxListGeneralConfig.CssClass = "DisplayInline";
                else if (lb.ID == "lb3")
                    divCheckBoxListBigraph.CssClass = "DisplayInline";
            }
            else
            {
                lb.Text = "[+]";
                if (lb.ID == "lb1")
                    divCheckBoxListParse.CssClass = "DisplayNone";
                else if (lb.ID == "lb2")
                    divCheckBoxListGeneralConfig.CssClass = "DisplayNone";
                else if (lb.ID == "lb3")
                    divCheckBoxListBigraph.CssClass = "DisplayNone";
            }
        }


        public void BtnGenerateClick(object sender, EventArgs e)
        {
            var s = tbXmlInput.Text; //update
            if (s.Length > MaxXmlInputChars)
            {
                lbStatus.Text = string.Format("Error, max chars exceeded {0} " + TimeStamp(), s.Length);
            }
            else
            {
                MySession.Current.Xml = tbXmlInput.Text; //update
                Run();
            }
        }

        static string TimeStamp()
        {
            return DateTime.Now.ToLongTimeString();
        }


        public void BtnDownloadClick(Object sender, EventArgs e)
        {
            lbStatusSvg.Text = "Downloading ..";

            ProcessRequestSvg();

            lbStatusSvg.Text = "Download done: " + TimeStamp();
        }

        public void BtnUpdateSvgClick(Object sender, EventArgs e)
        {
            lbStatusSvg.Text = "Updating ..";
            if (tbSvgInput.Text.Length > MaxSvgInputChars)
            {
                lbStatusSvg.Text = string.Format("Error, max chars exceeded {0} " + TimeStamp(), tbSvgInput.Text.Length);
            }
            else
            {
                MySession.Current.Svg = tbSvgInput.Text;
                lbStatusSvg.Text = "Update done: " + TimeStamp();
            }
        }

        void DoConfig()
        {
            this.UConfig = new UserConfig(ApplicationMode.WebApp) { RootPath = this.ServerPath };

            int type = DropDownList1.SelectedIndex;
            if (type < 0 || type >= DropDownList1.Items.Count)
                type = 0; // tree

            VisualizationType visualizationType = type.ToVisualizationType();
            this.UConfig.Visualization = visualizationType;

            int i = 0;
            UConfig.IncludeAttribute = CheckBoxListParse.Items[i++].Selected;
            UConfig.IncludeMixedText = CheckBoxListParse.Items[i++].Selected;
            UConfig.IncludeCdata = CheckBoxListParse.Items[i++].Selected;
            UConfig.IncludeText = CheckBoxListParse.Items[i++].Selected;
            UConfig.IncludeComment = CheckBoxListParse.Items[i++].Selected;
            UConfig.IncludeDocType = CheckBoxListParse.Items[i++].Selected;
            UConfig.IncludeProcessingInstruction = CheckBoxListParse.Items[i++].Selected;
            UConfig.IncludeDeclaration = CheckBoxListParse.Items[i++].Selected;

            i = 0;
            UConfig.FullText = CheckBoxListGeneralConfig.Items[i++].Selected;
            UConfig.ObjectsMovable = CheckBoxListGeneralConfig.Items[i++].Selected;

            i = 0;
            UConfig.IncludeBigraph = CheckBoxListBigraph.Items[i++].Selected;
            UConfig.BGIncludeNode = CheckBoxListBigraph.Items[i++].Selected;
            UConfig.BGIncludeAttributeName = CheckBoxListBigraph.Items[i++].Selected;
            UConfig.BGIncludeAttributeValue = CheckBoxListBigraph.Items[i++].Selected;
            UConfig.BGIncludeText = CheckBoxListBigraph.Items[i++].Selected;


            if (UConfig.IncludeBigraph && !string.IsNullOrWhiteSpace(tbBGIgnore.Text) && tbBGIgnore.Text.Length < MaxBGIgnore)
            {
                var set = new HashSet<int>();
                var arr = tbBGIgnore.Text.Split('|');
                foreach (var s in arr)
                    set.Add(s.GetHashCode());

                UConfig.BGIgnore = set;
            }

        }

        void Run()
        {
            MySession.Current.Svg = string.Empty; //clear
            var xml = MySession.Current.Xml;

            var instance = Application[Global.CacheInstance] as Cache;            
            instance.InitServer(this.ServerPath);

            DoConfig();

            if (!string.IsNullOrEmpty(xml))
            {
                var xmlTrimmed = xml.Trim(); //remove leading and trailing whitespace
                string invalidXml = XmlParser.ValidateByString(xmlTrimmed);

                if (!string.IsNullOrEmpty(invalidXml))
                {
                    // invalid
                    lbStatus.Text = "Xml error see Svg xml tab: " + TimeStamp();
                    tbSvgInput.Text = invalidXml;
                    EnableSvgAction(false);
                }
                else
                {
                    SvgBuilder builder = SvgBuilderFactory.Factory(this.UConfig.Visualization);

                    XmlMetadata nodeMetadata = XmlParser.Parse(xmlTrimmed, false, this.UConfig);
                    if (nodeMetadata.IsValid)
                    {
                        var svgFile = builder.BuildSvg(nodeMetadata);
                        var svgData = svgFile.CreateFile();
                        tbSvgInput.Text = svgData;
                        EnableSvgAction(true);
                        MySession.Current.Svg = svgData;
                        tbSymbol.Text = Core.Utilities.Print.InternalRepresentation(nodeMetadata);

                        lbStatus.Text = "SVG generated: " + TimeStamp();
                    }
                    else
                    {
                        lbStatus.Text = "Xml error see SVG tab: " + TimeStamp();
                        tbSvgInput.Text = nodeMetadata.ErrorMessage;
                        EnableSvgAction(false);
                    }
                }
            }
            else
            {
                lbStatus.Text = "No data in input field: " + TimeStamp();
                tbSvgInput.Text = string.Empty;
                tbSymbol.Text = string.Empty;
                EnableSvgAction(false);
            }
        }


        public void EnableSvgAction(bool b)
        {
            btnSvgView.Enabled = b;
            btnDownload.Enabled = b;
            btnUpdateSvg.Enabled = b;
        }

        public void ProcessRequestSvg()
        {
            string svg = MySession.Current.Svg;
            if (string.IsNullOrEmpty(svg))
                return;

            Response.Clear();
            //Turn off Caching and enforce a content type that will prompt to download/save.
            Response.AddHeader("Connection", "close");
            Response.AddHeader("Cache-Control", "private");
            Context.Response.ContentType = "image/svg+xml";//            
            Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "xmlview.svg"));

//            Response.Output.WriteLine(@"<svg />");
            //            Response.Output.WriteLine(svg);
//            Response.Write(@"<svg />");
            Response.Write(svg);
            Response.End();

//            Response.Flush();
            //            Response.Close();
        }


        protected void BtnSvgViewClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MySession.Current.Svg))
                Redirect();
        }

        public void Redirect()
        {
            //            Server.Transfer("svg.object");
            Response.Redirect("svg.object", false);
        }

        protected void BtnGetAllTextClick(object sender, EventArgs e)
        {
            AjaxControlToolkit.TabContainer container = (AjaxControlToolkit.TabContainer)TabContainer1;
            foreach (object obj in container.Controls)
            {
                if (obj is AjaxControlToolkit.TabPanel)
                {
                    AjaxControlToolkit.TabPanel tabPanel = (AjaxControlToolkit.TabPanel)obj;
                    foreach (object ctrl in tabPanel.Controls)
                    {
                        if (ctrl is Control)
                        {
                            System.Web.UI.Control c = (Control)ctrl;
                            foreach (object innerCtrl in c.Controls)
                            {
                                if (innerCtrl is System.Web.UI.WebControls.TextBox)
                                    Response.Write(((TextBox)innerCtrl).Text);
                            }
                        }
                    }
                }
            }

        }
    }
}