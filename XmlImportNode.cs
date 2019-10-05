
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace Common.Xml
{
    public class XmlImportNode
    {
        public static Dictionary<string, string> paramDic;
        public string name;
        public string key;
        public XmlNode xml;

        public XmlImportNode(XmlNode node)
        {
            this.name = node.Attributes["name"].Value;
            this.key = node.Attributes["key"].Value;
           


            var val = name;
            if (val.Contains("{"))
            {
                var startIndex = val.IndexOf("{");
                var endIndex = val.IndexOf("}");
                var sbs = val.Substring(startIndex + 1, endIndex - startIndex - 1);
                string replace;
                if (paramDic != null && paramDic.TryGetValue(sbs, out replace))
                {
                    //r replace = folderMap[sbs];
                    val = val.Replace("{" + sbs + "}", replace);
                    this.name = val;
                }
                
            }
        }

        public static void AddImportParams(string key , string value)
        {
            if (paramDic == null)
            {
                paramDic = new Dictionary<string, string>();
            }

            paramDic[key] = value;
        }
        public static void InitImportParams()
        {
            if (paramDic == null)
            {
                paramDic = new Dictionary<string, string>();
                paramDic["lang"] = "tr";
            }
        
        }
    }
}
