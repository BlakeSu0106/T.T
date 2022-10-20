using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.EventTag;

public class EventTagDto : EntityDto
{
    /// <summary>
    /// 事件識別碼
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// 標籤識別碼
    /// </summary>
    public Guid TagId { get; set; }

    /// <summary>
    /// 標籤所有對象類別
    /// </summary>
    public TagOwnerType TagOwnerType { get; set; }

    /// <summary>
    /// 權重
    /// </summary>
    public decimal Weight { get; set; }
}