using ExpressionDemo.MyDbFunctionsExtensions;
using ExpressionDemo.TestType;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressionDemo
{


    public interface IAnimal
    {
        public int Age { get; set; }
    }

    public class Animal : IAnimal
    {
        public int Age { get; set; }
    }

    class Program
    {
        public static void Test(IList<IAnimal> list)
        {
            var a = list.First() as Animal;
        }

        static async Task Main(string[] args)
        {
            try
            {
                decimal cc = 12.67m;

                List<Animal> animals = new List<Animal>() { new Animal { Age = 1 } };

                var aa = animals.First() as IAnimal;
                var a = animals.Select(r => (IAnimal)r).ToList();
                Test(a);

                Activetor activetor = new Activetor();
                activetor.Test();

                //var provider = BuildScopeServices();
                //using var db = provider.GetRequiredService<MyDbContext>();
                //var m = await db.Users.Where(r => EF.Functions.ThisLike(r.AccountNumber, "%admin%")).ToListAsync();

                //Console.WriteLine(m);

                //List<string> a = new List<string>();
                //a.Where(r => EF.Functions.Like(r, "12"));

                //GC.KeepAlive(this);

                //TestStatic testStatic = new TestStatic();
                //testStatic = new TestStatic();

                //ITestComponent testComponentA = new TestA();
                //ITestComponent testComponent = new TestB(testComponentA);
                //testComponent.Test();

                //TestAbstract testAbstract = new TestAbstractA(1);
                //testAbstract.Get();

                //TestM();
                //var obj = new DelegateTest();
                //obj.Add(Testmm);
                //var a = obj.Action();
                //Console.WriteLine(a);

                //Expression<Func<int, int, int>> expression22 = (x, y) => x + y;
                //Expression<Func<string, bool>> expression1 = t => t.Contains("xxxx");

                ////参数表达式
                //ParameterExpression p1 = Expression.Parameter(typeof(int), "i");
                ////常量表达式
                //ConstantExpression c1 = Expression.Constant(5, typeof(int));
                ////二元运算符表达式（需要两个参数）[++ += 就属于一元运算符]
                //BinaryExpression b1 = Expression.Assign(p1, c1);

                //ParameterExpression p = Expression.Parameter(typeof(int), "i");
                //ConstantExpression c = Expression.Constant(10, typeof(int));
                //BinaryExpression b = Expression.Add(p, c);
                //var m = Expression.Lambda<Func<int, int>>(b, p);
                //int k = m.Compile()(90);

                //Expression.Lambda<Action>(Expression.Call(typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string)}),estring));

                //Console.WriteLine(k);


                //var a1 = Expression.Parameter(typeof(int),"a");
                //var a2 = Expression.Parameter(typeof(int), "b");
                //var a3 = Expression.Parameter(typeof(int), "c");
                //var a4 = Expression.Add(a1, a2);
                //var e1 = Expression.Multiply(a4,a3);
                //var xx = e1.Modify();
                //Console.WriteLine(e1.ToString());
                //Console.WriteLine(xx.ToString());

                //TestObj();

                //CreateObj();


                //using (TestGc testGc = new TestGc())
                //{

                //};
                //CreateGC();

                //List<Task> tasks = new List<Task>(10);
                //TestGc testGc = new TestGc();
                //for (int i = 0; i < 1; i++)
                //{
                //    tasks.Add(testGc.TestAdd());
                //}

                //Console.WriteLine($"1：{Thread.CurrentThread.ManagedThreadId}");
                //Task.WaitAll(tasks.ToArray());
                //Console.WriteLine($"2：{Thread.CurrentThread.ManagedThreadId}");
                //IQueryable<string> query = new TestQuerable<string>();
                //var result = from q in query select q;
                //foreach (var i in result) 
                //{
                //    Console.WriteLine(i);
                //}

                Console.Read();
            }
            catch (Exception e)
            {

            }

            return;
        }

        static IServiceProvider BuildScopeServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<MyDbContext>();
            //services.AddScoped<IMethodCallTranslator, DmDbFunctionsTranslateImpl>();
            //services.AddScoped<IMethodCallTranslatorProvider, DmAlgorithmsMethodCallTranslatorPlugin>();
            //services.AddScoped<IDbContextOptionsExtension, DmDbContextOptionsExtension>();

            return services.BuildServiceProvider().CreateScope().ServiceProvider;
        }

        #region test
        private class Demo
        {
            public int id { get; set; }
        }

        static void PropertyDempo()
        {
            var d = Expression.Parameter(typeof(Demo), "demo");
            var p = Expression.Property(d, "id");
            var demo = Expression.Lambda<Func<Demo>>(Expression.Assign(p, Expression.Constant(3, typeof(int)))).Compile()();
            var c = Expression.Equal(p, Expression.Constant(1, typeof(int)));
            var m = Expression.Lambda<Func<Demo, bool>>(c, d).Compile()(demo);
            Expression.Lambda<Action>(
                Expression.Call(
                    typeof(Console).GetMethod("WriteLine", new[] { typeof(bool) }),
                    Expression.Constant(m, typeof(bool))))
                .Compile()();
            Expression.Lambda<Action>(Expression.Call(typeof(Console).GetMethod("Read"))).Compile()();
        }

        private static int Testmm()
        {
            return 3;
        }

        static void TestM()
        {
            var list = new List<int>() { 1, 2, 3, 4 };
            var foo = new Foo<int, string>(list, "ryzen");

            var index = 0;
            Console.WriteLine($"索引：索引{index}的值:{foo[index]}");

            Console.WriteLine($"Filed:{foo.Field}");

            foo.Method(2333);

            foo.Method<DateTime, long>(DateTime.Now, 2021);

            foo.DelegateMenthod<string>("this is a delegate", DelegateMenthod);

            foo.InterfaceMenthod(new StringBuilder().Append("InterfaceMenthod:this is a interfaceMthod"));

            Console.WriteLine(foo + "重载+运算符");
        }

        static void DelegateMenthod(string str)
        {
            Console.WriteLine($"{nameof(DelegateMenthod)}:{str}");
        }





        static TestGc CreateObj()
        {
            TestGc testGc = new TestGc();
            return testGc;
        }

        static void CreateGC()
        {
            GC.Collect();
        }

        static int i = 0;




        static void TestObj()
        {
            Demo d = new Demo() { id = 1 };
            //d.id = 1;
            //var c = d;
            //d = null;
            TestObj1(d);
            Console.WriteLine(d.id);
            Console.Read();
        }

        static void TestObj1(Demo demo)
        {
            var c = demo;
            c.id = 2;
            demo = new Demo { id = 39 };
        }

        static void TestObjs()
        {
            List<Demo> demos = new List<Demo>
            {
                new Demo{ id = 1},
                new Demo{ id = 2},
            };

            var c = demos.Where(r => r.id > 1);
        }

        /// <summary>
        /// 条件表达式
        /// </summary>
        static void IfThenElse()
        {
            var ex = Expression.IfThenElse(
                Expression.Equal(Expression.Constant(1), Expression.Constant(2)),
                //Expression.Constant(false),
                Expression.Call(
                    typeof(Console).GetMethod("WriteLine", new Type[1] { typeof(string) }),
                    Expression.Constant("条件为true")),
                Expression.Call(
                    typeof(Console).GetMethod("WriteLine", new Type[1] { typeof(string) }),
                    Expression.Constant("条件为false")));

            Expression.Lambda<Action>(ex).Compile()();
            Expression.Lambda<Action>(Expression.Call(typeof(Console).GetMethod("Read"))).Compile()();
        }

        /// <summary>
        /// 赋值表达式
        /// </summary>
        static void BinaryExpression()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(int), "age");
            BlockExpression block = Expression.Block(
                new[] { parameter },
                Expression.Assign(parameter, Expression.Constant(10, typeof(int))),
                Expression.AddAssign(parameter, Expression.Constant(20, typeof(int))),
                Expression.SubtractAssign(parameter, Expression.Constant(15, typeof(int))),
                Expression.MultiplyAssign(parameter, Expression.Constant(2, typeof(int))),
                Expression.DivideAssign(parameter, Expression.Constant(10, typeof(int))));

            var m = Expression.Lambda<Func<int>>(block).Compile()();
            var c1 = Expression.Call(
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) }),
                Expression.Constant(m, typeof(int)));
            var c2 = Expression.Call(typeof(Console).GetMethod("Read"));
            Expression.Lambda<Action>(c1).Compile()();
            Expression.Lambda<Action>(c2).Compile()();
        }

        /// <summary>
        /// 循环表达式与块表达式
        /// 取1-100之内的偶数
        /// </summary>
        static void BlockandLoop()
        {
            var abc = Expression.Parameter(typeof(int), "abc");
            LabelTarget label = Expression.Label();
            var b = Expression.Block(
                new[] { abc },
                Expression.Assign(abc, Expression.Constant(1, typeof(int))),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThanOrEqual(abc, Expression.Constant(100, typeof(int))),
                        Expression.Block(
                            Expression.IfThen(
                                Expression.Equal(
                                    Expression.Modulo(abc, Expression.Constant(2, typeof(int))),
                                    Expression.Constant(0, typeof(int)))
                                , Expression.Call(typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) }), abc))
                            , Expression.AddAssign(abc, Expression.Constant(1, typeof(int)))),
                        Expression.Break(label)),
                    label));

            Expression.Lambda<Action>(b).Compile()();
            Expression.Lambda<Action>(Expression.Call(typeof(Console).GetMethod("Read"))).Compile()();
        }

        /// <summary>
        /// 
        /// </summary>
        static void TestBlockAndLoop()
        {
            var i = Expression.Parameter(typeof(int), "i");
            var label = Expression.Label();
            var b = Expression.Block(
                new[] { i },
                Expression.Assign(i, Expression.Constant(1, typeof(int))),
                Expression.Loop(
                    Expression.IfThenElse(
                         Expression.LessThanOrEqual(i, Expression.Constant(100, typeof(int)))
                          , Expression.Block(
                           Expression.IfThen(
                               Expression.Equal(Expression.Modulo(i, Expression.Constant(2, typeof(int))), Expression.Constant(0, typeof(int)))
                              , Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(int) }), i))
                           , Expression.PostIncrementAssign(i))
                        , Expression.Break(label)),
                    label));
            Expression.Lambda<Action>(b).Compile()();
            Expression.Lambda<Action>(Expression.Call(typeof(Console).GetMethod("Read"))).Compile()();
        }

        static void Test()
        {
            int a = 0;
            while (a <= 100)
            {
                for (int i = 2; i <= a; i++)
                {

                }
                Console.WriteLine(a);
                a++;
            }

            Console.Read();
        }
    }

    public class TestGc : IDisposable
    {
        private int m = 0;
        private int i = 0;

        public async Task TestAdd()
        {
            //Console.WriteLine($"first：{Thread.CurrentThread.ManagedThreadId},{m}");
            Console.WriteLine($"first：{Thread.CurrentThread.ManagedThreadId}");
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://www.baidu.com/");
            string html = await httpClient.GetStringAsync("/").ConfigureAwait(false);
            //var cc = Interlocked.Increment(ref m);
            //i++;
            Console.WriteLine($"second：{Thread.CurrentThread.ManagedThreadId}");
            //Console.WriteLine($"before：{Thread.CurrentThread.ManagedThreadId},{i},{m}");
            //i++;
            //Console.WriteLine($"after：{Thread.CurrentThread.ManagedThreadId},{i}");
            return;
        }

        public void Close()
        {
            Console.WriteLine("Close");
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~TestGc()
        {
            Console.WriteLine("~TestGc()");
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Console.WriteLine("Dispose()");

            //抑制析构函数调用
            GC.SuppressFinalize(this);
        }
        #endregion 
    }
}
