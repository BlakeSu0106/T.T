using System.ComponentModel.DataAnnotations;
using Telligent.Core.Application.DataTransferObjects;

namespace Telligent.Tag.Application.Dtos.CustomizationTagCategory;

public class CreateCustomizationTagCategoryDto : EntityDto
{
    internal new Guid Id { get; set; }

    /// <summary>
    /// 類別名稱
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 排序編號
    /// </summary>
    [Required]
    public int SeqNo { get; set; }

    /// <summary>
    /// 圖示路徑
    /// </summary>
    public string IconUrl { get; set; }

    /// <summary>
    /// 標的物僅可擁有一張該分類下的標籤
    /// </summary>
    public bool? IsUnique { get; set; }
}