using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Kunukn.XmlVisual.Core.Extensions;
using Kunukn.XmlVisual.Core.Entities;

namespace Kunukn.XmlVisual.Core
{
    /// <summary>
    /// Kunuk Nykjaer
    /// </summary>
    public static class Config
    {
        public const string Version = "Xml Visual ver. Alpha 0.6";

        public static FileInfo PathInXml { get; private set; }        
        public static FileInfo PathOutSvg { get; private set; }
        public static VisualizationType VisualizationType { get; set; }
        public static bool IncludeDeclaration { get; private set; }
        public static bool IncludeProcessingInstruction { get; private set; }
        public static bool IncludeAttribute { get; private set; }
        public static bool IncludeCdata { get; private set; }
        public static bool IncludeText { get; private set; }
        public static bool IncludeMixedText { get; private set; }        
        public static bool IncludeComment { get; private set; }        
        public static bool FullText { get; private set; }
        public static bool IncludeDocType { get; private set; }
        public static bool Debug { get; private set; }

        // bigraph
        public static bool IncludeBigraph { get; private set; }
        public static int BigraphMinimum { get; private set; }
        public static bool BGIncludeNode { get; private set; }
        public static bool BGIncludeAttributeName { get; private set; }
        public static bool BGIncludeAttributeValue { get; private set; }
        public static bool BGIncludeText { get; private set; }
        public static bool BGIncludeComment { get; private set; }
        public static bool BGIncludeCdata { get; private set; }
        public static HashSet<int> BGIgnore { get; private set; }  //string HashCode
        
        // js setup
        public static int JsGroupFn { get; private set; }

        // gui
        public static bool ObjectsMovable { get; set; }


        // console app
        public static void Init()
        {            
            if (PathInXml != null)
                return; //already init, done

            Debug = Convert.ToBoolean(ConfigurationManager.AppSettings["Debug"]);
            PathInXml = new FileInfo(ConfigurationManager.AppSettings["PathInXml"]);            
            PathOutSvg = new FileInfo(ConfigurationManager.AppSettings["PathOutSvg"]);
            
            VisualizationType = ConfigurationManager.AppSettings["VisualizationType"].ToVisualizationTypeEnum();
            FullText = Convert.ToBoolean(ConfigurationManager.AppSettings["FullText"]);
            
            IncludeDeclaration = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeDeclaration"]);
            IncludeProcessingInstruction = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeProcessingInstruction"]);
            IncludeAttribute = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeAttribute"]);
            IncludeCdata = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeCdata"]);
            IncludeText = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeText"]);
            IncludeMixedText = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeMixedText"]);            
            IncludeComment = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeComment"]);
            IncludeDocType = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeDocType"]);
            
            IncludeBigraph = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeBigraph"]); 
            BigraphMinimum = Convert.ToInt32(ConfigurationManager.AppSettings["BigraphMinimum"]); 
            BGIncludeNode = Convert.ToBoolean(ConfigurationManager.AppSettings["BGIncludeNode"]);
            BGIncludeAttributeName = Convert.ToBoolean(ConfigurationManager.AppSettings["BGIncludeAttributeName"]); 
            BGIncludeAttributeValue = Convert.ToBoolean(ConfigurationManager.AppSettings["BGIncludeAttributeValue"]); 
            BGIncludeText = Convert.ToBoolean(ConfigurationManager.AppSettings["BGIncludeText"]); 
            BGIncludeComment = Convert.ToBoolean(ConfigurationManager.AppSettings["BGIncludeComment"]); 
            BGIncludeCdata = Convert.ToBoolean(ConfigurationManager.AppSettings["BGIncludeCdata"]);

            BGIgnore = new HashSet<int>(); 
            var ignore = ConfigurationManager.AppSettings["BGIgnore"];
            var ignoree = ignore.Split('|');
            foreach (var s in ignoree)            
                BGIgnore.Add(s.GetHashCode());
            

            JsGroupFn = Convert.ToInt32(ConfigurationManager.AppSettings["JsGroupFn"]);

            ObjectsMovable = Convert.ToBoolean(ConfigurationManager.AppSettings["ObjectsMovable"]);              
        }


        public static void UpdatePathIn(FileInfo fi)
        {
            PathInXml = fi;
        }

        public static void ShowConfig()
        {
            foreach (string key in ConfigurationManager.AppSettings)
            {
                var value = ConfigurationManager.AppSettings[key];
                Console.WriteLine(string.Format("Key: {0}, Value: {1}", key, value));
            }
        }

    }
}
