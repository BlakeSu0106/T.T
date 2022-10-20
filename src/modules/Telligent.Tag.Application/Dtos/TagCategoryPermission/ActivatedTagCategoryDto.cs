using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.TagCategoryPermission;

public class ActivatedTagCategoryDto
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
    /// 類別名稱
    /// </summary>
    public string Name { get; set; }
}