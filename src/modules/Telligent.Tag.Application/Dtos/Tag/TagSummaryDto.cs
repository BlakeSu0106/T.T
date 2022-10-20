namespace Telligent.Tag.Application.Dtos.Tag;

public class TagSummaryDto
{
    /// <summary>
    /// 標籤識別碼
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 標籤名稱
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 事件/項目 綁定
    /// </summary>
    public bool EventBinding { get; set; }

    /// <summary>
    /// 貼標人數
    /// </summary>
    public int UsedQuantity { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string CreatorName { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime? CreationTime { get; set; }
}