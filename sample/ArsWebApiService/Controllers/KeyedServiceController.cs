using Ars.Common.Core.IDependency;
using ArsWebApiService.Controllers.BaseControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApiWithIdentityServer4.Controllers;

namespace ArsWebApiService.Controllers
{
    public class KeyedServiceController : ArsWebApiBaseController
    {
        [Autowired("Bird")]
        public ITestDomain TestDomain { get; set; }

        [Autowired("Cat")]
        public ITestDomain TestDomain1 { get; set; }

        [Autowired("Dog")]
        public DogDomain DogDomain { get; set; }

        [Autowired]
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        [Autowired]
        public ITestService TestService { get; set; }

        /// <summary>
        /// 测试KeyedService特性，获取不同的实现
        /// 如果用了KeyedService，则Autowired一定要带上Key，否则会报错找不到服务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TestKeyedServiceAsync() 
        {
            return Ok((await TestDomain.Test(),await TestDomain1.Test(),await DogDomain.Test()));
        }

        /// <summary>
        /// 测试ServiceScopeFactory获取KeyedService
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TestKeyedService1Async()
        {
            var service = ServiceScopeFactory.CreateScope().ServiceProvider.GetRequiredKeyedService<ITestDomain>("Bird");

            return Ok(await service.Test());
        }
    }
}
