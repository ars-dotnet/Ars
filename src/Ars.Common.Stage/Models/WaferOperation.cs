using Ars.Common.Stage.Enums;

namespace Ars.Common.Stage.Models;

/// <summary>
/// Wafer 装卸参数
/// </summary>
public class WaferOperation
{
    public EnumWaferSize WaferSize { get; set; }

    public string SrcStation { get; set; } = string.Empty;
}
