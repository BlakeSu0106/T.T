using System.ComponentModel.DataAnnotations;
using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.TagTracking;

public class CreatePluralElectronicCommerceTagTrackingDto : EntityDto
{
    /// <summary>
    /// 公司識別碼
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 渠道識別碼
    /// </summary>
    public Guid? ChannelId { get; set; }

    /// <summary>
    /// 事件陣列
    /// </summary>
    public IList<CreateTagTrackingDto> TagTracking { get; set; }

}