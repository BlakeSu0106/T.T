using System.ComponentModel.DataAnnotations.Schema;
using Telligent.Core.Domain.Entities;

namespace Telligent.Tag.Domain.Tags;

/// <summary>
/// 行為標籤類別
/// </summary>
[Table("behavior_tag_category")]
public class BehaviorTagCategory : Entity
{
    /// <summary>
    /// 類別代碼
    /// </summary>
    [Column("code")]
    public string Code { get; set; }

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
    /// 連結外部系統
    /// </summary>
    [Column("is_connection_external_system")]
    public bool? IsConnectionExternalSystem { get; set; }
}