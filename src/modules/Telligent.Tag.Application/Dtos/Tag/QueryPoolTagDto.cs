namespace Telligent.Tag.Application.Dtos.Tag;

public class QueryPoolTagDto
{
    public Guid TagOwnerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<Guid> TagCategoryIds { get; set; }
    public string Name { get; set; }
}