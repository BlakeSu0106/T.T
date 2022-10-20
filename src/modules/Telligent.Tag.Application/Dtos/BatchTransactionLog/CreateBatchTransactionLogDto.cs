using System.ComponentModel.DataAnnotations;
using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.BatchTransactionLog;

public class CreateBatchTransactionLogDto : EntityDto
{
    internal new Guid Id { get; set; }

    /// <summary>
    /// 公司識別碼
    /// </summary>
    [Required]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 操作行為
    /// </summary>
    [Required]
    public CommandType CommandType { get; set; }

    /// <summary>
    /// 資料交易筆數
    /// </summary>
    [Required]
    public int TransactionCount { get; set; }

    /// <summary>
    /// 交易成功筆數
    /// </summary>
    [Required]
    public int SuccessfulCount { get; set; }

    /// <summary>
    /// 交易失敗筆數
    /// </summary>
    [Required]
    public int FailureCount { get; set; }

    /// <summary>
    /// 交易訊息
    /// </summary>
    public string Message { get; set; }
}