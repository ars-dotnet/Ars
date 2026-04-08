using Ars.Common.Stage.Enums;

namespace Ars.Common.Stage.Models;

/// <summary>
/// 站位查询参数
/// </summary>
public class StageLocationQuery
{
    public EnumWaferSize WaferSize { get; set; }

    public string SrcStation { get; set; } = string.Empty;
}
