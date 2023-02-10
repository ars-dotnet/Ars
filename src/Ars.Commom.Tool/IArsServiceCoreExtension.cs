using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyArsenal.Commom.Tool
{
    public interface IArsServiceCoreExtension
    {
        void AddService(IServiceCollection services);

        //void UseApp(IApplicationBuilder builder);
    }
}
