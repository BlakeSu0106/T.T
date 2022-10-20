using Telligent.Core.Application.DataTransferObjects;

namespace Telligent.Tag.Application.Dtos.Members;

public class MemberDto : EntityDto
{
    /// <summary>
    /// 客戶端識別碼
    /// </summary>
    public string ThirdPartyUserId { get; set; }

    /// <summary>
    /// 用戶頭像
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// 會員姓名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 會籍名稱
    /// </summary>
    public string MembershipName { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 電話國碼
    /// </summary>
    public string CountryCallingCode { get; set; }

    /// <summary>
    /// 會員手機
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreationTime { get; set; }
}