using System.ComponentModel.DataAnnotations;
using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.EventTag;

public class CreateEventTagDto : EntityDto
{
    internal new Guid Id { get; set; }

    /// <summary>
    /// 事件識別碼
    /// </summary>
    [Required]
    public Guid EventId { get; set; }

    /// <summary>
    /// 標籤識別碼
    /// </summary>
    [Required]
    public Guid TagId { get; set; }

    /// <summary>
    /// 標籤所有對象類別
    /// </summary>
    [Required]
    public TagOwnerType TagOwnerType { get; set; }

    /// <summary>
    /// 權重
    /// </summary>
    [Required]
    public decimal Weight { get; set; }
}