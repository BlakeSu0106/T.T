using System.ComponentModel.DataAnnotations.Schema;
using Telligent.Core.Domain.Entities;

namespace Telligent.Tag.Domain.Tags;

/// <summary>
/// 事件設定檔
/// </summary>
[Table("event")]
public class Event : Entity
{
    /// <summary>
    /// 公司識別碼
    /// </summary>
    [Column("company_id")]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 事件名稱
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    /// 權重
    /// </summary>
    [Column("weight")]
    public decimal Weight { get; set; }

    /// <summary>
    /// 標籤蒐集起始日期
    /// </summary>
    [Column("stick_start_time")]
    public DateTime? StickStartTime { get; set; }

    /// <summary>
    /// 標籤蒐集截止日期
    /// </summary>
    [Column("stick_end_time")]
    public DateTime? StickEndTime { get; set; }

    /// <summary>
    /// 啟用/停用
    /// </summary>
    [Column("activation_status")]
    public bool ActivationStatus { get; set; }
}