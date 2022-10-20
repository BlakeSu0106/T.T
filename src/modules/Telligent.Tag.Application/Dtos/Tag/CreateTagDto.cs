using System.ComponentModel.DataAnnotations;
using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.Tag;

public class CreateTagDto : EntityDto
{
    internal new Guid Id { get; set; }

    /// <summary>
    /// 公司識別碼
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 標籤分類類別
    /// </summary>
    [Required]
    public TagCategoryType CategoryType { get; set; }

    /// <summary>
    /// 標籤類別識別碼
    /// </summary>
    [Required]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// 標籤名稱
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 啟用/停用
    /// </summary>
    public bool ActivationStatus { get; set; }
}