using System.ComponentModel.DataAnnotations.Schema;
using Telligent.Core.Domain.Entities;

namespace Telligent.Tag.Domain.Tags;

/// <summary>
/// 自訂標籤類別
/// </summary>
[Table("customization_tag_category")]
public class CustomizationTagCategory : Entity
{
    /// <summary>
    /// 公司識別碼
    /// </summary>
    [Column("company_id")]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 類別名稱
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    /// 排序編號
    /// </summary>
    [Column("seq_no")]
    public int SeqNo { get; set; }

    /// <summary>
    /// 圖示路徑
    /// </summary>
    [Column("icon_url")]
    public string IconUrl { get; set; }

    /// <summary>
    /// 標的物僅可擁有一張該分類下的標籤
    /// </summary>
    [Column("is_unique")]
    public bool? IsUnique { get; set; }

    /// <summary>
    /// 啟用/停用
    /// </summary>
    [Column("activation_status")]
    public bool ActivationStatus { get; set; }
}