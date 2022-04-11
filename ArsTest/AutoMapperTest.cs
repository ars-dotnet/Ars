using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class AutoMapperTest
    {
        private IMapper _mapper;
        public AutoMapperTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(Assembly.GetAssembly(GetType()));

            _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
        }

        [Fact]
        public void Test() 
        {
            try
            {
                MapperA mapperA = new MapperA() { Name = "123", Top = "187m" };
                MapperB mapperB = _mapper.Map<MapperB>(mapperA);
            }
            catch (Exception e) 
            {

            }
        }
    }

    public class MapperA 
    {
        public string Name { get; set; }

        public string Top { get; set; }
    }

    public class MapperB 
    {
        public string Name { get; set; }

        public int age { get; set; }

        public bool A { get; set; }
    }

    public class AutoMapperProfile : Profile 
    {
        public AutoMapperProfile()
        {
            CreateMap<MapperA,MapperB>()
                .AfterMap((_,d) => d.A = true);
        }
    }
}
