using System.ComponentModel.DataAnnotations.Schema;
using Telligent.Core.Domain.Entities;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Domain.Tags;

/// <summary>
/// 批量交易記錄檔
/// </summary>
[Table("batch_transaction_log")]
public class BatchTransactionLog : Entity
{
    /// <summary>
    /// 公司識別碼
    /// </summary>
    [Column("company_id")]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 操作行為類別
    /// </summary>
    [Column("command_type")]
    public CommandType CommandType { get; set; }

    /// <summary>
    /// 資料交易筆數
    /// </summary>
    [Column("transaction_count")]
    public int TransactionCount { get; set; }

    /// <summary>
    /// 交易成功筆數
    /// </summary>
    [Column("successful_count")]
    public int SuccessfulCount { get; set; }

    /// <summary>
    /// 交易失敗筆數
    /// </summary>
    [Column("failure_count")]
    public int FailureCount { get; set; }

    /// <summary>
    /// 交易訊息
    /// </summary>
    [Column("message")]
    public string Message { get; set; }
}