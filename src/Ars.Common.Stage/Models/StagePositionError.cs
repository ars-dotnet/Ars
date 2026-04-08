using Ars.Common.Stage.Enums;

namespace Ars.Common.Stage.Models;

/// <summary>
/// 各轴位置误差
/// </summary>
public class StagePositionError
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float T { get; set; }
    public float W { get; set; }

    public float this[EnumAxis axis] => axis switch
    {
        EnumAxis.X => X,
        EnumAxis.Y => Y,
        EnumAxis.Z => Z,
        EnumAxis.T => T,
        EnumAxis.W => W,
        _ => throw new ArgumentOutOfRangeException(nameof(axis))
    };
}
