using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.BatchTransactionLog;

public class BatchTransactionLogDto : EntityDto
{
    /// <summary>
    /// 公司識別碼
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 操作行為
    /// </summary>
    public CommandType CommandType { get; set; }

    /// <summary>
    /// 資料交易筆數
    /// </summary>
    public int TransactionCount { get; set; }

    /// <summary>
    /// 交易成功筆數
    /// </summary>
    public int SuccessfulCount { get; set; }

    /// <summary>
    /// 交易失敗筆數
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// 交易訊息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string CreatorName { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime? CreationTime { get; set; }
}