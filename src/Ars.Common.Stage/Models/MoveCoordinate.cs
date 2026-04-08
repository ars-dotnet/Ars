using Ars.Common.Stage.Enums;

namespace Ars.Common.Stage.Models;

/// <summary>
/// 多轴联动移动参数
/// </summary>
public class MoveCoordinate
{
    public CoordinateDefinition Coordinate { get; set; } = new();

    public EnumCoordSetType MoveType { get; set; }
}
