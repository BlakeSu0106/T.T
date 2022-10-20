using Telligent.Tag.Domain.Shared;
using Telligent.Tag.Application.Dtos.PoolTag;

namespace Telligent.Tag.Application.Dtos.PoolTag;

public class TagInfoDto
{
    public TagCategoryType TagCategoryType { get; set; }
    public Guid TagCategoryId { get; set; }
    public string TagCategoryName { get; set; }
    public List<TagDto> Tags { get; set; }
    public int Quantity { get; set; }
    public DateTime? LastTime { get; set; }
}