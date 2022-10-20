using Telligent.Core.Application.DataTransferObjects;

namespace Telligent.Tag.Application.Dtos.SystemEvent;

public class SystemEventDto : EntityDto
{
    /// <summary>
    /// 事件代碼
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 事件名稱
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 啟用/停用
    /// </summary>
    public bool ActivationStatus { get; set; }
}