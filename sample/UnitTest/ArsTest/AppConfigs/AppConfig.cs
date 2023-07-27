using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ArsTest.AppConfigs
{
    public class AppConfigs 
    {
        public IEnumerable<AppConfig> AppConfig { get; set; }
    }

    public class AppConfig
    {
        public string StationCode { get; set; }

        public IEnumerable<BerthConfig> BerthConfigs { get; set; }
    }

    [XmlRoot("configuration")]
    public class Configuration 
    {
        [XmlArray("BerthConfigs")]
        [XmlArrayItem()]
        public List<BerthConfig> BerthConfig { get; set; }
    }

    public class BerthConfig
    {
        [XmlElement]
        public string BerthCode { get; set; }

        [XmlArray("ButtonConfigs")]
        [XmlArrayItem]
        public List<ButtonConfig> ButtonConfig { get; set; }
    }

    public class ButtonConfig 
    {
        [XmlElement()]
        public int Type { get; set; }

        [XmlElement()]
        public string PlcAddr { get; set; }
    }
}
