using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Telligent.Core.Application.Services;
using Telligent.Core.Domain.Repositories;
using Telligent.Core.Infrastructure.Generators;
using Telligent.Tag.Application.Dtos.PoolTag;
using Telligent.Tag.Application.Dtos.Tag;
using Telligent.Tag.Domain.Shared;
using TagDto = Telligent.Tag.Application.Dtos.Tag.TagDto;

namespace Telligent.Tag.Application.AppServices;

public class TagAppService : CrudAppService<Domain.Tags.Tag, TagDto, CreateTagDto, UpdateTagDto>
{
    private readonly BehaviorTagCategoryAppService _behaviorTagCategoryAppServiceService;
    private readonly EventTagAppService _eventTagService;
    private readonly MemberAppService _memberService;
    private readonly TagCategoryPermissionAppService _tagCategoryPermissionAppService;
    private readonly TagTrackingAppService _tagTrackingService;
    private readonly UnitOfWork _uow;

    private string _companyId;
    private string _userId;

    public TagAppService(
        IRepository<Domain.Tags.Tag> repository,
        IMapper mapper,
        BehaviorTagCategoryAppService behaviorTagCategoryAppServiceService,
        TagCategoryPermissionAppService tagCategoryPermissionAppService,
        EventTagAppService eventTagService,
        MemberAppService memberService,
        TagTrackingAppService tagTrackingService,
        IHttpContextAccessor httpContextAccessor,
        UnitOfWork uow)
        : base(repository, mapper, httpContextAccessor)
    {
        _behaviorTagCategoryAppServiceService = behaviorTagCategoryAppServiceService;
        _tagCategoryPermissionAppService = tagCategoryPermissionAppService;
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
    /// 取得標籤
    /// </summary>
    /// <returns>標籤</returns>
    public async Task<IList<TagDto>> GetAsync()
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        return await GetListAsync(m => m.CompanyId.Equals(companyId) && m.EntityStatus);
    }

    /// <summary>
    /// 取得標籤
    /// </summary>
    /// <param name="categoryId">標籤類別識別碼</param>
    /// <returns>標籤</returns>
    public async Task<IList<TagDto>> GetByCategoryIdAsync(Guid categoryId)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (categoryId == Guid.Empty) throw new ValidationException("格式錯誤");
        return await GetByCategoryIdAsync(companyId,categoryId);
    }
    public async Task<IList<TagDto>> GetByCategoryIdAsync(Guid companyId, Guid categoryId)
    {
 
        return await GetListAsync(m => m.CompanyId.Equals(companyId) && m.CategoryId.Equals(categoryId) && m.EntityStatus && m.ActivationStatus);
    }
    /// <summary>
    /// 取得標籤依照分類頁碼
    /// </summary>
    /// <param name="categoryId">標籤類別識別碼</param>
    /// <param name="limit">顯示數量</param>
    /// <param name="offset">起始數</param>
    /// <returns></returns>
    public async Task<IList<TagSummaryDto>> GetSummaryAsync(Guid categoryId, int limit, int offset)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (categoryId == Guid.Empty) throw new ValidationException("格式錯誤");
        if (limit <= 0 || offset <-1) throw new ValidationException("格式錯誤");

        var tagSummaryDtos = new List<TagSummaryDto>();
        var tags = await _uow.TagRepository.GetListAsync(m =>
            m.CategoryId.Equals(categoryId) && m.CompanyId.Equals(companyId) &&
            m.EntityStatus);

        foreach (var tag in tags.Skip(offset).Take(limit).OrderByDescending(m => m.CreationTime))
        {
            var userDto = tag.CreatorId.HasValue ? await _memberService.GetUserAsync(tag.CreatorId.Value.ToString()) : null;
            var tagTrackingDtos = await _tagTrackingService.GetByTagIdAsync(tag.Id,companyId);

            tagSummaryDtos.Add(new TagSummaryDto
            {
                Id = tag.Id,
                Name = tag.Name,
                EventBinding = tagTrackingDtos.Count > 0,
                UsedQuantity = tagTrackingDtos.Count,
                CreatorName = userDto?.Name,
                CreationTime = tag.CreationTime
            });
        }

        return tagSummaryDtos;
    }

    /// <summary>
    /// 取得標籤依照分類關鍵字
    /// </summary>
    /// <param name="categoryId">標籤類別識別碼</param>    
    /// <param name="keyword">關鍵字</param>
    /// <returns></returns>
    public async Task<IList<TagSummaryDto>> GetCategoryWithKeywordAsync(Guid categoryId, string keyword)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (categoryId == Guid.Empty) throw new ValidationException("格式錯誤");
        if (keyword.IsNullOrEmpty()) throw new ValidationException("格式錯誤");

        var tagSummaryDtos = new List<TagSummaryDto>();
        var tags = await _uow.TagRepository.GetListAsync(m =>
                m.CategoryId.Equals(categoryId) && 
                m.EntityStatus && m.CompanyId.Equals(companyId) && m.Name.Contains(keyword));

        foreach (var tag in tags)
        {
            var userDto = tag.CreatorId.HasValue
                ? await _memberService.GetUserAsync(tag.CreatorId.Value.ToString())
                : null;
            var tagTrackingDtos = await _tagTrackingService.GetByTagIdAsync(tag.Id, companyId);
            tagSummaryDtos.Add(new TagSummaryDto
            {
                Id = tag.Id,
                Name = tag.Name,
                EventBinding = tagTrackingDtos.Count > 0,
                UsedQuantity = tagTrackingDtos.Count,
                CreatorName = userDto?.Name,
                CreationTime = tag.CreationTime
            });
        }
        return tagSummaryDtos;
    }

    /// <summary>
    /// 取得標籤
    /// </summary>
    /// <param name="memberId">會員</param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public async Task<IList<TagDto>> GetByMemberIdAsync(Guid memberId)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        var tagTrackingDtos = await _tagTrackingService.GetListAsync(m =>
            m.CompanyId.Equals(companyId) &&
            m.TagOwnerType.Equals(TagOwnerType.Member) &&
            m.TagOwnerId.Equals(memberId) &&
            m.EntityStatus);

        return await GetListAsync(m =>
            tagTrackingDtos.Select(x => x.TagId).Contains(m.Id));
    }

    /// <summary>
    /// 取得標籤池統計資訊(會員)
    /// </summary>
    /// <param name="dto">查詢條件</param>
    /// <returns></returns>
    public async Task<IList<PoolTagDto>> GetPoolTagByMemberAsync(QueryPoolTagDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        return await GetPoolTagAsync(companyId, TagOwnerType.Member, dto);
    }

    /// <summary>
    /// 取得標籤池統計資訊(潛客)
    /// </summary>
    /// <param name="dto">查詢條件</param>
    /// <returns></returns>
    public async Task<IList<PoolTagDto>> GetPoolTagByProspectAsync(QueryPoolTagDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        return await GetPoolTagAsync(companyId, TagOwnerType.Prospect, dto);
    }

    /// <summary>
    /// 取得標籤池統計資訊(會員)
    /// </summary>
    /// <param name="dto">查詢條件</param>
    /// <returns></returns>
    public async Task<IList<PoolCategoryTagDto>> GetCategoryTagByMemberAsync(QueryPoolTagDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        return await GetPoolTagByCategoryAsync(companyId, TagOwnerType.Member, dto);
    }

    /// <summary>
    /// 取得標籤池統計資訊(潛客)
    /// </summary>
    /// <param name="dto">查詢條件</param>
    /// <returns></returns>
    public async Task<IList<PoolCategoryTagDto>> GetCategoryTagByProspectAsync(QueryPoolTagDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        return await GetPoolTagByCategoryAsync(companyId, TagOwnerType.Prospect, dto);
    }

    /// <summary>
    /// 取得標籤池統計資訊
    /// </summary>
    /// <param name="companyId">公司識別碼</param>
    /// <param name="ownerType">標籤所有對象類別</param>
    /// <param name="dto">查詢條件</param>
    /// <returns></returns>
    private async Task<IList<PoolTagDto>> GetPoolTagAsync(Guid companyId, TagOwnerType ownerType, QueryPoolTagDto dto)
    {
        var poolTagDtos = new List<PoolTagDto>();

        var tagTracking = await _uow.TagTrackingRepository.GetListAsync(m =>
            m.CompanyId.Equals(companyId) &&
            m.TagOwnerType.Equals(ownerType) &&
            m.TagOwnerId.Equals(dto.TagOwnerId) &&
            m.EntityStatus);

        var targetData = tagTracking;

        if (tagTracking == null) return null;

        if (dto.StartDate.HasValue)
            targetData = targetData.Where(m =>
                    m.CreationTime.HasValue &&
                    DateTime.Compare(dto.StartDate.Value.Date, m.CreationTime.Value.Date) <= 0)
                .ToList();

        if (dto.EndDate.HasValue)
            targetData = targetData.Where(m =>
                    m.CreationTime.HasValue && DateTime.Compare(dto.EndDate.Value.Date, m.CreationTime.Value.Date) >= 0)
                .ToList();

        var tagIds = targetData.Select(m => m.TagId).Distinct();

        foreach (var tagId in tagIds)
        {
            var tag = await _uow.TagRepository.GetAsync(tagId);

            if (!dto.TagCategoryIds.Contains(tag.CategoryId)) continue;

            if (!string.IsNullOrEmpty(dto.Name))
                if (!dto.Name.Contains(tag.Name))
                    continue;

            var quantity = targetData.Count(m => m.TagId.Equals(tagId));
            var totalQuantity = tagTracking.Count(m => m.TagId.Equals(tagId));
            var lastTime = tagTracking.Where(m => m.TagId.Equals(tagId) && m.CreationTime.HasValue)
                .MaxBy(m => m.CreationTime.Value)?
                .CreationTime;

            poolTagDtos.Add(new PoolTagDto
            {
                TagCategoryType = tag.CategoryType,
                TagCategoryId = tag.CategoryId,
                TagId = tag.Id,
                Name = tag.Name,
                Quantity = quantity,
                TotalQuantity = totalQuantity,
                LastTime = lastTime
            });
        }

        return poolTagDtos.OrderByDescending(m => m.Quantity).ThenByDescending(m => m.LastTime).ToList();
    }

    private async Task<IList<PoolCategoryTagDto>> GetPoolTagByCategoryAsync(Guid companyId, TagOwnerType ownerType,
        QueryPoolTagDto dto)
    {
        var poolCategoryTagDtos = new List<PoolCategoryTagDto>();

        var tagTracking = await _uow.TagTrackingRepository.GetListAsync(m =>
            m.CompanyId.Equals(companyId) &&
            m.TagOwnerType.Equals(ownerType) &&
            m.TagOwnerId.Equals(dto.TagOwnerId) &&
            m.EntityStatus);

        var targetData = tagTracking;

        if (tagTracking == null) return null;

        if (dto.StartDate.HasValue)
            targetData = targetData.Where(m =>
                    m.CreationTime.HasValue &&
                    DateTime.Compare(dto.StartDate.Value.Date, m.CreationTime.Value.Date) <= 0)
                .ToList();

        if (dto.EndDate.HasValue)
            targetData = targetData.Where(m =>
                    m.CreationTime.HasValue && DateTime.Compare(dto.EndDate.Value.Date, m.CreationTime.Value.Date) >= 0)
                .ToList();

        var tagIds = targetData.OrderByDescending(m => m.CreationTime).Select(m => m.TagId).Distinct();

        foreach (var dtoTagCategoryId in dto.TagCategoryIds)
        {
            var name = string.Empty;
            var tagCategoryPermission = await _tagCategoryPermissionAppService.GetByCategoryIdAsync(dtoTagCategoryId);

            switch (tagCategoryPermission.CategoryType)
            {
                case TagCategoryType.Behavior:
                    var behaviorTagCategory = await _uow.BehaviorTagCategoryRepository.GetAsync(dtoTagCategoryId);
                    name = behaviorTagCategory.Name;
                    break;
                case TagCategoryType.Customization:
                    var customizationTagCategory =
                        await _uow.CustomizationTagCategoryRepository.GetAsync(dtoTagCategoryId);
                    name = customizationTagCategory.Name;
                    break;
            }

            var tags = new List<Dtos.PoolTag.TagDto>();

            var tagQuality = 0;
            // 1. 這個分類下有多少標籤
            //    tagCategoryId => GetTags
            //    tagIds => 貼標的標籤，無關分類
            // 2. 這個分類下的標籤，有哪些被貼在這個人身上
            foreach (var tagid in tagIds)
            {
                var tag = new Domain.Tags.Tag();

                if (!string.IsNullOrEmpty(dto.Name))
                    tag = await Repository.GetAsync(t =>
                        t.Id.Equals(tagid) &&
                        t.EntityStatus && t.Name.Contains(dto.Name) && t.CategoryId.Equals(dtoTagCategoryId));
                else
                    tag = await Repository.GetAsync(t =>
                        t.Id.Equals(tagid) &&
                        t.EntityStatus && t.CategoryId.Equals(dtoTagCategoryId));
                if (tag != null)
                {
                    tagQuality = tagQuality + targetData.Count(tracking => tracking.TagId.Equals(tagid));
                    tags.Add(new Dtos.PoolTag.TagDto { Name = tag.Name, Id = tag.Id });
                }
            }

            poolCategoryTagDtos.Add(new PoolCategoryTagDto
            {
                TagCategoryType = tagCategoryPermission.CategoryType,
                TagCategoryId = dtoTagCategoryId,
                TagCategoryName = name,
                Tags = tags,
                Quantity = tagQuality
            });
        }

        return poolCategoryTagDtos.OrderByDescending(m => m.Quantity).ToList();
    }

    /// <summary>
    /// 取得標籤
    /// </summary>
    /// <param name="prospectId">潛客</param>
    /// <returns>標籤</returns>
    /// <exception cref="ValidationException"></exception>
    public async Task<IList<TagDto>> GetByProspectIdAsync(Guid prospectId)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        var tagTrackingDtos = await _tagTrackingService.GetListAsync(m =>
            m.CompanyId.Equals(companyId) &&
            m.TagOwnerType.Equals(TagOwnerType.Prospect) &&
            m.TagOwnerId.Equals(prospectId) &&
            m.EntityStatus);

        return await GetListAsync(m =>
            tagTrackingDtos.Select(x => x.TagId).Contains(m.Id));
    }

    /// <summary>
    /// 依標籤名稱取回標籤
    /// </summary>
    /// <param name="dto">Query</param>
    /// <returns>標籤</returns>
    /// <exception cref="ValidationException"></exception>
    public async Task<TagDto> GetByNameAsync(TagDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        //存在相同名稱的標籤，檢查啟用狀態
        var tagDto = await GetAsync(m =>
            m.CompanyId.Equals(companyId) &&
            m.CategoryType.Equals(dto.CategoryType) &&
            m.CategoryId.Equals(dto.CategoryId) &&
            m.Name.Equals(dto.Name) &&
            m.ActivationStatus &&
            m.EntityStatus);

        if (tagDto == null) throw new ValidationException("標籤名稱不存在");

        return tagDto;
    }

    /// <summary>
    /// 檢查標籤是否存在
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>true/false</returns>
    /// <exception cref="ValidationException"></exception>
    public async Task<bool> CheckDataExistAsync(CreateTagDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        //存在相同名稱的標籤，檢查啟用狀態
        var tagDtos = await GetListAsync(m =>
            m.CompanyId.Equals(companyId) &&
            m.CategoryType.Equals(dto.CategoryType) &&
            m.CategoryId.Equals(dto.CategoryId) &&
            m.Name.Equals(dto.Name) &&
            m.ActivationStatus &&
            m.EntityStatus);

        if (tagDtos.Count > 0) throw new ValidationException("標籤名稱重複");

        return true;
    }

    /// <summary>
    /// 新增/異動標籤
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>標籤</returns>
    /// <exception cref="ValidationException"></exception>
    public async Task<IList<TagDto>> CreateAsync(CreateMultiTagsDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (!Guid.TryParse(_userId, out var creatorId)) throw new ValidationException("無法取得維護人員資訊");

        Payload.MemberId = creatorId;

        var ids = new List<Guid>();

        foreach (var createTagDto in dto.Tags)
        {
            createTagDto.CompanyId = companyId;

            switch (createTagDto.CategoryType)
            {
                case TagCategoryType.Behavior:
                    {
                        var behaviorTagCategoryDto =
                            await _behaviorTagCategoryAppServiceService.GetAsync(createTagDto.CategoryId);
                        if (behaviorTagCategoryDto == null) throw new ValidationException("標籤類別不存在");
                        break;
                    }
                case TagCategoryType.Customization:
                    {
                        var customizationTagCategoryDto =
                            await _uow.CustomizationTagCategoryRepository.GetAsync(createTagDto.CategoryId);
                        if (customizationTagCategoryDto == null) throw new ValidationException("標籤類別不存在");
                        break;
                    }
                default:
                    throw new ValidationException("標籤名稱重複");
            }

            var tag = await Repository.GetAsync(t =>
                t.CompanyId.Equals(createTagDto.CompanyId) &&
                t.CategoryType.Equals(createTagDto.CategoryType) &&
                t.CategoryId.Equals(createTagDto.CategoryId) &&
                t.Name.Equals(createTagDto.Name) &&
                t.EntityStatus);

            if (tag != null)
            {
                if (tag.ActivationStatus) throw new ValidationException("標籤名稱重複");

                tag.ActivationStatus = true;
                tag.ModifierId = Payload.MemberId;

                _uow.TagRepository.Update(tag);
            }
            else
            {
                tag = Mapper.Map<Domain.Tags.Tag>(createTagDto);

                tag.Id = SequentialGuidGenerator.Instance.GetGuid();
                tag.ActivationStatus = true;
                tag.IsSync = tag.CategoryType.Equals(TagCategoryType.Behavior);
                tag.TenantId = Payload.TenantId;
                tag.CreatorId = Payload.MemberId;

                await _uow.TagRepository.CreateAsync(tag);
            }

            ids.Add(tag.Id);
        }

        await _uow.SaveChangeAsync();

        return await GetListAsync(m => ids.Contains(m.Id));
    }

    /// <summary>
    /// 修改標籤
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public override async Task<bool> UpdateAsync(UpdateTagDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (!Guid.TryParse(_userId, out var modifierId)) throw new ValidationException("無法取得維護人員資訊");

        Payload.MemberId = modifierId;

        var tag = await Repository.GetAsync(dto.Id);

        if (tag.CategoryType.Equals(TagCategoryType.Behavior)) throw new ValidationException("目前僅開放調整自訂標籤類別下的標籤");

        var eventTagDtos = await _eventTagService.GetByTagIdAsync(dto.Id);
        var tagTrackingDtos = await _tagTrackingService.GetByTagIdAsync(dto.Id, companyId);

        if (eventTagDtos.Count > 0) throw new ValidationException("該標籤已被使用，不允許修改");
        if (tagTrackingDtos.Count > 0) throw new ValidationException("該標籤已被使用，不允許修改");

        return await base.UpdateAsync(dto);
    }

    /// <summary>
    /// 刪除標籤
    /// </summary>
    /// <param name="id">標籤識別碼</param>
    /// <returns>true/false</returns>
    /// <exception cref="ValidationException"></exception>
    public override async Task<bool> DeleteAsync(Guid id)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (!Guid.TryParse(_userId, out var deleterId)) throw new ValidationException("無法取得維護人員資訊");

        Payload.MemberId = deleterId;

        var eventTagDtos = await _eventTagService.GetByTagIdAsync(id);
        var tagTrackingDtos = await _tagTrackingService.GetByTagIdAsync(id, companyId);

        if (eventTagDtos.Count > 0) throw new ValidationException("該標籤已被使用，不允許刪除");
        if (tagTrackingDtos.Count > 0) throw new ValidationException("該標籤已被使用，不允許刪除");

        return await base.DeleteAsync(id);
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