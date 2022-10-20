using Telligent.Core.Application.DataTransferObjects;
using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.Members;

public class ProspectDto : EntityDto
{
    /// <summary>
    /// 會員識別碼
    /// </summary>
    public Guid? MemberId { get; set; }

    /// <summary>
    /// 帳號來源類別
    /// </summary>
    public AccountType AccountType { get; set; }

    /// <summary>
    /// 登入帳號
    /// </summary>
    public string ThirdPartyUserId { get; set; }

    /// <summary>
    /// 暱稱
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    /// 是否訂閱官方帳號(頻道)
    /// </summary>
    public bool? IsSubscribedChannel { get; set; }
}