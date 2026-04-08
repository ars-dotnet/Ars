namespace Ars.Common.Stage.Models;

/// <summary>
/// 环境参数（温度、湿度等传感器数据）
/// </summary>
public class EnvironmentParam
{
    public string Name { get; set; } = string.Empty;

    public double Value { get; set; }

    public string Unit { get; set; } = string.Empty;
}
