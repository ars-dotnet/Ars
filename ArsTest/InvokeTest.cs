using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class InvokeTest
    {

        interface IABase { }
        interface IA : IABase{ }

        class A : IA { }

        [Fact]
        public void TestMulss() 
        {
            void get(string a,string b) => Console.WriteLine(a,b);

            string m = "11";
            string n = "12";
            Action<string, string> aa = (a,b) => get(a,b);
            aa(m, n);

            IServiceCollection services = new ServiceCollection();

            services.AddTransient<IA, A>();
            services.AddSingleton<IA, A>(); 
            services.AddTransient(typeof(Sky<>),typeof(SkyChild<>));

            var provider = services.BuildServiceProvider();
            var a = provider.GetService<IA>();
            var b = provider.GetService<IA>();

            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void TestInject()
        {
            Assert.True(typeof(IABase).IsAssignableFrom(typeof(A)));
        }

        [Fact]
        public void TestGeneric()
        {
            var x = typeof(Sky<>);//invoke失败
            var y = typeof(Sky<>).MakeGenericType(typeof(Animal));
            var method = y.GetMethod(nameof(Sky<Animal>.skyFluence)!)!.MakeGenericMethod(typeof(Animal));
            var xx = method!.Invoke(new Sky<Animal>(null), new object[] { "xxxx" });
        }

        [Fact]
        public async Task TestGenericAsync()
        {
            var x = typeof(Sky<>);//invoke失败
            var y = typeof(Sky<>).MakeGenericType(typeof(Animal));
            var method = y.GetMethod(nameof(Sky<Animal>.skyFluenceAsync)!, new Type[] { typeof(string) })!.MakeGenericMethod(typeof(Animal));
            Task task = (Task)method!.Invoke(new Sky<Animal>(null), new object[] { "xxxx" })!;
            await task!;

            var resultProperty = task.GetType().GetProperty("Result");
            var xx = resultProperty?.GetValue(task);
            return;
        }

        [Fact]
        public async Task TestGenericParamsAsync()
        {
            try
            {
                var x = typeof(Sky<>);//invoke失败
                var y = typeof(Sky<>).MakeGenericType(typeof(Animal));
                var method = y.GetMethod(nameof(Sky<Animal>.skyFluenceAsync)!, new Type[] { typeof(int), typeof(string[]) })!.MakeGenericMethod(typeof(Animal));
                Task task = (Task)method!.Invoke(new Sky<Animal>(null), new object[] { 123, new string[] { "123", "456" } })!;
                await task!;

                var resultProperty = task.GetType().GetProperty("Result");
                var xx = resultProperty?.GetValue(task);
                return;
            }
            catch (Exception ex)
            {

            }

        }

        [Fact]
        public void TestScope()
        {
            ScopeContriner scopeContriner = ScopeContriner.Empty;
            scopeContriner.list.TryAdd(typeof(Sky<Animal>).Name, new Sky<Animal>(null));
            using (var a = GetScope<Sky<Animal>>(scopeContriner))
            {
                string res = a._obj.skyFluence<int>("123");
            }
            Console.Read();
        }

        [Fact]
        public void TestServiceCollection()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<Sky<Animal>>();

            var provider = services.BuildServiceProvider();
            int xx = 0;
            int x = 0;

            int y = 0;
            int yy = 0;
            using (var scope = provider.CreateScope())
            {
                var b = scope.ServiceProvider.GetService<Sky<Animal>>();
                xx = b.GetHashCode();

                var c = scope.ServiceProvider.GetService<Sky<Animal>>();
                x = c.GetHashCode();
            }

            using (var scope = provider.CreateScope())
            {
                var b = scope.ServiceProvider.GetService<Sky<Animal>>();
                y = b.GetHashCode();

                var c = scope.ServiceProvider.GetService<Sky<Animal>>();
                yy = c.GetHashCode();
            }

            var a = provider.GetService<Sky<Animal>>();
            var xxx = a.GetHashCode();
            var aa = provider.GetService<Sky<Animal>>();
            var xxxx = aa.GetHashCode();
        }

        [Fact]
        public void TestRegisteGenericType()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<Animal>();
            serviceCollection.AddTransient(typeof(Sky<>));

            var a = serviceCollection.BuildServiceProvider().GetService(typeof(Sky<Animal>))! as Sky<Animal>;
            var mm = a.skyFluence<string>("123");
        }

        private ScopeResolve<T> GetScope<T>(ScopeContriner contriner) 
        {
            ScopeResolve<T> scope = new ScopeResolve<T>(contriner, (T)contriner.list[typeof(T).Name]);
            return scope;
        }

        class ScopeResolve<T> : IDisposable
        {
            private ScopeContriner _scopeContriner;
            public T _obj;
            public ScopeResolve(ScopeContriner scopeContriner,T obj)
            {
                _scopeContriner = scopeContriner;
                _obj = obj;
            }

            public void Dispose()
            {
                //删除obj的引用
                _scopeContriner.list.Remove(_obj!.GetType().Name);
            }
        }

        interface ICovariant<in R> { }

        // Extending covariant interface.
        interface IExtCovariant<R> : ICovariant<R> { }

        // Implementing covariant interface.
        class Sample<R> : IExtCovariant<R> { }

        class Program
        {
            static void Test()
            {
                ICovariant<Object> iobj = new Sample<Object>();
                ICovariant<String> istr = new Sample<String>();

                // You can assign istr to iobj because
                // the ICovariant interface is covariant.
                //iobj = istr;

                istr = iobj;
            }
        }

        class ScopeContriner : IDisposable
        {
            public IDictionary<string,object> list;
            protected ScopeContriner()
            {
                list = new Dictionary<string,object>();
            }

            public static ScopeContriner Empty => new ScopeContriner();

            public void Dispose()
            {
            }
        }

        class Animal 
        {
            public Animal()
            {

            }

        }

        class SkyChild<A> : Sky<A> where A : Animal
        {
            public SkyChild(A t) : base(t)
            {

            }
        }

        class Sky<T> where T : Animal
        {
            private T _t;
            public Sky(T t)
            {
                _t = t;
            }

            public void Dispose()
            {

            }

            public string skyFluence<Tp>(string a)
            {
                return string.Concat(typeof(Tp).Name,"_",a);
            }

            public Task<string> skyFluenceAsync<Tp>(string a)
            {
                return Task.Run(() =>
                {
                    var aa = string.Concat(typeof(Tp).Name, "_", a);
                    return aa;
                });
            }

            public Task<string> skyFluenceAsync<Tp>(int b, params string[] a)
            {
                return Task.Run(() =>
                {
                    var aa = string.Concat(typeof(Tp).Name, "_", string.Join(".",a));
                    return aa;
                });
            }
        }
    }
}
