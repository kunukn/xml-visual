using System.Collections.Generic;
using Kunukn.XmlVisual.Core.Extensions;

namespace Kunukn.XmlVisual.Core.Entities
{
    public class UserConfig
    {
        public ApplicationMode ApplicationMode { get; set; }  // consoleapp, webapp


        public  VisualizationType Visualization { get; set; }
        
        // parse
        public  bool IncludeDeclaration { get;  set; }
        public  bool IncludeProcessingInstruction { get;  set; }
        public  bool IncludeAttribute { get;  set; }
        public  bool IncludeCdata { get;  set; }
        public  bool IncludeText { get;  set; }
        public  bool IncludeMixedText { get;  set; }
        public  bool IncludeComment { get;  set; }        
        public  bool FullText { get;  set; }
        public bool IncludeDocType { get; set; }
        public bool Debug { get; set; }

        // bigraph
        public  bool IncludeBigraph { get;  set; }
        public  int BigraphMinimum { get;  set; }
        public  bool BGIncludeNode { get;  set; }
        public  bool BGIncludeAttributeName { get;  set; }
        public bool BGIncludeAttributeValue { get; set; }
        public  bool BGIncludeText { get;  set; }
        public  bool BGIncludeComment { get;  set; }
        public  bool BGIncludeCdata { get;  set; }
        public HashSet<int> BGIgnore { get; set; }  //string HashCode
        
        public string RootPath { get; set; }
        
        // js setup
        public int JsGroupFn { get; set; }

        // gui
        public bool ObjectsMovable { get; set; }



        public string GetSvgTitle()
        {
            var s = "";
            if( Config.PathInXml!=null)
            {
                s = Config.PathInXml.Name + " ";
            }
            return s + Visualization.GetString() + " view";
        }


        public UserConfig(ApplicationMode mode)
        {
            ApplicationMode = mode;
            RootPath = "";

            // defaults
            Visualization = VisualizationType.Unknown;
            JsGroupFn = 1; //default g1()
            ObjectsMovable = false;
            
            // until setting available through web gui, default values
            BigraphMinimum = 2;
            BGIncludeNode = false;
            BGIncludeAttributeValue = true;
            BGIncludeText = false;
            BGIncludeComment = false;
            BGIncludeCdata = false;
            BGIgnore = new HashSet<int>();
//            BGIgnore.Add("iAmIgnored".GetHashCode());
//            BGIgnore.Add("iAmIgnoredToo".GetHashCode());


            if (mode == ApplicationMode.ConsoleApp)
            {
                Visualization = Config.VisualizationType;
                IncludeDeclaration = Config.IncludeDeclaration;
                IncludeProcessingInstruction = Config.IncludeProcessingInstruction;
                IncludeDocType = Config.IncludeDocType;
                IncludeAttribute = Config.IncludeAttribute;
                IncludeCdata = Config.IncludeCdata;
                IncludeText = Config.IncludeText;
                IncludeMixedText = Config.IncludeMixedText;
                IncludeComment = Config.IncludeComment;                
                FullText = Config.FullText;                
                Debug = Config.Debug;

                // bigraph
                IncludeBigraph = Config.IncludeBigraph;
                BigraphMinimum = Config.BigraphMinimum;
                BGIncludeNode = Config.BGIncludeNode;
                BGIncludeAttributeName = Config.BGIncludeAttributeName;
                BGIncludeAttributeValue = Config.BGIncludeAttributeValue;
                BGIncludeText = Config.BGIncludeText;
                BGIncludeComment = Config.BGIncludeComment;
                BGIncludeCdata = Config.BGIncludeCdata;
                BGIgnore = Config.BGIgnore;

                // js
                JsGroupFn = Config.JsGroupFn;

                // gui
                ObjectsMovable = Config.ObjectsMovable;
            }
        }
    
    }
}
