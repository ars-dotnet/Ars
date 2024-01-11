using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ars.Common.Tool.Tools
{
    public class XmlNamespaceIgnoreReader : XmlTextReader
    {
        public XmlNamespaceIgnoreReader(TextReader textReader) :base(textReader)
        {
            
        }

        public override string NamespaceURI => "";
    }
}
