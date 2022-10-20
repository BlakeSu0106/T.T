using System.ComponentModel.DataAnnotations;
using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Application.Dtos.EventTag;

namespace Telligent.Tag.Application.Dtos.Event;

public class CreateEventDto : EntityDto
{
    internal new Guid Id { get; set; }

    /// <summary>
    /// 公司識別碼
    /// </summary>
    [Required]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 事件名稱
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 權重
    /// </summary>
    [Required]
    public decimal Weight { get; set; }

    /// <summary>
    /// 標籤蒐集起始日期
    /// </summary>
    public DateTime? StickStartTime { get; set; }

    /// <summary>
    /// 標籤蒐集截止日期
    /// </summary>
    public DateTime? StickEndTime { get; set; }

    /// <summary>
    /// 事件標籤設定資料
    /// </summary>
    public IList<CreateEventTagDto> EventTags { get; set; }
}