using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Topro.CombRetLine.HttpApi.Contract.MesXmlModel
{
    [XmlRoot("Barcode")]
    public class CallMesXmlInput
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("macCode")]
        public string MacCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("wipEntity")]
        public string WipEntity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("item")]
        public List<Item> Item { get; set; }
    }

    public class Item
    {
        [XmlAttribute("tagCode")]
        public string TagCode { get; set; }

        [XmlAttribute("tagValue")]
        public string TagValue { get; set; }

        [XmlAttribute("timeStamp")]
        public string TimeStamp { get; set; }
    }
}
