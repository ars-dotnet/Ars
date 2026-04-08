using Ars.Common.Stage.Enums;

namespace Ars.Common.Stage.Models;

/// <summary>
/// 预定义站位定义
/// </summary>
public class StationDefinition
{
    public string Name { get; set; } = string.Empty;

    public CoordinateDefinition Coordinate { get; set; } = new();

    public EnumWaferSize WaferSize { get; set; }
}
