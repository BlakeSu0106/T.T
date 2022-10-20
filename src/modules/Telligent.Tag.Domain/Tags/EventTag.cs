using System.ComponentModel.DataAnnotations.Schema;
using Telligent.Core.Domain.Entities;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Domain.Tags;

/// <summary>
/// 事件標籤設定檔
/// </summary>
[Table("event_tag")]
public class EventTag : Entity
{
    /// <summary>
    /// 事件識別碼
    /// </summary>
    [Column("event_id")]
    public Guid EventId { get; set; }

    /// <summary>
    /// 標籤識別碼
    /// </summary>
    [Column("tag_id")]
    public Guid TagId { get; set; }

    /// <summary>
    /// 標籤所有對象類別
    /// </summary>
    [Column("tag_owner_type")]
    public TagOwnerType TagOwnerType { get; set; }

    /// <summary>
    /// 權重
    /// </summary>
    [Column("weight")]
    public decimal Weight { get; set; }
}