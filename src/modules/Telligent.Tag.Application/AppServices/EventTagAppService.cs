using AutoMapper;
using Microsoft.AspNetCore.Http;
using Telligent.Core.Application.Services;
using Telligent.Core.Domain.Repositories;
using Telligent.Tag.Application.Dtos.EventTag;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Application.AppServices;

public class EventTagAppService :
    CrudAppService<EventTag, EventTagDto, CreateEventTagDto, EventTagDto>
{
    public EventTagAppService(
        IRepository<EventTag> repository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, mapper, httpContextAccessor)
    {
    }

    /// <summary>
    /// 取得事件標籤設定檔
    /// </summary>
    /// <param name="tagId">標籤識別碼</param>
    /// <returns>事件標籤設定檔</returns>
    public async Task<IList<EventTagDto>> GetByTagIdAsync(Guid tagId)
    {
        return await GetListAsync(m =>
            m.TagId.Equals(tagId) &&
            m.EntityStatus);
    }

    /// <summary>
    /// 取得事件標籤設定檔
    /// </summary>
    /// <param name="eventId">事件識別碼</param>
    /// <returns>事件標籤設定檔</returns>
    public async Task<IList<EventTagDto>> GetByEventIdAsync(Guid eventId)
    {
        return await GetListAsync(m =>
            m.EventId.Equals(eventId) &&
            m.EntityStatus);
    }
}