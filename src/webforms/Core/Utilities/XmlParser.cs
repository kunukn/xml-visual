using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Extensions;
using Attribute = Kunukn.XmlVisual.Core.Entities.Attribute;
using System.Security;

namespace Kunukn.XmlVisual.Core.Utilities
{
    /// <summary>
    /// Kunuk Nykjaer
    /// </summary>
    public static class XmlParser
    {
        public static string EscapeXml(string str)
        {
            var s = str;
            //            s = System.Security.SecurityElement.Escape(s);            
            return System.Web.HttpUtility.HtmlEncode(s);
        }


        //        public const int MaxElementParsing = 2000; // browsers run dead if to many items, stop here
        public static XmlDocument GetNotValidated(string path)
        {
            // No validation by DTD doctype

            // Create an XmlReaderSettings object. 
            // Set XmlResolver to null, and ProhibitDtd to false.            
            var settings = new XmlReaderSettings { XmlResolver = null, DtdProcessing = DtdProcessing.Ignore }; //ProhibitDtd = false

            // Now, create an XmlReader.  This is a forward-only text-reader based
            // reader of Xml.  Passing in the settings will ensure that validation
            // is not performed.
            var reader = XmlReader.Create(path, settings);
            var doc = new XmlDocument();
            doc.Load(reader);
            return doc;
        }

        public static XmlDocument GetValidated(string path)
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace);
            }

            return doc;
        }

        public static string Validate(string path)
        {
            try
            {
                //UBER slow with doctype validation
//                XmlDocument d = new XmlDocument();
//                using (var tr = new XmlTextReader(path))
//                {
//                    tr.Namespaces = false;
//                    d.XmlResolver = null;
//                    d.Load(tr);
//                }


                // this fails with namespace not decl
                var doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.Load(path);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string ValidateByString(string xmldata)
        {
            try
            {
                //UBER slow with doctype validation
//                XmlDocument d = new XmlDocument();
//                using (var tr = new XmlTextReader(new StringReader(xmldata)))
//                {
//                    tr.Namespaces = false;
//                    d.XmlResolver = null;
//                    d.Load(tr);
//                }


                // this fails with namespace not decl
                                var doc = new XmlDocument();
                                doc.XmlResolver = null;                
                                doc.LoadXml(xmldata);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        public static XmlMetadata Parse(string pathOrXmlstring, bool isPath, UserConfig userConfig) //xmldata or path to xmldata
        {
            var data = pathOrXmlstring;

            // put here because used by console and webapp
            Reset.Run(); // reset for every process xml => parse => transform => svg

            DtdProcessing dtdProcessing = DtdProcessing.Ignore;
            if (userConfig.IncludeDocType) dtdProcessing = DtdProcessing.Parse; //never include DocType then comment out

            int nextElementId = 0;
            var node = new Element(null, XmlType.Node, 0, nextElementId++);
            var nodeMetadata = new XmlMetadata(node, userConfig);
            int height = 0;

            try
            {
                // Create an XmlReaderSettings object. 
                // Set XmlResolver to null, and ProhibitDtd to false for no validation else slow for misc. doctypes e.x. SVG
                XmlReaderSettings settings = new XmlReaderSettings { XmlResolver = null, DtdProcessing = dtdProcessing }; //ProhibitDtd = false is obsolete

                //UBER slow doctype validation
//                using (var xtr = isPath ? new XmlTextReader(data) : new XmlTextReader(new StringReader(data)))
//                {
//                    xtr.Namespaces = false; //namespaces dont need to be defined
//                    using (var reader = XmlReader.Create(xtr, settings))

                //namespace must be decl
                    using (var reader = isPath ? XmlReader.Create(data, settings) : XmlReader.Create(new StringReader(data), settings)) // no validation by DTD doctype
                    {
                        int level = 1;
                        Element newnode;

                        // Read the line of the xml file
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    newnode = new Element(node, XmlType.Node, level, nextElementId++) { Name = EscapeXml(reader.Name) };
                                    node.Childs.Add(newnode);
                                    nodeMetadata.Add(newnode);

                                    bool isEmptyElement = reader.IsEmptyElement; // must, when using MoveToAttribute this changes


                                    if (userConfig.IncludeAttribute && reader.HasAttributes)
                                    {
                                        // If element has attributes
                                        for (int i = 0; i < reader.AttributeCount; i++)
                                        {
                                            reader.MoveToAttribute(i);
                                            newnode.Attributes.Add(new Attribute { Name = EscapeXml(reader.Name), Value = EscapeXml(reader.Value) });
                                        }
                                    }
                                    if (isEmptyElement)
                                    {

                                    }
                                    else
                                    {
                                        node = newnode; // point to child
                                        level++;
                                        if (level > height)
                                            height = level;
                                    }

                                    break;
                                case XmlNodeType.Text:
                                    if (!userConfig.IncludeText)
                                        break;

                                    newnode = new Element(node, XmlType.Text, level, nextElementId++);
                                    newnode.SetValue(EscapeXml(reader.Value.OneWhiteSpace()), userConfig);
                                    nodeMetadata.Add(newnode);
                                    node.Childs.Add(newnode);
                                    break;
                                case XmlNodeType.XmlDeclaration:
                                    if (!userConfig.IncludeDeclaration)
                                        break;

                                    nodeMetadata.XmlDeclaration = EscapeXml(reader.Value);
                                    break;
                                case XmlNodeType.ProcessingInstruction:
                                    if (!userConfig.IncludeProcessingInstruction)
                                        break;

                                    newnode = new Element(node, XmlType.ProcessingInstruction, level, nextElementId++);
                                    if (string.IsNullOrEmpty(reader.Name) == false)
                                        newnode.Name = EscapeXml(reader.Name.OneWhiteSpace());
                                    newnode.SetValue(EscapeXml(reader.Value.OneWhiteSpace()), userConfig);
                                    nodeMetadata.Add(newnode);
                                    node.Childs.Add(newnode);
                                    break;

                                case XmlNodeType.DocumentType:
                                    newnode = new Element(node, XmlType.DocType, level, nextElementId++);
                                    if (string.IsNullOrEmpty(reader.Name) == false)
                                        newnode.Name = EscapeXml(reader.Name.OneWhiteSpace());
                                    newnode.SetValue(EscapeXml(reader.Value.OneWhiteSpace()), userConfig);
                                    nodeMetadata.Add(newnode);
                                    node.Childs.Add(newnode);
                                    break;

                                case XmlNodeType.Comment:
                                    if (!userConfig.IncludeComment)
                                        break;

                                    newnode = new Element(node, XmlType.Comment, level, nextElementId++);
                                    newnode.SetValue(EscapeXml(reader.Value.OneWhiteSpace()), userConfig);
                                    nodeMetadata.Add(newnode);
                                    node.Childs.Add(newnode);
                                    break;
                                case XmlNodeType.CDATA:
                                    if (!userConfig.IncludeCdata)
                                        break;

                                    newnode = new Element(node, XmlType.Cdata, level, nextElementId++);
                                    newnode.SetValue(EscapeXml(reader.Value.OneWhiteSpace()), userConfig);
                                    nodeMetadata.Add(newnode);
                                    node.Childs.Add(newnode);
                                    break;

                                case XmlNodeType.EndElement:
                                    level--;
                                    node = node.Parent; // point to parent                                
                                    break;

                                case XmlNodeType.Whitespace: //not parsed
                                    break;

                                case XmlNodeType.SignificantWhitespace: //not parsed
                                    break;

                                case XmlNodeType.EntityReference: //not parsed within DOCTYPE
                                    break;

                                case XmlNodeType.Notation: //not parsed within DOCTYPE
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
//                }
                nodeMetadata.IsValid = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                nodeMetadata.ErrorMessage = ex.Message;
            }

            if (!userConfig.IncludeMixedText)
                nodeMetadata.RemoveMixedTextElement(); //must be before CalcMetadata()

            nodeMetadata.CalcMetadata();

            return nodeMetadata;
        }




        public static XDocument CloneRemoveDocType(string xml)
        {
            XDocument loaded = XDocument.Parse(xml, LoadOptions.SetLineInfo);
            var clone = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                loaded.LastNode
                );

            return clone;
        }
    }
}
