using System.ComponentModel.DataAnnotations.Schema;
using Telligent.Core.Domain.Entities;

namespace Telligent.Tag.Domain.Tags;

/// <summary>
/// 系統事件定義檔
/// </summary>
[Table("system_event")]
public class SystemEvent : Entity
{
    /// <summary>
    /// 事件代碼
    /// </summary>
    [Column("code")]
    public string Code { get; set; }

    /// <summary>
    /// 事件名稱
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    /// 啟用/停用
    /// </summary>
    [Column("activation_status")]
    public bool ActivationStatus { get; set; }
}