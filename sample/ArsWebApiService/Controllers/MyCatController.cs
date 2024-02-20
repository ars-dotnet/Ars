using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Ars.Common.Core.Uow.Attributes;
using Ars.Common.EFCore.Repository;
using ArsWebApiService.Model.MyCatModel;
using Aspose.Cells;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SkyWalking.NetworkProtocol.V3;
using System;
using Task = ArsWebApiService.Model.MyCatModel.Task;
using Ars.Common.EFCore.Extension;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using System.Text.Unicode;
using System.Text;
using ArsWebApiService.Controllers.BaseControllers;

namespace ArsWebApiService.Controllers
{
    /// <summary>
    /// mycat测试
    /// </summary>
    public class MyCatController : ArsWebApiBaseController
    {
        [Autowired]
        public IRepository<User> UserRepo { get; set; }

        [Autowired]
        public IRepository<Product> ProductRepo { get; set; }

        [Autowired]
        public IRepository<Task> TaskRepo { get; set; }

        [Autowired]
        public IRepository<TaskDetail> TaskDetailRepo { get; set; }

        [Autowired]
        public IRepository<Order> OrderRepo { get; set; }

        [Autowired]
        public IRepository<OrderDetail> OrderDetailRepo { get; set; }

        [Autowired]
        public IRepository<OrderQuery> OrderQueryRepo { get; set; }

        [Autowired]
        public IRepository<OrderDetailQuery> OrderDetailQueryRepo { get; set; }

        [Autowired]
        public MyCatDbContext MyCatDbContext { get; set; }

        [Autowired]
        public MyCatQueryDbContext MyCatQueryDbContext { get; set; }

        [Autowired]
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        [Autowired]
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        /// <summary>
        /// mycat添加User
        /// mycat似乎不支持ercore自定义事务
        /// mycat只支持一次插入一张表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddUser() 
        {
            //根据section_code进行一致性hash规则分片
            await MyCatDbContext.t_user.AddAsync(new User
            {
                Id = 33,
                Name = "Tom",
                Age = 26,
                CreateTime = DateTime.Now,
                SectionCode = "Brs"
            });
            await MyCatDbContext.t_user.AddAsync(new User
            {
                Id = 34,
                Name = "Jerry",
                Age = 27,
                CreateTime = DateTime.Now,
                SectionCode = "Crs"
            });

            await MyCatDbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// mycat添加Product
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddProduct() 
        {
            //全局表
            await MyCatDbContext.t_product.AddAsync(new Product
            {
                Id = 4,
                ProductName = "梨子",
                ProductCode = "004",
                CreateUser = 30
            });
            await MyCatDbContext.t_product.AddAsync(new Product
            {
                Id = 5,
                ProductName = "石榴",
                ProductCode = "005",
                CreateUser = 30
            });

            await MyCatDbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// mycat添加Task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddTask()
        {
            //根据id进行十进制求模运算
            await MyCatDbContext.t_task.AddAsync(new Task
            {
                Id = 26,
                TaskCode = "T2O",
                OrgCode = "O002",
                CreateUser = 30,
            });
            await MyCatDbContext.t_task.AddAsync(new Task
            {
                Id = 27,
                TaskCode = "T21",
                OrgCode = "O002",
                CreateUser = 30,
            });
            await MyCatDbContext.t_task.AddAsync(new Task
            {
                Id = 28,
                TaskCode = "T22",
                OrgCode = "O002",
                CreateUser = 30,
            });

            //子表不支持一次插入多条记录
            //和task表erjoin
            using var dbcontext = ServiceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MyCatDbContext>();
            await dbcontext.t_task_detail.AddAsync(new TaskDetail
            {
                Id = 30,
                TaskId = 26,
                PlateNo = "1001"
            });
            await dbcontext.SaveChangesAsync();

            using var dbcontext1 = ServiceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MyCatDbContext>();
            await dbcontext1.t_task_detail.AddAsync(new TaskDetail
            {
                Id = 31,
                TaskId = 26,
                PlateNo = "1002"
            });
            await dbcontext1.SaveChangesAsync();


            using var dbcontext2 = ServiceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MyCatDbContext>();
            await dbcontext2.t_task_detail.AddAsync(new TaskDetail
            {
                Id = 32,
                TaskId = 27,
                PlateNo = "1003"
            });
            await dbcontext2.SaveChangesAsync();

            using var dbcontext3 = ServiceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MyCatDbContext>();
            await dbcontext3.t_task_detail.AddAsync(new TaskDetail
            {
                Id = 33,
                TaskId = 28,
                PlateNo = "1004"
            });
            await dbcontext3.SaveChangesAsync();

            await MyCatDbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// mycat添加Order
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddOrder()
        {
            //create_time按月分表
            //这里分表的字段添加时用string类型
            //因为mycat会默认将datetime转换位timestamp
            //INSERT INTO `t_order_detail` (`id`, `create_time`, `order_id`, `pay_amount`, `product_id`)
            // VALUES(33, timestamp('2024-01-29 11:39:51.348865'), 31, 100.78, 4)
            //但是insert却要报错 (columnValue:timestamp('2024-01-26 14:49:33.000000') Please check if the format satisfied.)
            await MyCatDbContext.t_order.AddAsync(new Order
            {
                Id = 12,
                OrderCode = "O011",
                CreateTime = "2023-06-10 10:06:35",
                CreateUser = 30,
            });
            await MyCatDbContext.t_order.AddAsync(new Order
            {
                Id = 13,
                OrderCode = "O012",
                CreateTime = "2023-10-10 10:06:35",
                CreateUser = 30,
            });
            await MyCatDbContext.t_order.AddAsync(new Order
            {
                Id = 14,
                OrderCode = "O013",
                CreateTime = "2024-10-10 10:06:35",
                CreateUser = 30,
            });
            await MyCatDbContext.t_order.AddAsync(new Order
            {
                Id = 15,
                OrderCode = "O014",
                CreateTime = "2024-09-10 10:06:35",
                CreateUser = 30,
            });
            await MyCatDbContext.t_order.AddAsync(new Order
            {
                Id = 16,
                OrderCode = "O015",
                CreateTime = "2024-08-10 10:06:35",
                CreateUser = 30,
            });
            await MyCatDbContext.t_order.AddAsync(new Order
            {
                Id = 17,
                OrderCode = "O016",
                CreateTime = "2024-08-15 10:06:35",
                CreateUser = 30,
            });

            await MyCatDbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// mycat添加OrderDetail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddOrderDetail() 
        {
            //create_time按月分表
            await MyCatDbContext.t_order_detail.AddAsync(new OrderDetail
            {
                Id = 13,
                OrderId = 11,
                ProductId = 4,
                PayAmount = 100.23m,
                CreateTime = "2023-04-10 11:11:11"
            });
            await MyCatDbContext.t_order_detail.AddAsync(new OrderDetail
            {
                Id = 14,
                OrderId = 11,
                ProductId = 4,
                PayAmount = 55.89m,
                CreateTime = "2023-07-10 11:11:11",
            });
            await MyCatDbContext.t_order_detail.AddAsync(new OrderDetail
            {
                Id = 15,
                OrderId = 16,
                ProductId = 4,
                PayAmount = 100.78m,
                CreateTime = "2024-09-15 11:11:11",
            });

            await MyCatDbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// 要报错
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> AddOrderDetailTest() 
        {
            await MyCatQueryDbContext.t_order.AddAsync(new OrderQuery
            {
                Id = 31,
                OrderCode = "O016",
                CreateTime = DateTime.Now,
                CreateUser = 30,
            });

            await MyCatQueryDbContext.t_order_detail.AddAsync(new OrderDetailQuery
            {
                Id = 34,
                OrderId = 31,
                ProductId = 4,
                PayAmount = 100.78m,
                CreateTime = DateTime.Now,
            });

            await MyCatQueryDbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> QueryCountAsync() 
        {
            var user = await UserRepo.CountAsync();

            var product = await ProductRepo.CountAsync();

            var task = await TaskRepo.CountAsync();

            var taskDetail = await TaskDetailRepo.CountAsync();

            var order = await OrderRepo.CountAsync();

            var orderDetail = await OrderDetailRepo.CountAsync();

            return Ok((user, product, task, taskDetail, order, orderDetail));
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> QueryDataAsync() 
        {
            var data1 = await (from u in UserRepo.GetAll()
                              join p in ProductRepo.GetAll()
                              on u.Id equals p.CreateUser
                              select new { u, p })
                             .ToListAsync();

            object? data2 = null;

            //var data2 = await (from p in ProductRepo.GetAll()
            //                   join o in OrderQueryRepo.GetAll()
            //                   on p.CreateUser equals o.CreateUser
            //                   orderby o.CreateTime
            //                   select new { p, o })
            //                  .ToListAsync();

            var data3 = await (from o in OrderQueryRepo.GetAll()
                               join od in OrderDetailQueryRepo.GetAll()
                               on o.Id equals od.OrderId
                               orderby o.CreateTime
                               select new { o,od })
                              .ToListAsync();

            var data4 = await (from o in TaskRepo.GetAll()
                               join od in TaskDetailRepo.GetAll()
                               on o.Id equals od.TaskId
                               orderby o.Id
                               select new { o, od })
                              .ToListAsync();

            return Ok((data1, data2, data3,data4));
        }
    }
}
