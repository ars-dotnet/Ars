namespace Ars.Common.Stage.Enums;

/// <summary>
/// Stage 及 EFEM 通信操作类型枚举
/// </summary>
public enum EnumStageOperation
{
    #region EFEM

    /// <summary>传片通知（true：正在传片，false：传片完成）</summary>
    NoticeEFEMTransferWafer,

    #endregion

    #region 连接管理

    /// <summary>连接硬件</summary>
    Connect,

    /// <summary>断开硬件连接</summary>
    Disconnect,

    /// <summary>查询是否已连接</summary>
    IsConnect,

    /// <summary>获取 Stage 制造商标识</summary>
    GetStageManufacturer,

    #endregion

    #region 回零

    /// <summary>全轴回零（GoHome）</summary>
    FindHome,

    /// <summary>单轴回零</summary>
    FindHomeAxis,

    #endregion

    #region 运动控制

    /// <summary>多轴联动移动</summary>
    Move,

    /// <summary>单轴移动</summary>
    MoveAxis,

    /// <summary>停止所有轴</summary>
    Stop,

    /// <summary>停止单轴</summary>
    StopAxis,

    #endregion

    #region 轴使能

    /// <summary>使能轴</summary>
    EnableAxis,

    /// <summary>禁用轴</summary>
    DisableAxis,

    /// <summary>检查轴是否使能</summary>
    CheckEnabled,

    #endregion

    #region 位置读取

    /// <summary>读取所有轴当前位置</summary>
    ReadPosition,

    /// <summary>读取单轴当前位置</summary>
    ReadPositionAxis,

    /// <summary>读取所有轴位置误差</summary>
    ReadPositionError,

    /// <summary>读取单轴位置误差</summary>
    ReadPositionErrorAxis,

    #endregion

    #region 速度参数

    /// <summary>设置轴速度</summary>
    SetAxisSpeed,

    /// <summary>获取轴速度</summary>
    GetAxisSpeed,

    /// <summary>设置轴减速度</summary>
    SetAxisDeceSpeed,

    /// <summary>获取轴减速度</summary>
    GetAxisDeceSpeed,

    #endregion

    #region 限位查询

    /// <summary>获取轴最大软限位</summary>
    GetMaxLimit,

    /// <summary>获取轴最小软限位</summary>
    GetMinLimit,

    /// <summary>设置轴最小软限位</summary>
    SetMinLimit,

    /// <summary>检查 ISO 轴状态</summary>
    CheckIsoAxisStatus,

    /// <summary>获取活动轴列表</summary>
    GetActiveAxes,

    #endregion

    #region 环境/状态

    /// <summary>读取环境参数（温度、湿度等）</summary>
    ReadEnvironmentParam,

    /// <summary>获取硬件状态</summary>
    GetStageStatus,

    /// <summary>读取 Stage 默认参数配置</summary>
    ReadStageParameters,

    /// <summary>读取移动到位参数</summary>
    GetParamFloats,

    #endregion

    #region Wafer 管理

    /// <summary>获取当前 Wafer 尺寸</summary>
    GetCurrentWaferSize,

    /// <summary>更换 Wafer 尺寸</summary>
    ChangeWaferSize,

    /// <summary>装载 Wafer</summary>
    LoadWafer,

    /// <summary>卸载 Wafer</summary>
    UnloadWafer,

    /// <summary>检查 Stage 是否在卸载位置</summary>
    CheckStageAtUnloadLocation,

    /// <summary>检查 Wafer 是否存在</summary>
    CheckWaferExists,

    #endregion

    #region 站位管理

    /// <summary>获取预定义站位列表</summary>
    GetPredefinedStations,

    /// <summary>获取 Stage 位置</summary>
    GetStageLocation,

    #endregion

    #region 检测流程

    /// <summary>开始旋转（Spin）</summary>
    StartSpin,

    #endregion

    #region 参数调整

    /// <summary>设置缩放因子</summary>
    SetReductionFactor,

    #endregion

    #region 轴状态

    /// <summary>检查各轴状态</summary>
    CheckAxisStatus,

    #endregion
}
