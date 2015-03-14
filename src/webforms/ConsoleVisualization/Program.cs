using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Kunukn.XmlVisual.Core;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.SvgBuilder;
using Kunukn.XmlVisual.Core.SvgBuilder.Factory;
using Kunukn.XmlVisual.Core.Utilities;
using SvgFile = Kunukn.XmlVisual.Core.Entities.SvgFile;
using Kunukn.XmlVisual.Core.Extensions;

namespace Kunukn.XmlVisual.ConsoleVisualization
{
    /// <summary>
    /// @Author Kunuk Nykjaer
    /// </summary>
    class Program
    {
        public static void Main(string[] args)
        {
            DateTime begin = DateTime.Now;
            Console.WriteLine(string.Format("-- {0} --", Config.Version));
            Config.Init();

            Run(new UserConfig(ApplicationMode.ConsoleApp));


            #region :: TEST ::
            //PrintXml();
            //                        PrintXmlInternalRepresentation();
            //            SvgTest();
            //            Test();
            //            CreateXmlTest2();
            //            TestPolar();
//                        Temp();

            #endregion :: TEST ::

            var end = DateTime.Now;
            System.TimeSpan done = end.Subtract(begin);
            Console.WriteLine("duration: " + done.TotalSeconds + " sec.\nPress any key to exit ...");
            Console.ReadKey();
        }


        static void Run(UserConfig userConfig)
        {
            SvgBuilder builder = SvgBuilderFactory.Factory(userConfig.Visualization);
            if (!Config.PathInXml.Exists)
            {
                //try adding .xml
                FileInfo fi = new FileInfo(Config.PathInXml.FullName + ".xml");
                if (!fi.Exists)
                    throw new Exception("File dont exists: " + Config.PathInXml.FullName);
                Config.UpdatePathIn(fi);
            }
            string path = Config.PathInXml.FullName;
            PL(Config.PathInXml.Name);
            string validXml = XmlParser.Validate(path);

            if (!string.IsNullOrEmpty(validXml))
            {
                var lines = FileUtil.ReadFile(new FileInfo(@"xml\ErrorMessage.svg"));
                var str = lines.GetString();
                str = str.Replace("ERROR_MESSAGE", validXml);
                FileUtil.WriteFile(str, Config.PathOutSvg.Name);
            }
            else
            {
                XmlMetadata nodeMetadata = XmlParser.Parse(path, true, userConfig);
                if (nodeMetadata.IsValid)
                {
                    var svg = builder.BuildSvg(nodeMetadata);
                    FileUtil.WriteFile(svg.CreateFile(), Config.PathOutSvg.Name);
                    PL(Config.PathOutSvg.Name);
                }
                else
                {
                    var lines = FileUtil.ReadFile(new FileInfo(@"xml\ErrorMessage.svg"));
                    var str = lines.GetString();
                    str = str.Replace("ERROR_MESSAGE", nodeMetadata.ErrorMessage);
                    FileUtil.WriteFile(str, Config.PathOutSvg.Name);
                }
            }
        }

        #region :: Testing ::


        static void Temp()
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.Append("<svg  version='1.1' baseProfile='full' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' xmlns:ev='http://www.w3.org/2001/xml-events'>" + Environment.NewLine);
            sb.Append("<g transform='translate(120,40)'>" + Environment.NewLine);

            // <path d="M cx+x1 cy+y1 A r r 0 0 1 cx+x2 cy+y2 L cx cy z" fill="blue" stroke="black" stroke-width="2"/>

            int cx = 100;
            int cy = 100;
            int r0 = 40;
            int r1 = r0 * 2;
            int r2 = r0 * 3;
            int span = 4;

            var e1 = new Core.Entities.SunburstData();            
            e1.ThetaLeftRestrict = MathTool.AngleToRadian(-45);
            e1.ThetaRightRestrict = MathTool.AngleToRadian(45);
            e1.RootCenter = new Point(cx, cy);
            e1.Radius = r0 + span;
            e1.Radius2 = r1;

//            e1.Init();

            var e2 = new SunburstData();
            e2.ThetaLeftRestrict = MathTool.AngleToRadian(0);
            e2.ThetaRightRestrict = MathTool.AngleToRadian(359);
            e2.RootCenter = new Point(cx, cy);
            e2.Radius = r0;// +span;
            e2.Radius2 = r1;
            e2.Init();

            var e11 = new SunburstData();            
            e11.ThetaLeftRestrict = e1.ThetaLeftRestrict;
            e11.ThetaRightRestrict = MathTool.AngleToRadian(0);
            e11.RootCenter = new Point(e1.RootCenter.X, e1.RootCenter.Y);
            e11.Radius = r1 + span;
            e11.Radius2 = r2;
//            e11.Init();
            

            const string _ = " ";

            sb.Append(string.Format("<circle cx='{0}' cy='{1}' r='{2}'  fill='red' stroke='black' stroke-width='1'/>", cx, cy, r0));

            //sb.Append(string.Format("<circle stroke='black' cx='100' cy='100' r='{0}'  fill='red'  />" + Environment.NewLine, r0));

            sb.Append(SunburstData.TestGetSvg(e2) + Environment.NewLine);
//            sb.Append(SunburstData.GetSvg(e1) + Environment.NewLine);
//            sb.Append(SunburstData.GetSvg(e11) + Environment.NewLine);





            sb.Append("</g></svg>");

            FileUtil.WriteFile(sb.ToString(), "temp.svg");
        }


        static void TestPolar()
        {

            double t, r, t2, r2, t3, r3, t4, r4;
            MathTool.CartesianToPolar(13, 2, out t, out r);
            MathTool.CartesianToPolar(-101, 40, out t2, out r2);
            MathTool.CartesianToPolar(40, -112, out t3, out r3);
            MathTool.CartesianToPolar(-3, -1, out t4, out r4);
            var d = MathTool.RadiansToAngle(t);
            var d2 = MathTool.RadiansToAngle(t2);
            var d3 = MathTool.RadiansToAngle(t3);
            var d4 = MathTool.RadiansToAngle(t4);

            double x, y;
            MathTool.PolarToCartesian(t, r, out x, out y);
            MathTool.PolarToCartesian(t2, r2, out x, out y);
            MathTool.PolarToCartesian(t3, r3, out x, out y);
            MathTool.PolarToCartesian(t4, r4, out x, out y);
        }

        static void PrintXml()
        {
            string path = Config.PathInXml.FullName;// @"xml\KML.xml"; //Bygning2 Error Weather KML
            XmlDocument validateXml = XmlParser.GetValidated(path); //validate xml document            
            XmlMetadata nodeMetadata = XmlParser.Parse(path, true, new UserConfig(ApplicationMode.ConsoleApp)); //path "temp.svg"
            var str = Print.NodeMetadata(nodeMetadata);
            PL(str);
            //            FileUtil.WriteFile(str, "parsed.txt");
        }

        static void PrintXmlInternalRepresentation()
        {
            string path = Config.PathInXml.FullName;// @"xml\KML.xml"; //Bygning2 Error Weather KML
            XmlDocument validateXml = XmlParser.GetValidated(path); //validate xml document            
            XmlMetadata nodeMetadata = XmlParser.Parse(path, true, new UserConfig(ApplicationMode.ConsoleApp)); //path "temp.svg"
            var str = Print.InternalRepresentation(nodeMetadata);
            PL(str);
            //            FileUtil.WriteFile(str, "parsed.txt");
        }

        static void CreateXmlTest()
        {
            XElement xml = new XElement("people",
                    new XElement("person",
                        new XElement("id", 1),
                        new XElement("firstname", "Carl"),
                        new XElement("lastname", "Lewis"),
                        new XElement("idrole", 2)));
            Console.WriteLine(xml);
            FileUtil.WriteFile(xml.ToString(), "temp.xml");
        }
        static void CreateXmlTest2()
        {
            var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"), //bug utf-16 is always used
                new XElement("people",
                    new XElement("idperson",
                    new XAttribute("id", 1),
                    new XAttribute("year", 2004),
                    new XAttribute("salaryyear", "10000,0000"))));

            var xml = new System.IO.StringWriter();
            doc.Save(xml);
            Console.WriteLine(xml);
            FileUtil.WriteFile(xml.ToString(), "temp.xml");
        }
        static void SvgTest()
        {
            var svg = new SvgFile(new UserConfig(ApplicationMode.ConsoleApp));
            svg.GraphicContent += "<title>SVG testing</title>" + Environment.NewLine;
            //            svg.GraphicContent = "<circle cx='50' cy='50' r='30' stroke='black' stroke-width='2' fill='red'/>";
            svg.GraphicContent += "<path class='StrokeGreen FillYellow DisplayInline' "
            + "d='M250,140 c 0,0 -16,24 -16,33 c 0,9 7,15 16,15 c 9,0 16,-7 16,-15 c 0,-9 -16,-33 -16,-33'/>";
            FileUtil.WriteFile(svg.CreateFile(), "temp.svg");

        }
        static void Test()
        {
            var xmlLines = FileUtil.ReadFile(Config.PathInXml);
            P(xmlLines.Show());
            //            XElement elem = XElement.Load(Config.PathInXml, LoadOptions.PreserveWhitespace);
            XElement elem = XElement.Load(Config.PathInXml.FullName, LoadOptions.None);
            P(elem.Name);
            foreach (var attrib in elem.Attributes())
            {
                Console.WriteLine("> " + attrib.Name + " = " + attrib.Value);
            }

            var elements = elem.Elements();
            foreach (var e in elements)
            {
                P(e.Name);
            }

            foreach (var el in elem.Descendants())
            {
                Console.WriteLine(el.Name + " " + el.Value);
                foreach (var attrib in el.Attributes())
                {
                    Console.WriteLine("> " + attrib.Name + " = " + attrib.Value);
                }
            }

            //            var svgLines = Parser.Parse(xmlLines);
            //            FileUtil.WriteFile(svgLines, Config.PathOutSvg);

            //            var v = ParserModule.Servers;
            //            var vv = ParserModule.Infos;
            //            var vvv = ParserModule.Fileformats;
        }
        #endregion



        static void PL(object o)
        {
            Console.WriteLine(o);
        }
        static void P(object o)
        {
            Console.Write(o);
        }
    }
}
