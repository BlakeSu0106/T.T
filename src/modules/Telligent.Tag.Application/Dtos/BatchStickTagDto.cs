namespace Telligent.Tag.Application.Dtos;

public class BatchStickTagDto
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
    /// 標籤識別碼清單
    /// </summary>
    public List<Guid> TagIds { get; set; }

    /// <summary>
    /// 會員識別碼清單
    /// </summary>
    public List<Guid> MemberIds { get; set; }

    /// <summary>
    /// 潛客識別碼清單
    /// </summary>
    public List<Guid> ProspectIds { get; set; }
}