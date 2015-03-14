using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Extensions;
using Kunukn.XmlVisual.Core.Utilities;

namespace Kunukn.XmlVisual.Core
{
    /// <summary>
    /// Kunuk Nykjaer
    /// </summary>
    public sealed class Cache
    {
        private static string _rootPath = @"";
        private static string _cssCorePath = @"css\svg.css";
        private static string _jsCorePath = @"js\svg.min.js";
        private static string _defsCorePath = @"svg\defs.svg";
        private static string _svgCorePath = @"svg\core.svg";

        private static bool _isServerAlreadyInit = false;

        private static readonly Dictionary<string, string> LookupCss = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> LookupJs = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> LookupDefs = new Dictionary<string, string>();
        
        public readonly string CurrentWorkingDirectory;

        // singleton
        private static Cache _cache;
        private static readonly object Padlock = new object();
        private Cache()
        {
            CurrentWorkingDirectory = Directory.GetCurrentDirectory();
            BuildCache();
        }
        public static Cache Instance //thread safe
        {
            get
            {
                lock (Padlock)
                {
                    return _cache ?? (_cache = new Cache());                
                }
            }
        }     
        // end singleton


        private static void BuildCache()
        {
            foreach (var type in Enum.GetNames(typeof(Core.Entities.VisualizationType)))
            {
                var css = _rootPath + string.Format(@"css\svg{0}.css", type);
                var js = _rootPath + string.Format(@"js\svg{0}.js", type);
                var defs = _rootPath + string.Format(@"svg\defs{0}.svg", type);

                if (!LookupCss.ContainsKey(type))
                {
                    var fileInfo = new FileInfo(css);
                    if (fileInfo.Exists)
                        LookupCss.Add(type, FileUtil.ReadFile(fileInfo).GetString());
                }
                if (!LookupJs.ContainsKey(type))
                {
                    var fileInfo = new FileInfo(js);
                    if (fileInfo.Exists)
                        LookupJs.Add(type, FileUtil.ReadFile(fileInfo).GetString());
                }
                if (!LookupDefs.ContainsKey(type))
                {
                    var fileInfo = new FileInfo(defs);
                    if (fileInfo.Exists)
                        LookupDefs.Add(type, FileUtil.ReadFile(fileInfo).GetString());
                }
            }
        }

        public void InitServer(string serverpath)
        {
            if (_isServerAlreadyInit)
                return;

            _rootPath = serverpath;

            _cssCorePath = _cssCorePath.Contains(serverpath) ? _cssCorePath : serverpath + _cssCorePath;
            _jsCorePath = _jsCorePath.Contains(serverpath) ? _jsCorePath : serverpath + _jsCorePath;
            _defsCorePath = _defsCorePath.Contains(serverpath) ? _defsCorePath : serverpath + _defsCorePath;
            _svgCorePath = _svgCorePath.Contains(serverpath) ? _svgCorePath : serverpath + _svgCorePath;

            BuildCache();
            
            _isServerAlreadyInit = true;
        }


        private string _cssCore;        
        public string Css(UserConfig userConfig)
        {

            if (_cssCore == null)
                _cssCore = FileUtil.ReadFile(_cssCorePath).GetString();

            var css = new StringBuilder(_cssCore);

            var type = userConfig.Visualization.GetString();
            if (LookupCss.ContainsKey(type))
                css.Append( string.Concat(Environment.NewLine,LookupCss[type]) );

            return css.ToString();
        }

        private StringBuilder _jsCore;        
        public string Js(UserConfig userConfig)
        {
            if (_jsCore == null)
                _jsCore = new StringBuilder(FileUtil.ReadFile(_jsCorePath).GetString());

            const string sfalse = "var ObjectsMovable = false;";
            const string strue = "var ObjectsMovable = true;";
            if (userConfig.ObjectsMovable) _jsCore.Replace(sfalse, strue);
            else _jsCore.Replace(strue, sfalse);

            var js = new StringBuilder(_jsCore.ToString());
            var type = userConfig.Visualization.GetString();
            if (LookupJs.ContainsKey(type))
                js.Append( string.Concat(Environment.NewLine,LookupJs[type]));

            return js.ToString();
        }

        private string _defsCore;        
        public string Defs(UserConfig userConfig)
        {
            if (_defsCore == null)
                _defsCore = FileUtil.ReadFile(_defsCorePath).GetString();

            var defs = new StringBuilder(_defsCore);

            var type = userConfig.Visualization.GetString();
            if (LookupDefs.ContainsKey(type))
                defs.Append( string.Concat(Environment.NewLine,LookupDefs[type]) );

            return defs.ToString();

        }
        private string _coreSvg;
        public string CoreSvg
        {
            get
            {
                if (_coreSvg == null)
                {
                    var lines = FileUtil.ReadFile(_svgCorePath);
                    _coreSvg = lines.GetString();
                    _coreSvg = _coreSvg.Replace("$$STRING_REPLACE_VERSION_ID", Config.Version);
                }
                return _coreSvg;
            }
        }

    }
}
