using Ars.Common.Stage.Enums;

namespace Ars.Common.Stage.Models;

/// <summary>
/// 轴操作参数（使能/禁用/检查/限位/速度等单轴参数传递）
/// </summary>
public class AxisParam
{
    public EnumAxis Axis { get; set; }

    public float Value { get; set; }
}
