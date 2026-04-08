using Ars.Common.Stage.Enums;

namespace Ars.Common.Stage.Models;

/// <summary>
/// 通信请求对象
/// </summary>
/// <typeparam name="TResult">期望的返回值类型</typeparam>
/// <typeparam name="TData">请求携带的数据类型</typeparam>
public class Request<TResult, TData>
{
    public EnumHardwareType HardwareType { get; set; }

    public EnumStageOperation CommandType { get; set; }

    public TData? Data { get; set; }
}
