using Telligent.Core.Application.DataTransferObjects;

namespace Telligent.Tag.Application.Dtos.TagCategoryPermission;

public class UpdateTagCategoryPermissionDto : EntityDto
{
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