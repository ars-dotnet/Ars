using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyArsenal.Commom.Tool
{
    public interface IConfigExtension
    {
        void AddService(IServiceCollection services);
    }
}
