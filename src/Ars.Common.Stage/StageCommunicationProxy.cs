using Ars.Common.Stage.Enums;
using Ars.Common.Stage.Models;

namespace Ars.Common.Stage;

/// <summary>
/// Stage / EFEM 通信代理基类。
/// 派生类须实现 <see cref="_request{TResult,TData}"/> 和
/// <see cref="_requestWithResult{TResult,TData}"/> 以接入具体 IPC 通道。
/// </summary>
public abstract class StageCommunicationProxyBase
{
    /// <summary>发送请求（不关注返回值）</summary>
    protected abstract void _request<TResult, TData>(Request<TResult, TData> request);

    /// <summary>发送请求并返回结果</summary>
    protected abstract Response<TResult> _requestWithResult<TResult, TData>(Request<TResult, TData> request);
}

/// <summary>
/// Stage / EFEM 通信代理，基于 <see cref="EnumStageOperation"/> 枚举封装所有
/// 硬件操作，并实现 <see cref="IStageHardwareService"/> 接口。
/// </summary>
public abstract class StageCommunicationProxy : StageCommunicationProxyBase, IStageHardwareService
{
    #region EFEM

    /// <summary>
    /// 传片通知
    /// </summary>
    /// <param name="isTransferWafer">true：正在传片，false：传片完成</param>
    public void EFEMTransferWaferNotice(bool isTransferWafer)
    {
        var request = new Request<string, string>
        {
            HardwareType = EnumHardwareType.EFEM,
            CommandType = EnumStageOperation.NoticeEFEMTransferWafer,
            Data = isTransferWafer.ToString()
        };
        _request<string, string>(request);
    }

    #endregion

    #region 连接管理

    /// <inheritdoc/>
    public void Connect()
    {
        var request = new Request<string, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.Connect
        };
        _request<string, string>(request);
    }

    /// <inheritdoc/>
    public void Disconnect()
    {
        var request = new Request<string, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.Disconnect
        };
        _request<string, string>(request);
    }

    /// <inheritdoc/>
    public bool IsConnected
    {
        get
        {
            var request = new Request<bool, string>
            {
                HardwareType = EnumHardwareType.Stage,
                CommandType = EnumStageOperation.IsConnect
            };
            var rlt = _requestWithResult<bool, string>(request);
            return rlt.Data;
        }
    }

    /// <inheritdoc/>
    public EnumStageManufacturer StageManufacturer
    {
        get
        {
            var request = new Request<EnumStageManufacturer, string>
            {
                HardwareType = EnumHardwareType.Stage,
                CommandType = EnumStageOperation.GetStageManufacturer
            };
            var rlt = _requestWithResult<EnumStageManufacturer, string>(request);
            return rlt.Data;
        }
    }

    #endregion

    #region 回零

    /// <summary>全轴回零（GoHome）</summary>
    public void FindHome()
    {
        var request = new Request<string, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.FindHome
        };
        _request<string, string>(request);
    }

    /// <summary>单轴回零</summary>
    public void FindHome(EnumAxis axis)
    {
        var request = new Request<string, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.FindHomeAxis,
            Data = axis
        };
        _request<string, EnumAxis>(request);
    }

    #endregion

    #region 运动控制

    /// <inheritdoc/>
    public StageActionResult Move(CoordinateDefinition coord, EnumCoordSetType moveType = EnumCoordSetType.Absolute, int millisecondsTimeout = -1)
    {
        var request = new Request<StageActionResult, MoveCoordinate>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.Move,
            Data = new MoveCoordinate { Coordinate = coord, MoveType = moveType }
        };
        var rlt = _requestWithResult<StageActionResult, MoveCoordinate>(request);
        return rlt.Data ?? new StageActionResult { IsSuccess = false, ErrorMessage = "No response" };
    }

    /// <inheritdoc/>
    public StageActionResult Move(EnumAxis axis, float value, EnumCoordSetType moveType = EnumCoordSetType.Absolute, int millisecondsTimeout = -1)
    {
        var request = new Request<StageActionResult, MoveAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.MoveAxis,
            Data = new MoveAxis { Axis = axis, Value = value, MoveType = moveType }
        };
        var rlt = _requestWithResult<StageActionResult, MoveAxis>(request);
        return rlt.Data ?? new StageActionResult { IsSuccess = false, ErrorMessage = "No response" };
    }

    /// <inheritdoc/>
    public void Stop()
    {
        var request = new Request<string, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.Stop
        };
        _request<string, string>(request);
    }

    /// <inheritdoc/>
    public void Stop(EnumAxis axis)
    {
        var request = new Request<string, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.StopAxis,
            Data = axis
        };
        _request<string, EnumAxis>(request);
    }

    #endregion

    #region 轴使能

    /// <inheritdoc/>
    public void Enable(EnumAxis axis)
    {
        var request = new Request<string, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.EnableAxis,
            Data = axis
        };
        _request<string, EnumAxis>(request);
    }

    /// <inheritdoc/>
    public void Disable(EnumAxis axis)
    {
        var request = new Request<string, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.DisableAxis,
            Data = axis
        };
        _request<string, EnumAxis>(request);
    }

    /// <inheritdoc/>
    public bool CheckEnabled(EnumAxis axis)
    {
        var request = new Request<bool, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.CheckEnabled,
            Data = axis
        };
        var rlt = _requestWithResult<bool, EnumAxis>(request);
        return rlt.Data;
    }

    #endregion

    #region 位置读取

    /// <inheritdoc/>
    public CoordinateDefinition ReadPosition()
    {
        var request = new Request<CoordinateDefinition, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.ReadPosition
        };
        var rlt = _requestWithResult<CoordinateDefinition, string>(request);
        return rlt.Data ?? new CoordinateDefinition();
    }

    /// <inheritdoc/>
    public float ReadPosition(EnumAxis axis)
    {
        var request = new Request<float, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.ReadPositionAxis,
            Data = axis
        };
        var rlt = _requestWithResult<float, EnumAxis>(request);
        return rlt.Data;
    }

    /// <inheritdoc/>
    public StagePositionError ReadPositionError()
    {
        var request = new Request<StagePositionError, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.ReadPositionError
        };
        var rlt = _requestWithResult<StagePositionError, string>(request);
        return rlt.Data ?? new StagePositionError();
    }

    /// <inheritdoc/>
    public float ReadPositionError(EnumAxis axis)
    {
        var request = new Request<float, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.ReadPositionErrorAxis,
            Data = axis
        };
        var rlt = _requestWithResult<float, EnumAxis>(request);
        return rlt.Data;
    }

    #endregion

    #region 速度参数

    /// <inheritdoc/>
    public void SetAxisSpeed(EnumAxis axis, float speed)
    {
        var request = new Request<string, AxisParam>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.SetAxisSpeed,
            Data = new AxisParam { Axis = axis, Value = speed }
        };
        _request<string, AxisParam>(request);
    }

    /// <inheritdoc/>
    public float GetAxisSpeed(EnumAxis axis)
    {
        var request = new Request<float, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.GetAxisSpeed,
            Data = axis
        };
        var rlt = _requestWithResult<float, EnumAxis>(request);
        return rlt.Data;
    }

    /// <inheritdoc/>
    public void SetAxisDeceSpeed(EnumAxis axis, float speed)
    {
        var request = new Request<string, AxisParam>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.SetAxisDeceSpeed,
            Data = new AxisParam { Axis = axis, Value = speed }
        };
        _request<string, AxisParam>(request);
    }

    /// <inheritdoc/>
    public float GetAxisDeceSpeed(EnumAxis axis)
    {
        var request = new Request<float, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.GetAxisDeceSpeed,
            Data = axis
        };
        var rlt = _requestWithResult<float, EnumAxis>(request);
        return rlt.Data;
    }

    #endregion

    #region 限位查询

    /// <inheritdoc/>
    public float GetMaxLimit(EnumAxis axis)
    {
        var request = new Request<float, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.GetMaxLimit,
            Data = axis
        };
        var rlt = _requestWithResult<float, EnumAxis>(request);
        return rlt.Data;
    }

    /// <inheritdoc/>
    public float GetMinLimit(EnumAxis axis)
    {
        var request = new Request<float, EnumAxis>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.GetMinLimit,
            Data = axis
        };
        var rlt = _requestWithResult<float, EnumAxis>(request);
        return rlt.Data;
    }

    /// <inheritdoc/>
    public bool SetMinLimit(EnumAxis axis, float pos)
    {
        var request = new Request<bool, AxisParam>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.SetMinLimit,
            Data = new AxisParam { Axis = axis, Value = pos }
        };
        var rlt = _requestWithResult<bool, AxisParam>(request);
        return rlt.Data;
    }

    /// <inheritdoc/>
    public List<EnumAxis> ActiveAxes
    {
        get
        {
            var request = new Request<List<EnumAxis>, string>
            {
                HardwareType = EnumHardwareType.Stage,
                CommandType = EnumStageOperation.GetActiveAxes
            };
            var rlt = _requestWithResult<List<EnumAxis>, string>(request);
            return rlt.Data ?? new List<EnumAxis>();
        }
    }

    /// <inheritdoc/>
    public HardwareStatus CheckIsoAxisStatus()
    {
        var request = new Request<HardwareStatus, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.CheckIsoAxisStatus
        };
        var rlt = _requestWithResult<HardwareStatus, string>(request);
        return rlt.Data ?? new HardwareStatus();
    }

    #endregion

    #region 环境/状态

    /// <inheritdoc/>
    public List<EnvironmentParam> ReadEnvironmentParam()
    {
        var request = new Request<List<EnvironmentParam>, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.ReadEnvironmentParam
        };
        var rlt = _requestWithResult<List<EnvironmentParam>, string>(request);
        return rlt.Data ?? new List<EnvironmentParam>();
    }

    /// <inheritdoc/>
    public HardwareStatus GetStageStatus()
    {
        var request = new Request<HardwareStatus, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.GetStageStatus
        };
        var rlt = _requestWithResult<HardwareStatus, string>(request);
        return rlt.Data ?? new HardwareStatus();
    }

    /// <inheritdoc/>
    public StageDefaultParameters ReadStageParameters()
    {
        var request = new Request<StageDefaultParameters, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.ReadStageParameters
        };
        var rlt = _requestWithResult<StageDefaultParameters, string>(request);
        return rlt.Data ?? new StageDefaultParameters();
    }

    /// <inheritdoc/>
    public Dictionary<string, string> GetParamFloats()
    {
        var request = new Request<Dictionary<string, string>, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.GetParamFloats
        };
        var rlt = _requestWithResult<Dictionary<string, string>, string>(request);
        return rlt.Data ?? new Dictionary<string, string>();
    }

    #endregion

    #region Wafer 管理

    /// <inheritdoc/>
    public EnumWaferSize CurrentWaferSize
    {
        get
        {
            var request = new Request<EnumWaferSize, string>
            {
                HardwareType = EnumHardwareType.Stage,
                CommandType = EnumStageOperation.GetCurrentWaferSize
            };
            var rlt = _requestWithResult<EnumWaferSize, string>(request);
            return rlt.Data;
        }
    }

    /// <inheritdoc/>
    public void ChangeWaferSize(EnumWaferSize waferSize)
    {
        var request = new Request<string, EnumWaferSize>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.ChangeWaferSize,
            Data = waferSize
        };
        _request<string, EnumWaferSize>(request);
    }

    /// <inheritdoc/>
    public void LoadWafer(EnumWaferSize waferSize, string srcStation)
    {
        var request = new Request<string, WaferOperation>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.LoadWafer,
            Data = new WaferOperation { WaferSize = waferSize, SrcStation = srcStation }
        };
        _request<string, WaferOperation>(request);
    }

    /// <inheritdoc/>
    public void UnloadWafer(EnumWaferSize waferSize, string srcStation)
    {
        var request = new Request<string, WaferOperation>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.UnloadWafer,
            Data = new WaferOperation { WaferSize = waferSize, SrcStation = srcStation }
        };
        _request<string, WaferOperation>(request);
    }

    /// <inheritdoc/>
    public bool CheckStageAtUnloadLocation()
    {
        var request = new Request<bool, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.CheckStageAtUnloadLocation
        };
        var rlt = _requestWithResult<bool, string>(request);
        return rlt.Data;
    }

    /// <inheritdoc/>
    public bool CheckWaferExists()
    {
        var request = new Request<bool, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.CheckWaferExists
        };
        var rlt = _requestWithResult<bool, string>(request);
        return rlt.Data;
    }

    #endregion

    #region 站位管理

    /// <inheritdoc/>
    public List<StationDefinition> GetPredefinedStations()
    {
        var request = new Request<List<StationDefinition>, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.GetPredefinedStations
        };
        var rlt = _requestWithResult<List<StationDefinition>, string>(request);
        return rlt.Data ?? new List<StationDefinition>();
    }

    /// <inheritdoc/>
    public EnumStageLocation GetStageLocation(EnumWaferSize waferSize, string srcStation)
    {
        var request = new Request<EnumStageLocation, StageLocationQuery>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.GetStageLocation,
            Data = new StageLocationQuery { WaferSize = waferSize, SrcStation = srcStation }
        };
        var rlt = _requestWithResult<EnumStageLocation, StageLocationQuery>(request);
        return rlt.Data;
    }

    #endregion

    #region 检测流程

    /// <inheritdoc/>
    public void StartSpin(float speed)
    {
        var request = new Request<string, float>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.StartSpin,
            Data = speed
        };
        _request<string, float>(request);
    }

    #endregion

    #region 参数调整

    /// <inheritdoc/>
    public void SetReductionFactor(double factor)
    {
        var request = new Request<string, double>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.SetReductionFactor,
            Data = factor
        };
        _request<string, double>(request);
    }

    #endregion

    #region 轴状态

    /// <inheritdoc/>
    public List<AxisStatus> CheckAxisStatus()
    {
        var request = new Request<List<AxisStatus>, string>
        {
            HardwareType = EnumHardwareType.Stage,
            CommandType = EnumStageOperation.CheckAxisStatus
        };
        var rlt = _requestWithResult<List<AxisStatus>, string>(request);
        return rlt.Data ?? new List<AxisStatus>();
    }

    #endregion
}
