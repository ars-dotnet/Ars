
using Ars.Commom.Tool.Extension;
using Ars.Common.Tool.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Ars.Common.Tool.Extension
{
    public static class XmlExtensions
    {
        public static string XMLSerialize<T>(this T entity)
        {
            StringBuilder buffer = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new Utf8StringWriter(buffer))
            {
                serializer.Serialize(writer, entity);
            }

            return buffer.ToString();
        }

        public static T? DeXmlSerialize<T>(this string xmlString, XmlRootAttribute? xmlRootAttribute = null)
            where T : class
        {
            T? cloneObject;

            StringBuilder buffer = new StringBuilder();
            buffer.Append(xmlString);

            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRootAttribute);

            using (TextReader reader = new StringReader(buffer.ToString()))
            {
                object? obj = serializer.Deserialize(new XmlNamespaceIgnoreReader(reader));
                cloneObject = obj.As<T>();
            }

            return cloneObject;
        }

        public static T? DeXmlPathSerialize<T>(this string xmlPath, XmlRootAttribute? xmlRootAttribute = null)
            where T : class
        {
            T? cloneObject;

            if (!File.Exists(xmlPath))
                return default;

            StringBuilder buffer = new StringBuilder();
            buffer.Append(xmlPath);

            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRootAttribute);
            using (TextReader reader = new StreamReader(buffer.ToString()))
            {
                object? obj = serializer.Deserialize(reader);
                cloneObject = obj.As<T>();
            }

            return cloneObject;
        }

        public static XmlDocument ParseXmlConfigFile(this string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit
                };

                var xmlDocument = new XmlDocument();
                using (var reader = XmlReader.Create(stream, settings))
                {
                    xmlDocument.Load(reader);
                }

                return xmlDocument;
            }
        }
    }
}
