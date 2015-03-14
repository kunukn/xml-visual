using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Extensions;

namespace Kunukn.XmlVisual.Core.SvgBuilder.Factory
{    
    public abstract class SvgBuilder
    {        
        protected static string NL = Environment.NewLine;
        const int FontCharWidth = 7;
        public UserConfig UConfig { get; set; }
             
        public virtual SvgFile BuildSvg(XmlMetadata data)
        {
            this.UConfig = data.UConfig;

            var svg = new SvgFile(this.UConfig);
            var sb = new StringBuilder();
            var title = string.Format("{0} {1} view", !Config.PathInXml.Exists ? "" : Config.PathInXml.Name, data.UConfig.Visualization.GetString());
            data.Title = title;
            sb.Append(string.Format("<title>{0}</title>" + NL, title));
            sb.Append(SvgTool.GetMetaText(data));
            sb.Append("<!-- viewport -->" + NL);
            sb.Append("<g id='viewport'>" + NL);
            sb.Append("<text x='100' y='100' transform='scale(2.0)' class='FillRed'>View is not implemented</text>");
            sb.Append("</g>");
            sb.Append("<!-- end viewport -->" + NL + NL);
            svg.GraphicContent = sb.ToString();

            return svg;
        }



        public static string GetBigraphText(XmlMetadata data, out int bigraphItemCount, out int bigraphTotalCount, out int bigraphMaxCount, int xoffset)
        {
            // calc bigraph         
            // find all where string value is shared by at least N diff nodes or texts
            int bigraphCounter = data.UConfig.BigraphMinimum;
            bigraphTotalCount = 0;
            bigraphMaxCount = 0;
            var lookup = new Dictionary<string, int>(); // key, count            
            const int maxBigraphItems = 100; // restrict bigraph items
            foreach (Element e in data.Nodes)
            {
                string key;
                if (string.IsNullOrEmpty(e.Name)) continue; //elem name should not be empty

                key = e.Name;
                if (data.UConfig.BGIncludeNode && !data.UConfig.BGIgnore.Contains(key.GetHashCode()))
                {
                    if (!lookup.ContainsKey(key))
                        lookup.Add(key, 1);
                    else
                        lookup[key]++;
                }
                if (data.UConfig.BGIncludeAttributeName)
                    foreach (Entities.Attribute a in e.Attributes)
                    {
                        key = a.Name;
                        if (string.IsNullOrEmpty(key) || data.UConfig.BGIgnore.Contains(key.GetHashCode())) continue;
                        if (!lookup.ContainsKey(key))
                            lookup.Add(key, 1);
                        else
                            lookup[key]++;
                    }
                if (data.UConfig.BGIncludeAttributeValue )
                    foreach (Entities.Attribute a in e.Attributes)
                    {
                        key = a.Value;
                        if (string.IsNullOrEmpty(key) || data.UConfig.BGIgnore.Contains(key.GetHashCode())) continue;
                        if (!lookup.ContainsKey(key))
                            lookup.Add(key, 1);
                        else
                            lookup[key]++;
                    }
            }

            if (data.UConfig.BGIncludeText)
                foreach (Element e in data.Texts)
                {
                    var key = e.ValueShort;
                    if (string.IsNullOrEmpty(e.Value) || data.UConfig.BGIgnore.Contains(key.GetHashCode())) continue;

                    if (!lookup.ContainsKey(key))
                        lookup.Add(key, 1);
                    else
                        lookup[key]++;
                }

            var set = new HashSet<string>(); // key
            foreach (var i in lookup)
            {
                if (i.Value >= bigraphCounter)
                {
                    set.Add(i.Key);
                    bigraphTotalCount += i.Value;
                    if (i.Value > bigraphMaxCount)
                        bigraphMaxCount = i.Value;
                }
            }
            IEnumerable<string> bigraphData = set.ToList().Take(maxBigraphItems); // restrict
            var lookupPos = new Dictionary<int, Point>();


            // Find bigraph and generate svg  
            // calc bigraphData pos             
            bigraphItemCount = bigraphData.Count();
            int barWidth = (int)(bigraphItemCount * SvgTool.ElementWidth * 1.5);//heuristic, this times elemWidth is close to 1 bigraphItem
            int x = -barWidth/2 + SvgTool.Width/2 + xoffset;
            int yrect = (int)(3 * SvgTool.ElementHeight); //bigraph bar y-pos
            int y = yrect + (int)(0.5 * SvgTool.ElementHeight);
            var sb = new StringBuilder();
            sb.Append(string.Format("<g id='bigraphContainer'>" + NL));
            sb.Append(string.Format("<rect id='bigraphRect' onclick='bgRect(evt);' class='Bigraph' x='{0}' y='{1}'  width='{2}' height='{3}' rx='{4}' ry='{5}'/>", x - 10, yrect, barWidth, SvgTool.ElementHeight, SvgTool.ElementRounded, SvgTool.ElementRounded));
            sb.Append(string.Format("<text x='{0}' y='{1}' class='FontText' onmouseover='HItem(evt);' onmouseout='UItem(evt);'>" + NL, x, y));

            
            if (bigraphItemCount >= maxBigraphItems)
            {
                const string str = "!BigraphData stopped max items reached = ";
                sb.Append(string.Format("<tspan x='{0}' dy='{1}'>{2}</tspan>{3}", x, 0,
                                        str + maxBigraphItems, NL));
                x += SvgBuilder.FontCharWidth * str.Length + SvgBuilder.FontCharWidth;
            }            
            foreach (string str in bigraphData)
            {
                // calc pos
                // put in lookupPos
                // put in sb                
                string s = string.Format("{0} ({1})", str, lookup[str]);
                var key = str.GetHashCode();
                sb.Append(string.Format("<tspan x='{0}' dy='{1}' id='bg{2}' onclick='bgText(evt)'>{3}</tspan>" + NL, x, 0, key, s));
                var p = new Point { X = x, Y = y };
                lookupPos.Add(key, p);
                x += FontCharWidth * s.Length + FontCharWidth;
            }

            sb.Append(string.Format("</text>" + NL));

            // todo bundle lines to group matching bg text
            var lineGroups = new Dictionary<int,StringBuilder>();
            sb.Append(string.Format("<g id='bigraphLines'>" + NL));


            foreach (var key in lookupPos.Keys)
            {
                if (!lineGroups.ContainsKey(key))
                    lineGroups.Add(key, new StringBuilder());
            }

            foreach (Element e in data.Nodes)
            {
                if (data.UConfig.BGIncludeNode && lookupPos.ContainsKey(e.Name.GetHashCode() ))
                {
                    var key = e.Name.GetHashCode();                    
                    var p = lookupPos[key];
                    var s = string.Format("<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' class='LineNode'/>" + NL, p.X, p.Y,
                                          e.Svg.X, e.Svg.Y);                    
                    lineGroups[key].Append(s);
                }
                foreach (Entities.Attribute a in e.Attributes)
                {
                    var key = a.Name.GetHashCode();
                    if (data.UConfig.BGIncludeAttributeName && lookupPos.ContainsKey(key))
                    {                        
                        var p = lookupPos[key];
                        var s = string.Format("<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' class='LineAttr'/>" + NL, p.X,
                                              p.Y, e.Svg.X, e.Svg.Y);
                        lineGroups[key].Append(s);
                    }

                    key = a.Value.GetHashCode();
                    if (data.UConfig.BGIncludeAttributeValue && lookupPos.ContainsKey(key))
                    {                        
                        var p = lookupPos[key];
                        var s = string.Format("<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' class='LineAttr'/>" + NL, p.X,
                                              p.Y, e.Svg.X, e.Svg.Y);
                        lineGroups[key].Append(s);
                    }
                }
            }
            foreach (Element e in data.Texts)
            {
                if (string.IsNullOrEmpty(e.Value)) continue;
                string str = e.ValueShort;
                var key = str.GetHashCode();
                if (data.UConfig.BGIncludeText && lookupPos.ContainsKey(key))
                {
                    var p = lookupPos[key];
                    var s = string.Format("<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' class='LineText'/>" + NL, p.X, p.Y,
                                          e.Svg.X, e.Svg.Y);
                    lineGroups[key].Append(s);
                }
            }

            foreach (var key in lineGroups.Keys)
            {
                var s = lineGroups[key].ToString();
                sb.Append(string.Format("<g id='subbg{0}' style=''>{1}{2}",key,NL,s));
                sb.Append(string.Format("</g>" + NL)); 
            }

            sb.Append(string.Format("   </g>" + NL)); // end bigraphLines
            sb.Append(string.Format("</g>" + NL + NL)); // end bigraph

            return sb.ToString();
        }
    }
}
