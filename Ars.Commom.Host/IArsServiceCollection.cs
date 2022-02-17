using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Host
{
    public interface IArsServiceCollection : IServiceCollection
    {
        IServiceCollection ServiceCollection { get; }

        IConfiguration Configuration { get; }

        IWebHostEnvironment WebHostEnvironment { get; }

        IServiceProvider Provider { get; }
    }

    public class ArsServiceCollection : IArsServiceCollection
    {
        public ArsServiceCollection(IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            ServiceCollection = services;
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IServiceCollection ServiceCollection { get; }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        public IServiceProvider Provider => ServiceCollection.BuildServiceProvider();

        public IEnumerator<ServiceDescriptor> GetEnumerator() => ServiceCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(ServiceDescriptor item) => ServiceCollection.Add(item);

        public void Clear() => ServiceCollection.Clear();

        public bool Contains(ServiceDescriptor item) => ServiceCollection.Contains(item);

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => ServiceCollection.CopyTo(array, arrayIndex);

        public bool Remove(ServiceDescriptor item) => ServiceCollection.Remove(item);

        public int Count => ServiceCollection.Count;
        public bool IsReadOnly => ServiceCollection.IsReadOnly;

        public int IndexOf(ServiceDescriptor item) => ServiceCollection.IndexOf(item);

        public void Insert(int index, ServiceDescriptor item) => ServiceCollection.Insert(index, item);

        public void RemoveAt(int index) => ServiceCollection.RemoveAt(index);

        public ServiceDescriptor this[int index]
        {
            get => ServiceCollection[index];
            set => ServiceCollection[index] = value;
        }
    }
}
