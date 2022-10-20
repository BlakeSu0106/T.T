using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Application.Dtos.EventTag;

namespace Telligent.Tag.Application.Dtos.Event;

public class UpdateEventDto : EntityDto
{
    /// <summary>
    /// 標籤蒐集起始日期
    /// </summary>
    public DateTime? StickStartTime { get; set; }

    /// <summary>
    /// 標籤蒐集截止日期
    /// </summary>
    public DateTime? StickEndTime { get; set; }

    /// <summary>
    /// 啟用/停用
    /// </summary>
    public bool ActivationStatus { get; set; }

    /// <summary>
    /// 事件標籤設定資料
    /// </summary>
    public IList<CreateEventTagDto> EventTags { get; set; }
}