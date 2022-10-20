using Telligent.Core.Application.DataTransferObjects;

namespace Telligent.Tag.Application.Dtos.Tag;

public class UpdateTagDto : EntityDto
{
    /// <summary>
    /// 標籤名稱
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 啟用/停用
    /// </summary>
    public bool ActivationStatus { get; set; }
}