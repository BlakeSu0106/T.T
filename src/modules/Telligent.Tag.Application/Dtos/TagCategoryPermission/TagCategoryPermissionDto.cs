using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.TagCategoryPermission;

public class TagCategoryPermissionDto : EntityDto
{
    /// <summary>
    /// 公司識別碼
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 標籤分類類別
    /// </summary>
    public TagCategoryType CategoryType { get; set; }

    /// <summary>
    /// 標籤類別識別碼
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// 開放起始日期
    /// </summary>
    public DateTime? ActivationStartTime { get; set; }

    /// <summary>
    /// 開放截止日期
    /// </summary>
    public DateTime? ActivationEndTime { get; set; }

    /// <summary>
    /// 啟用/停用
    /// </summary>
    public bool ActivationStatus { get; set; }
}