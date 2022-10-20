using Telligent.Core.Application.DataTransferObjects;

namespace Telligent.Tag.Application.Dtos.Members;

public class UserDto : EntityDto
{
    public string UserId { get; set; }
    public string Name { get; set; }
}