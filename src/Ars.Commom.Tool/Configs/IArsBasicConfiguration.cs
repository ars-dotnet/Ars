using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Configs
{
    public interface IArsBasicConfiguration
    {
        /// <summary>
        /// application根目录
        /// </summary>
        string Root { get; }

        /// <summary>
        /// application url
        /// </summary>
        public string ApplicationUrl { get; set; }
    }
}
