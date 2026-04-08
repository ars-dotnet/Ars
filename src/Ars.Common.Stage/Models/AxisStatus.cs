using Ars.Common.Stage.Enums;

namespace Ars.Common.Stage.Models;

/// <summary>
/// 轴状态信息
/// </summary>
public class AxisStatus
{
    public EnumAxis Axis { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsHomed { get; set; }

    public bool IsInError { get; set; }

    public string? ErrorMessage { get; set; }
}
