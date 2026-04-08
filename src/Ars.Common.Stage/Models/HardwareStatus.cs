namespace Ars.Common.Stage.Models;

/// <summary>
/// 硬件状态
/// </summary>
public class HardwareStatus
{
    public bool IsNormal { get; set; }

    public string? StatusMessage { get; set; }

    public int ErrorCode { get; set; }
}
