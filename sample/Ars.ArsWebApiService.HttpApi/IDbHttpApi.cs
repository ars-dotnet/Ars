﻿using Ars.ArsWebApiService.HttpApi.Contract.IRpcContract;
using Ars.Common.RpcClientCore.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace Ars.ArsWebApiService.HttpApi
{
    [ServiceName("arswebapiservice")]
    public interface IDbHttpApi
    {
        [HttpGet("Api/ArsWebApi/V1/DbContext/Query/Query")]
        Task<StudentOutput> Query();

        [Token]
        [HttpPost("Api/ArsWebApi/V1/DbContext/ModifyWithDefaultTransaction/ModifyWithDefaultTransaction")]
        Task ModifyWithDefaultTransaction();
    }
}
