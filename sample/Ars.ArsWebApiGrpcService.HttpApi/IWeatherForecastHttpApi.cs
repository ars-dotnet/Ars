using Ars.ArsWebApiGrpcService.HttpApi.Contract;
using Ars.Common.RpcClientCore.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace Ars.ArsWebApiGrpcService.HttpApi
{
    [ServiceName("apigrpcwebapiservice")]
    public interface IWeatherForecastHttpApi
    {
        [HttpGet("Api/ArsGrpcWebApi/WeatherForecast/Get")]
        Task<IEnumerable<WeatherForecastOutput>> Get();
    }
}
