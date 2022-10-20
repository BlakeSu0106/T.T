using AutoMapper;
using Microsoft.AspNetCore.Http;
using Telligent.Core.Application.Services;
using Telligent.Core.Domain.Repositories;
using Telligent.Tag.Application.Dtos.SystemEvent;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Application.AppServices;

public class SystemEventAppService : CrudAppService<SystemEvent, SystemEventDto, SystemEventDto, SystemEventDto>
{
    public SystemEventAppService(
        IRepository<SystemEvent> repository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, mapper, httpContextAccessor)
    {
    }

    /// <summary>
    /// 取得系統事件定義檔
    /// </summary>
    /// <param name="code">事件代碼</param>
    /// <returns>系統事件定義檔</returns>
    public async Task<SystemEventDto> GetByCodeAsync(string code)
    {
        var systemEventDtos = await GetListAsync(m =>
            m.Code.Equals(code) &&
            m.EntityStatus);

        return systemEventDtos.FirstOrDefault();
    }
}