namespace Ars.Common.Stage.Models;

/// <summary>
/// 通信响应对象
/// </summary>
/// <typeparam name="TResult">响应数据类型</typeparam>
public class Response<TResult>
{
    public bool IsSuccess { get; set; }

    public TResult? Data { get; set; }

    public string? ErrorMessage { get; set; }
}
