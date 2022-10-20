using Telligent.Core.Application.DataTransferObjects;

namespace Telligent.Tag.Application.Dtos.Members;

public class ChannelDto : EntityDto
{
    /// <summary>
    /// 公司識別碼
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 渠道名稱
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 第三方渠道id
    /// </summary>
    public string ThirdPartyChannelId { get; set; }
}