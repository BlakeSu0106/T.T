using Telligent.Core.Application.DataTransferObjects;

namespace Telligent.Tag.Application.Dtos.CustomizationTagCategory;

public class CustomizationTagCategoryDto : EntityDto
{
    /// <summary>
    /// 公司識別碼
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 類別名稱
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 排序編號
    /// </summary>
    public int SeqNo { get; set; }

    /// <summary>
    /// 圖示路徑
    /// </summary>
    public string IconUrl { get; set; }

    /// <summary>
    /// 標的物僅可擁有一張該分類下的標籤
    /// </summary>
    public bool? IsUnique { get; set; }

    /// <summary>
    /// 標籤數量
    /// </summary>
    public int TagQuantity { get; set; }
}