using Ars.Commom.Tool.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ars.Common.Tool.Export
{
    internal class XmlFileManager : IXmlFileManager
    {
        private readonly IDictionary<string, Hashtable> _dis;
        private readonly SemaphoreSlim semaphoreSlim;
        public XmlFileManager()
        {
            _dis = new Dictionary<string, Hashtable>(0);
            semaphoreSlim = new SemaphoreSlim(1,1);
        }

        public virtual string GetPropertyXmlSummary(Type classType, string propertyName)
        {
            var path = classType.Assembly.Location.Replace("dll", "xml");
            string hashkey = string.Concat(classType.FullName, ".", propertyName);
            if (_dis.TryGetValue(path, out Hashtable? table) && 
                (table?.ContainsKey(hashkey) ?? false)) 
            {
                return table[hashkey]!.ToString()!;
            }

            if (!File.Exists(path))
                return propertyName;

            semaphoreSlim.Lock(() => SetXmlResource(path));

            if (_dis.TryGetValue(path, out Hashtable? t) &&
                (t?.ContainsKey(hashkey) ?? false))
            {
                return t[hashkey]!.ToString()!;
            }

            return propertyName;
        }

        public virtual void SetXmlResource(string xmlPath) 
        {
            if (_dis.ContainsKey(xmlPath))
                return;

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(xmlPath);
            //获取节点列表 
            XmlNodeList topM = xmldoc.SelectNodes("//member")!;
            Hashtable table = new Hashtable();

            XmlNode? xmlNode;
            string key = string.Empty;
            string value = string.Empty;
            foreach (XmlNode element in topM)
            {
                xmlNode = element.SelectSingleNode("summary");
                if (null == xmlNode)
                    continue;

                value = element.SelectSingleNode("summary")!.InnerText;

                if (string.IsNullOrEmpty(value))
                    continue;

                value = value.Trim();
                key = element.Attributes!["name"]!.Value.Split(':')[1];

                table.Add(key, value);
            }

            _dis.TryAdd(xmlPath, table);
        }
    }
}
