using System.ComponentModel.DataAnnotations.Schema;
using Telligent.Core.Domain.Entities;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Domain.Tags;

/// <summary>
/// 標籤
/// </summary>
[Table("tag")]
public class Tag : Entity
{
    /// <summary>
    /// 公司識別碼
    /// </summary>
    [Column("company_id")]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 標籤分類類別
    /// </summary>
    [Column("category_type")]
    public TagCategoryType CategoryType { get; set; }

    /// <summary>
    /// 標籤類別識別碼
    /// </summary>
    [Column("category_id")]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// 標籤名稱
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    /// 開放起始日期
    /// </summary>
    [Column("activation_start_time")]
    public DateTime? ActivationStartTime { get; set; }

    /// <summary>
    /// 開放截止日期
    /// </summary>
    [Column("activation_end_time")]
    public DateTime? ActivationEndTime { get; set; }

    /// <summary>
    /// 啟用/停用
    /// </summary>
    [Column("activation_status")]
    public bool ActivationStatus { get; set; }

    /// <summary>
    /// 新舊資料同步
    /// </summary>
    [Column("is_sync")]
    public bool IsSync { get; set; }
}