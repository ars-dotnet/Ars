﻿using Ars.Common.Consul.GrpcHelper;
using Grpc.Core;
using Grpc.Health.V1;

namespace GrpcService.Services
{
    public class HealthCheckService : Health.HealthBase
    {
        public override Task<HealthCheckResponse> Check(
            HealthCheckRequest request, 
            ServerCallContext context)
        {
            return Task.FromResult(
                 new HealthCheckResponse()
                 {
                     Status = HealthCheckResponse.Types.ServingStatus.Serving
                 });
        }

        public override async Task Watch(
            HealthCheckRequest request, 
            IServerStreamWriter<HealthCheckResponse> responseStream, 
            ServerCallContext context)
        {
            await responseStream.WriteAsync(
                new HealthCheckResponse()
                {
                    Status = HealthCheckResponse.Types.ServingStatus.Serving
                });
        }
    }
}
