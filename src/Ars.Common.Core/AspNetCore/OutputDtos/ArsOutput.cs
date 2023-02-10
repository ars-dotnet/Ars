using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.OutputDtos
{
    public class ArsOutput : IArsOutput 
    {
        public ArsOutput()
        {

        }

        public ArsOutput(int code,string error)
        {
            Code = code;
            Error = error;
        }

        public int Code { get; set; }

        public string Error { get; set; }
    }

    public class ArsOutput<T> : ArsOutput
    {
        public ArsOutput(T? data) : this(data,0)
        {

        }

        public ArsOutput(T? data, int code) : this(data, code, string.Empty) 
        {

        }

        public ArsOutput(T? data,int code,string error) : base(code,error)
        {
            Data = data;
        }

        public T? Data { get; set; }


        public static ArsOutput<T> Success(T data) => new ArsOutput<T>(data);

        public static ArsOutput<T> Failed(T? data,int code,string error) => new ArsOutput<T>(data, code,error);

        public static ArsOutput<T> Failed(int code, string error) => ArsOutput<T>.Failed(default(T),code,error);
    }
}
