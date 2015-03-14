<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Kunukn.XmlVisual.WebVisualization.AreaXmlV.Default"
    ValidateRequest="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Xml Visual</title>
    <script type="text/javascript" language="javascript">
        function isDefined(item) {
            return item !== null && item !== undefined && item !== "";
        }      
    </script>
    <style type="text/css">
        .BackColorTab
        {
        }
        
        .DisplayNone
        {
            display: none;
        }
        .DisplayInline
        {
            display: inline;
        }
        
        .flow
        {
            color: #FF0000;
            font-size: 120%;
        }
        
        #frame
        {
            margin-left: 0px;
            margin-top: 0px;
            width: 100%;
            height: 640px; /*90% not working in FF IE */
            border: 1px solid #FF0000;
            background-color: #FFFFFF;
        }
        
        .xmltable
        {
        }
        .xmltable td
        {
            vertical-align: top;
        }
        
        .label
        {
            font-weight: bold; /*
            color: #FFF;            
            background-color: #000;
            */
        }
        
        .version
        {
            color: #FF0000;
            font-weight: bold;
        }
        
        a.link
        {
            text-decoration: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server" />
    <div id="divTabContainer">
        <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0">
            <%-- Height="660"--%>
            <ajaxToolkit:TabPanel runat="server" HeaderText="Xml" ID="TabPanel1">
                <ContentTemplate>
                    <asp:Label ID="lbVersion" CssClass="version" runat="server" /><br />
                    <table class="xmltable">
                        <tr>
                            <td width="140">
                                <!-- left -->
                                <asp:Label ID="lbSelectView" CssClass="label" runat="server" Text="Select View type" /><br />
                                <asp:DropDownList ID="DropDownList1" runat="server">
                                    <asp:ListItem Selected="True">Tree</asp:ListItem>
                                    <asp:ListItem>Rectangle</asp:ListItem>
                                    <asp:ListItem>ChineseBoxes</asp:ListItem>
                                    <asp:ListItem>Mindmap</asp:ListItem>
                                    <asp:ListItem>FileSystem</asp:ListItem>
                                    <asp:ListItem>Sunburst</asp:ListItem>
                                </asp:DropDownList>
                                <br />
                                <br />
                                <asp:Label ID="lbParse" runat="server" Text="Parse config" CssClass="label" />
                                <asp:LinkButton ID="lb1" CssClass="link" runat="Server" OnClick="LinkButton_Click" />
                                <asp:Panel ID="divCheckBoxListParse" runat="server">
                                    <asp:CheckBoxList ID="CheckBoxListParse" runat="server">
                                        <asp:ListItem Selected="True">Attribute</asp:ListItem>
                                        <asp:ListItem Selected="True">MixedText</asp:ListItem>
                                        <asp:ListItem Selected="True">Cdata</asp:ListItem>
                                        <asp:ListItem Selected="True">Text</asp:ListItem>
                                        <asp:ListItem Selected="True">Comment</asp:ListItem>
                                        <asp:ListItem Selected="True">DocType</asp:ListItem>
                                        <asp:ListItem Selected="True">ProcessInstruc</asp:ListItem>
                                        <asp:ListItem Selected="False">Declaration</asp:ListItem>
                                    </asp:CheckBoxList>
                                </asp:Panel>
                                <br /><br />
                                <asp:Label ID="lbGeneralConfig" runat="server" Text="General config" CssClass="label" />
                                <asp:LinkButton ID="lb2" CssClass="link" runat="Server" OnClick="LinkButton_Click" />
                                <asp:Panel ID="divCheckBoxListGeneralConfig" runat="server">
                                    <asp:CheckBoxList ID="CheckBoxListGeneralConfig" runat="server">
                                        <asp:ListItem Selected="False">Full Text</asp:ListItem>
                                        <asp:ListItem Selected="False">Obj isMovable</asp:ListItem>
                                    </asp:CheckBoxList>
                                </asp:Panel>
                                <br /><br />
                                <asp:Label ID="lbBigraph" runat="server" Text="Bigraph config" CssClass="label" />
                                <asp:LinkButton ID="lb3" CssClass="link" runat="Server" OnClick="LinkButton_Click" />
                                <asp:Panel ID="divCheckBoxListBigraph" runat="server">
                                    <asp:CheckBoxList ID="CheckBoxListBigraph" runat="server">
                                        <asp:ListItem Selected="False">Show Bigraph</asp:ListItem>
                                        <asp:ListItem Selected="False">Node Name</asp:ListItem>
                                        <asp:ListItem Selected="False">Attribute Name</asp:ListItem>
                                        <asp:ListItem Selected="True">Attribute Value</asp:ListItem>
                                        <asp:ListItem Selected="False">Text value</asp:ListItem>
                                    </asp:CheckBoxList>
                                    <asp:Label ID="lbBGIgnore" runat="server" Text="Ignored text" /><br />
                                    <asp:TextBox ID="tbBGIgnore" Width="120" runat="server" />
                                </asp:Panel>
                                <br />
                                <br />
                                <asp:Button ID="ButtonGenerate" runat="Server" Text="Generate" OnClick="BtnGenerateClick" /><br />
                                <asp:Label ID="lbStatusTitle" runat="server" Text="Status:" /><br />
                                <asp:Label ID="lbStatus" runat="server" Text="Ready" />
                            </td>
                            <td width="6">
                                &nbsp;
                            </td>
                            <td>
                                <!-- right -->
                                <asp:Label ID="lbXml" CssClass="label" runat="server" Text="Insert XML data, max chars 100.000" /><br />
                                <asp:TextBox ID="tbXmlInput" runat="server" Height="500px" Width="780px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" HeaderText="Svg View" ID="TabPanel2">
                <ContentTemplate>
                    <iframe id="frame" src="svg.view" frameborder="0"></iframe>
                    <asp:Literal ID="svgLiteral" runat="server" Text=""></asp:Literal>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel3" runat="server" HeaderText="Svg Xml">
                <ContentTemplate>
                    <table class="xmltable">
                        <tr>
                            <td width="136">
                                <asp:Button ID="btnSvgView" Style="width: 100px" runat="server" Text="Open svg" /><br />
                                <asp:Button ID="btnUpdateSvg" Style="width: 100px" OnClick="BtnUpdateSvgClick" runat="server"
                                    Text="Update" /><br />
                                <asp:Button ID="btnDownload" Style="width: 100px" OnClick="BtnDownloadClick" runat="server"
                                    Text="Download" /><br />
                                <asp:Label ID="lbStatusSvg" runat="server" Text=" " /><br />
                            </td>
                            <td width="6">
                                &nbsp;
                            </td>
                            <td>
                                <asp:Label ID="lbSvgData" runat="server" Text="Generated SVG data, max chars 200.000 when using update button" /><br />
                                <asp:TextBox ID="tbSvgInput" runat="server" Height="500px" Width="780px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
             <ajaxToolkit:TabPanel ID="TabPanel6" runat="server" HeaderText="Symbol">
                <ContentTemplate>
                    <table class="xmltable">
                        <tr>
                            <td width="136">                                
                            </td>
                            <td width="6">
                                &nbsp;
                            </td>
                            <td>
                                <asp:Label ID="lbSymbol" runat="server" Text="Internal representation" /><br />
                                <asp:TextBox ID="tbSymbol" runat="server" Height="500px" Width="780px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel4" runat="server" HeaderText="About">
                <ContentTemplate>
                    <h1>
                        Functional Prototype: Generic visualization tool of XML documents</h1>
                    <h3>
                        Explore the structure of the XML documents graphically using the different visualization
                        types</h3>
                    <p>
                        Requires either: IE 9+, Firefox 4+, Chrome 10+, Safari 5+, Opera 11+, Other modern
                        browser which supports SVG 1.1</p>
                    <br />
                    <br />
                    <table>
                        <tr>
                            <td>
                                <span class="flow">XML</span>
                            </td>
                            <td>
                                &nbsp;&#8594;&nbsp;
                            </td>
                            <td>
                                <span class="flow">Parse</span>
                            </td>
                            <td>
                                &nbsp;&#8594;&nbsp;
                            </td>
                            <td>
                                <span class="flow">Transform</span>
                            </td>
                            <td>
                                &nbsp;&#8594;&nbsp;
                            </td>
                            <td>
                                <span class="flow">Svg</span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                document
                            </td>
                            <td>
                                &nbsp;&#8594;&nbsp;
                            </td>
                            <td>
                                internal xml tree representation
                            </td>
                            <td>
                                &nbsp;&#8594;&nbsp;
                            </td>
                            <td>
                                use config and generate visualization
                            </td>
                            <td>
                                &nbsp;&#8594;&nbsp;
                            </td>
                            <td>
                                XML document with embedded js and css
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <p>
                        Following hierarchical visualization types are implemented: <b>Tree, Rectangle, Chinese
                            Boxes, Mindmap, File System</b> and <b>Sunburst</b></p>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="TabPanel5" runat="server" HeaderText="Project">
                <ContentTemplate>
                    <h1>
                        Visualization of XML documents and transformations</h1>
                    <div style="width: 600px;">
                        <p>
                            Is it possible to make a generic visualization of XML documents and if so how would
                            the visualization look like?
                            <br />
                            <br />
                            Because XML documents are hierarchically structured miscellaneous hierarchical visualization
                            types has been examined and a few types has been chosen for the experiment. The principle is to keep the GUI
                            as simple as possible. The different view types share the same layout and sizes.
                            Only 6 different universal colors are used (black, white, red, green, blue and yellow)
                            and the interaction uses the same style for Google map for navigating (panning and
                            zoom). Svg is a standard which is supported by the modern browsers thus no plugin
                            is required.<br />
                            <br />
                            Bigraph shows shared values between the elements.
                        </p>
                    </div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </div>
    </form>
</body>
</html>
