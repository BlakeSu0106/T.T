using System.ComponentModel.DataAnnotations;
using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.TagTracking;

public class CreateTagTrackingDto : EntityDto
{
    internal new Guid Id { get; set; }

    /// <summary>
    /// 公司識別碼
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 渠道識別碼
    /// </summary>
    public Guid? ChannelId { get; set; }

    /// <summary>
    /// 參考事件識別碼
    /// </summary>
    public Guid? EventId { get; set; }

    /// <summary>
    /// 標籤所有對象類別
    /// </summary>
    [Required]
    public TagOwnerType TagOwnerType { get; set; }

    /// <summary>
    /// 標籤擁有對象識別碼
    /// </summary>
    [Required]
    public Guid TagOwnerId { get; set; }

    /// <summary>
    /// 標籤清單
    /// </summary>
    [Required]
    public IList<Guid> TagIds { get; set; }
}