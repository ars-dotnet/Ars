using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core
{
    public interface IHttpClientProvider : ISingletonDependency
    {
        /// <summary>
        /// 创建httpclient实例
        /// </summary>
        /// <param name="isHttps">是否是https请求</param>
        /// <returns></returns>
        HttpClient CreateClient(bool isHttps);

        /// <summary>
        /// 创建httpclient实例
        /// </summary>
        /// <param name="clientName"> The logical name of the client to create.</param>
        /// <returns></returns>
        HttpClient CreateClient(string clientName);
    }
}
