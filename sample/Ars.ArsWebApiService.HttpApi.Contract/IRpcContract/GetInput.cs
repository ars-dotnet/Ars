using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.ArsWebApiService.HttpApi.Contract.IRpcContract
{
    public class GetInput
    {
        public string Name { get; set; }

        public string Top { get; set; }
    }
}
