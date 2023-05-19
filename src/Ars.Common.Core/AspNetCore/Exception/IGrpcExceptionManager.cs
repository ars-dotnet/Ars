using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore
{
    public interface IGrpcExceptionManager
    {
        bool IsGrpcException(Exception e);

        (int, string) GetGrpcExceptionErr(Exception e);
    }
}
