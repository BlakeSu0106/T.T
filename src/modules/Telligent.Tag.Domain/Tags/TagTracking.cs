using System.ComponentModel.DataAnnotations.Schema;
using Telligent.Core.Domain.Entities;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Domain.Tags;

/// <summary>
/// 標籤追蹤檔
/// </summary>
[Table("tag_tracking")]
public class TagTracking : Entity
{
    /// <summary>
    /// 公司識別碼
    /// </summary>
    [Column("company_id")]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 渠道識別碼
    /// </summary>
    [Column("channel_id")]
    public Guid? ChannelId { get; set; }

    /// <summary>
    /// 參考事件識別碼
    /// </summary>
    [Column("event_id")]
    public Guid? EventId { get; set; }

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
    /// 標籤擁有對象識別碼
    /// </summary>
    [Column("tag_own_id")]
    public Guid TagOwnerId { get; set; }
}