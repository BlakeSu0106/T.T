using Telligent.Tag.Domain.Shared;

namespace Telligent.Tag.Application.Dtos.Tag;

public class PoolTagDto
{
    public TagCategoryType TagCategoryType { get; set; }
    public Guid TagCategoryId { get; set; }
    public Guid TagId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int TotalQuantity { get; set; }
    public DateTime? LastTime { get; set; }
}