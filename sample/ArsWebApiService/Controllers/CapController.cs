using Ars.Common.Cap;
using Ars.Common.Core.IDependency;
using ArsWebApiService.Controllers.BaseControllers;
using Asp.Versioning;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ArsWebApiService.Controllers
{
    /// <summary>
    /// cap test controller
    /// </summary>
    [ApiVersion("1.0")]
    public class CapController : ArsWebApiBaseController
    {
        /// <summary>
        /// cap发布测试
        /// </summary>
        /// <param name="arsCapPublisher"></param>
        /// <returns></returns>
        [HttpPost]
        //多个版本调用
        [ApiVersionNeutral]
        public async Task PuhlishAsync([FromServices] IArsCapPublisher arsCapPublisher) 
        {
            await arsCapPublisher.PublishAsync("ars.cap.publish",new { name = "ars",age = 30 });
        }

        /// <summary>
        /// cap发布测试V2.0
        /// </summary>
        /// <param name="arsCapPublisher"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiVersion("2.0")]
        public async Task PuhlishAsyncv2([FromServices] IArsCapPublisher arsCapPublisher)
        {
            await arsCapPublisher.PublishAsync("ars.cap.publish", new { name = "ars", age = 30 });
        }

        /// <summary>
        /// cap消费测试
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe("ars.cap.publish", Group = "g1")]
        public Task SubscribeAsync(object msg) 
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// cap消费测试
        /// 同topic，不同的group才能分开消费
        /// 同topic，不同的group不会互相影响
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe("ars.cap.publish", Group = "g2")] 
        public async Task Subscribe1Async(object msg)
        {
            //线性消费
            await Task.Delay(1000 * 10);

            return;
        }

        /// <summary>
        /// cap发布测试数据库事务
        /// </summary>
        /// <param name="lastName">名字</param>
        /// <param name="arsCapPublisher"></param>
        /// <returns></returns>
        [HttpPost]
        public Task PuhlishUowAsync([FromBody]string lastName, [FromServices] IArsCapPublisher arsCapPublisher) 
        {
            return arsCapPublisher.PublishDelayAsync(TimeSpan.FromSeconds(10),"ars.cap.uowtest", lastName);
        }
    }
}
