using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.Tag;

public class TagDto : EntityDto
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
    /// 標籤名稱
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 建立人員姓名
    /// </summary>
    public string CreatorName { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime? CreationTime { get; set; }
}