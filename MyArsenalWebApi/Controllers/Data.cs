using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyArsenalWebApi.Controllers
{
    /// <summary>
    /// Data
    /// </summary>
    public class Data
    {
        /// <summary>
        /// 这是id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 这是age
        /// </summary>
        public int age { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        public IFormFile file { get; set; }
    }
}
