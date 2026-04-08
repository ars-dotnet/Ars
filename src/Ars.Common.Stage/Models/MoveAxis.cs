using Ars.Common.Stage.Enums;

namespace Ars.Common.Stage.Models;

/// <summary>
/// 单轴移动参数
/// </summary>
public class MoveAxis
{
    public EnumAxis Axis { get; set; }

    public float Value { get; set; }

    public EnumCoordSetType MoveType { get; set; }
}
