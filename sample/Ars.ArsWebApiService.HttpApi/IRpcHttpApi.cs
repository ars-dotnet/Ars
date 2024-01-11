using Ars.ArsWebApiService.HttpApi.Contract.IRpcContract;
using Ars.Common.RpcClientCore.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace Ars.ArsWebApiService.HttpApi
{
    [ServiceName("arswebapiservice")]
    [Token]
    public interface IRpcHttpApi
    {
        [WebApiClientCore.Attributes.HttpPost("Api/ArsWebApi/V1/RpcHttpApi/PostFromForm")]
        Task<GetOutput> PostFromForm([FormContent] GetInput input);

        [WebApiClientCore.Attributes.HttpGet("Api/ArsWebApi/V1/RpcHttpApi/GetFromQuery")]
        Task<GetOutput> GetFromQuery([PathQuery] GetInput input);

        [WebApiClientCore.Attributes.HttpGet("Api/ArsWebApi/V1/RpcHttpApi/GetFromRoute/{where}/{when}")]
        Task<GetOutput> GetFromRoute([FromQuery] RouteInput route,[PathQuery] GetInput input);

        [WebApiClientCore.Attributes.HttpPost("Api/ArsWebApi/V1/RpcHttpApi/PostFromBody")]
        Task<GetOutput> PostFromBody([JsonContent] GetInput input);
    }
}
