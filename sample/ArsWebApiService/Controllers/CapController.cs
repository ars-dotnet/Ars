using Ars.Common.Cap;
using Ars.Common.Core.IDependency;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ArsWebApiService.Controllers
{
    /// <summary>
    /// cap test controller
    /// </summary>
    [ApiController]
    [Route("Api/[controller]/[action]")]
    public class CapController : ControllerBase
    {
        /// <summary>
        /// cap发布测试
        /// </summary>
        /// <param name="arsCapPublisher"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task PuhlishAsync([FromServices] IArsCapPublisher arsCapPublisher) 
        {
            await arsCapPublisher.PublishAsync("ars.cap.publish",new { name = "ars",age = 30 });

            await arsCapPublisher.PublishAsync("ars.cap.publish",123456);
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
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe("ars.cap.publish", Group = "g2")] //同topic，不同的group才能分开消费
        public async Task Subscribe1Async(object msg)
        {
            //线性消费
            //await Task.Delay(1000 * 60);

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
