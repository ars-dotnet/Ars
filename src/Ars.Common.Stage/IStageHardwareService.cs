using Ars.Common.Stage.Enums;
using Ars.Common.Stage.Models;

namespace Ars.Common.Stage;

/// <summary>
/// Stage 硬件服务接口
/// </summary>
public interface IStageHardwareService
{
    #region 连接管理

    /// <summary>连接硬件</summary>
    void Connect();

    /// <summary>断开硬件连接</summary>
    void Disconnect();

    /// <summary>是否已连接</summary>
    bool IsConnected { get; }

    /// <summary>Stage 制造商标识</summary>
    EnumStageManufacturer StageManufacturer { get; }

    #endregion

    #region 回零

    /// <summary>全轴回零</summary>
    void FindHome();

    /// <summary>单轴回零</summary>
    void FindHome(EnumAxis axis);

    #endregion

    #region 运动控制

    /// <summary>多轴联动移动</summary>
    /// <param name="coord">目标坐标 (Unit: mm / degree)</param>
    /// <param name="moveType">绝对/相对</param>
    /// <param name="millisecondsTimeout">超时时间</param>
    StageActionResult Move(CoordinateDefinition coord, EnumCoordSetType moveType = EnumCoordSetType.Absolute, int millisecondsTimeout = -1);

    /// <summary>单轴移动</summary>
    /// <param name="axis">轴</param>
    /// <param name="value">目标值 (Unit: mm / degree)</param>
    /// <param name="moveType">绝对/相对</param>
    /// <param name="millisecondsTimeout">超时时间</param>
    StageActionResult Move(EnumAxis axis, float value, EnumCoordSetType moveType = EnumCoordSetType.Absolute, int millisecondsTimeout = -1);

    /// <summary>停止所有轴</summary>
    void Stop();

    /// <summary>停止单轴</summary>
    void Stop(EnumAxis axis);

    #endregion

    #region 轴使能

    /// <summary>使能轴</summary>
    void Enable(EnumAxis axis);

    /// <summary>禁用轴</summary>
    void Disable(EnumAxis axis);

    /// <summary>检查轴是否使能</summary>
    bool CheckEnabled(EnumAxis axis);

    #endregion

    #region 位置读取

    /// <summary>读取所有轴当前位置</summary>
    CoordinateDefinition ReadPosition();

    /// <summary>读取单轴当前位置</summary>
    float ReadPosition(EnumAxis axis);

    /// <summary>读取所有轴位置误差</summary>
    StagePositionError ReadPositionError();

    /// <summary>读取单轴位置误差</summary>
    float ReadPositionError(EnumAxis axis);

    #endregion

    #region 速度参数

    /// <summary>设置轴速度 (Unit: mm/s)</summary>
    void SetAxisSpeed(EnumAxis axis, float speed);

    /// <summary>获取轴速度 (Unit: mm/s)</summary>
    float GetAxisSpeed(EnumAxis axis);

    /// <summary>设置轴减速度 (Unit: mm/s²)</summary>
    void SetAxisDeceSpeed(EnumAxis axis, float speed);

    /// <summary>获取轴减速度 (Unit: mm/s²)</summary>
    float GetAxisDeceSpeed(EnumAxis axis);

    #endregion

    #region 限位查询

    /// <summary>获取轴的最大软限位</summary>
    float GetMaxLimit(EnumAxis axis);

    /// <summary>获取轴的最小软限位</summary>
    float GetMinLimit(EnumAxis axis);

    /// <summary>设置轴的最小软限位</summary>
    bool SetMinLimit(EnumAxis axis, float pos);

    /// <summary>活动轴列表</summary>
    List<EnumAxis> ActiveAxes { get; }

    /// <summary>检查 ISO 轴状态</summary>
    HardwareStatus CheckIsoAxisStatus();

    #endregion

    #region 环境/状态

    /// <summary>读取环境参数（温度、湿度等传感器数据）</summary>
    List<EnvironmentParam> ReadEnvironmentParam();

    /// <summary>获取硬件状态</summary>
    HardwareStatus GetStageStatus();

    /// <summary>读取 Stage 默认参数配置</summary>
    StageDefaultParameters ReadStageParameters();

    /// <summary>读取移动到位参数</summary>
    Dictionary<string, string> GetParamFloats();

    #endregion

    #region Wafer 管理

    /// <summary>当前 Wafer 尺寸</summary>
    EnumWaferSize CurrentWaferSize { get; }

    /// <summary>更换 Wafer 尺寸</summary>
    void ChangeWaferSize(EnumWaferSize waferSize);

    /// <summary>装载 Wafer</summary>
    void LoadWafer(EnumWaferSize waferSize, string srcStation);

    /// <summary>卸载 Wafer</summary>
    void UnloadWafer(EnumWaferSize waferSize, string srcStation);

    /// <summary>检查 Stage 是否在卸载位置</summary>
    bool CheckStageAtUnloadLocation();

    /// <summary>检查 Wafer 是否存在</summary>
    bool CheckWaferExists();

    #endregion

    #region 站位管理

    /// <summary>获取预定义站位列表</summary>
    List<StationDefinition> GetPredefinedStations();

    /// <summary>获取 Stage 位置</summary>
    EnumStageLocation GetStageLocation(EnumWaferSize waferSize, string srcStation);

    #endregion

    #region 检测流程

    /// <summary>开始旋转</summary>
    void StartSpin(float speed);

    #endregion

    #region 参数调整

    /// <summary>设置缩放因子</summary>
    void SetReductionFactor(double factor);

    #endregion

    /// <summary>检查各轴状态</summary>
    List<AxisStatus> CheckAxisStatus();
}
