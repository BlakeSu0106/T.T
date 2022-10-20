using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Telligent.Core.Application.Services;
using Telligent.Core.Domain.Repositories;
using Telligent.Core.Infrastructure.Generators;
using Telligent.Tag.Application.Dtos.Event;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Application.AppServices;

public class EventAppService :
    CrudAppService<Event, EventDto, CreateEventDto, UpdateEventDto>
{
    private readonly EventTagAppService _eventTagService;
    private readonly MemberAppService _memberService;
    private readonly TagTrackingAppService _tagTrackingService;
    private readonly UnitOfWork _uow;

    private string _companyId;
    private string _userId;

    public EventAppService(
        IRepository<Event> repository,
        IMapper mapper,
        EventTagAppService eventTagService,
        MemberAppService memberService,
        TagTrackingAppService tagTrackingService,
        IHttpContextAccessor httpContextAccessor,
        UnitOfWork uow)
        : base(repository, mapper, httpContextAccessor)
    {
        _eventTagService = eventTagService;
        _memberService = memberService;
        _tagTrackingService = tagTrackingService;
        _uow = uow;

        if (httpContextAccessor.HttpContext == null) return;

        _companyId = httpContextAccessor.HttpContext.Request.Headers["Company"].ToString();
        _userId = httpContextAccessor.HttpContext.Request.Headers["User"].ToString();

        DataInitializeAsync().Wait();
    }

    /// <summary>
    /// 新增事件設定檔(含事件標籤設定檔)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>事件設定檔(含事件標籤設定檔)</returns>
    public override async Task<EventDto> CreateAsync(CreateEventDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (!Guid.TryParse(_userId, out var creatorId)) throw new ValidationException("無法取得維護人員資訊");

        var eventEntity = Mapper.Map<Event>(dto);

        eventEntity.Id = SequentialGuidGenerator.Instance.GetGuid();
        eventEntity.CompanyId = companyId;
        eventEntity.TenantId = Payload.TenantId;
        eventEntity.CreatorId = creatorId;

        await _uow.EventRepository.CreateAsync(eventEntity);

        if (dto.EventTags != null)
        {
            var eventTags = dto.EventTags.Select(m => new EventTag
            {
                Id = SequentialGuidGenerator.Instance.GetGuid(),
                TenantId = Payload.TenantId,
                EventId = eventEntity.Id,
                TagId = m.TagId,
                TagOwnerType = m.TagOwnerType,
                Weight = m.Weight,
                CreatorId = creatorId
            });

            foreach (var eventTag in eventTags)
                await _uow.EventTagRepository.CreateAsync(eventTag);
        }

        await _uow.SaveChangeAsync();

        return await GetAsync(eventEntity.Id);
    }

    /// <summary>
    /// 修改事件設定檔(含事件標籤設定檔)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>true/false</returns>
    public override async Task<bool> UpdateAsync(UpdateEventDto dto)
    {
        if (!Guid.TryParse(_userId, out var modifierId)) throw new ValidationException("無法取得維護人員資訊");

        var tagTrackingDtos = await _tagTrackingService.GetByEventIdAsync(dto.Id);

        if (tagTrackingDtos.Count > 0) throw new ValidationException("資料已被使用，不允許修改");

        var eventEntity = Mapper.Map<Event>(dto);

        eventEntity.ModifierId = modifierId;

        _uow.EventRepository.Update(eventEntity);

        var eventTagDtos = await _eventTagService.GetByEventIdAsync(eventEntity.Id);

        if (eventTagDtos.Count > 0)
            foreach (var eventTagDto in eventTagDtos)
                await _uow.EventTagRepository.DeleteAsync(eventTagDto.Id, modifierId);

        if (dto.EventTags == null) return true;

        var eventTags = dto.EventTags.Select(m => new EventTag
        {
            Id = SequentialGuidGenerator.Instance.GetGuid(),
            TenantId = Payload.TenantId,
            EventId = eventEntity.Id,
            TagId = m.TagId,
            TagOwnerType = m.TagOwnerType,
            Weight = m.Weight,
            CreatorId = modifierId
        });

        foreach (var eventTag in eventTags)
            await _uow.EventTagRepository.CreateAsync(eventTag);

        await _uow.SaveChangeAsync();

        return true;
    }

    /// <summary>
    /// 刪除事件設定檔(含事件標籤設定檔)
    /// </summary>
    /// <param name="id">事件識別碼</param>
    /// <returns>true/false</returns>
    public override async Task<bool> DeleteAsync(Guid id)
    {
        if (!Guid.TryParse(_userId, out var deleterId)) throw new ValidationException("無法取得維護人員資訊");

        var tagTrackingDtos = await _tagTrackingService.GetByEventIdAsync(id);

        if (tagTrackingDtos.Count > 0) throw new ValidationException("資料已被使用，不允許刪除");

        await _uow.EventRepository.DeleteAsync(id, deleterId);

        var eventTagDtos = await _eventTagService.GetByEventIdAsync(id);

        if (eventTagDtos.Count > 0)
            foreach (var eventTagDto in eventTagDtos)
                await _uow.EventTagRepository.DeleteAsync(eventTagDto.Id, deleterId);

        await _uow.SaveChangeAsync();

        return true;
    }

    /// <summary>
    /// 明細資料彙整
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>事件設定檔(含事件標籤設定檔)</returns>
    public override async Task<EventDto> SetAdditionPropertiesAsync(EventDto dto)
    {
        if (dto == null) return null;

        dto.EventTags = await _eventTagService.GetByEventIdAsync(dto.Id);

        return dto;
    }

    /// <summary>
    /// Gateway資料初始設定
    /// </summary>
    /// <returns></returns>
    private async Task DataInitializeAsync()
    {
        if (!string.IsNullOrEmpty(_companyId))
        {
            var mappingDto = await _memberService.GetCompanyMappingAsync(_companyId);

            if (mappingDto != null)
                _companyId = mappingDto.CompanyId.ToString();
        }

        if (!string.IsNullOrEmpty(_userId))
        {
            var userDto = await _memberService.GetUserAsync(_userId);

            if (userDto != null)
                _userId = userDto.Id.ToString();
        }
    }
}