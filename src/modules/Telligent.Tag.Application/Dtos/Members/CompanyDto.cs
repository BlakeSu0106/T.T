using Telligent.Core.Application.DataTransferObjects;

namespace Telligent.Tag.Application.Dtos.Members;

public class CompanyDto : EntityDto
{
    /// <summary>
    /// 租戶識別碼
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// 集團識別碼
    /// </summary>
    public Guid? GroupId { get; set; }

    /// <summary>
    /// 名稱
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
}