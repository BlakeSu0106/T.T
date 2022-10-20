using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Telligent.Core.Application.Services;
using Telligent.Core.Domain.Repositories;
using Telligent.Core.Infrastructure.Generators;
using Telligent.Tag.Application.Dtos;
using Telligent.Tag.Application.Dtos.TagTracking;
using Telligent.Tag.Domain.Shared;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Application.AppServices;

public class
    TagTrackingAppService : CrudAppService<TagTracking, TagTrackingDto, CreateTagTrackingDto, CreateTagTrackingDto>
{
    private readonly BehaviorTagCategoryAppService _behaviorTagCategoryService;
    private readonly ElectronicCommerceAppService _electronicCommerceService;
    private readonly MemberAppService _memberService;
    private readonly UnitOfWork _uow;
    private string _companyId;

    private readonly string _oldCompanyId;
    private string _userId;

    public TagTrackingAppService(
        IRepository<TagTracking> repository,
        IMapper mapper,
        BehaviorTagCategoryAppService behaviorTagCategoryService,
        MemberAppService memberService,
        ElectronicCommerceAppService electronicCommerceService,
        IHttpContextAccessor httpContextAccessor,
        UnitOfWork uow)
        : base(repository, mapper, httpContextAccessor)
    {
        _behaviorTagCategoryService = behaviorTagCategoryService;
        _memberService = memberService;
        _electronicCommerceService = electronicCommerceService;
        _uow = uow;

        if (httpContextAccessor.HttpContext == null) return;

        _companyId = httpContextAccessor.HttpContext.Request.Headers["Company"].ToString();
        _userId = httpContextAccessor.HttpContext.Request.Headers["User"].ToString();
        _oldCompanyId = httpContextAccessor.HttpContext.Request.Headers["Company"].ToString();

        DataInitializeAsync().Wait();
    }

    /// <summary>
    /// 新增標籤追蹤檔
    /// </summary>
    /// <param name="eventId">事件識別碼</param>
    /// <returns>標籤追蹤檔</returns>
    public async Task<IList<TagTrackingDto>> GetByEventIdAsync(Guid eventId)
    {
        return await GetListAsync(m => m.EventId.Equals(eventId) && m.EntityStatus);
    }

    /// <summary>
    /// 新增標籤追蹤檔
    /// </summary>
    /// <param name="tagId">標籤識別碼</param>
    /// <returns>標籤追蹤檔</returns>
    public async Task<IList<TagTrackingDto>> GetByTagIdAsync(Guid tagId,Guid companyId)
    {
        return await GetListAsync(m =>m.CompanyId.Equals(companyId) && m.TagId.Equals(tagId) && m.EntityStatus);
    }

    /// <summary>
    /// 新增標籤追蹤檔
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<IList<TagTrackingDto>> CreateTagTrackingAsync(CreateTagTrackingDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (!Guid.TryParse(_userId, out var creatorId)) throw new ValidationException("無法取得維護人員資訊");

        dto.CompanyId = companyId;
        Payload.MemberId = creatorId;

        var ids = new List<Guid>();

        if (dto.ChannelId.HasValue)
        {
            var channelDto = await _memberService.GetChannelAsync(dto.ChannelId.Value);

            if (channelDto == null) throw new ArgumentException("無法取得渠道資料");
        }

        if (dto.EventId.HasValue)
        {
            var eventDto = await _uow.EventRepository.GetAsync(dto.EventId.Value);

            if (eventDto == null) throw new ArgumentException("無法取得貼標事件資料");
        }

        switch (dto.TagOwnerType)
        {
            case TagOwnerType.Member:
                var memberDto = await _memberService.GetMemberAsync(dto.TagOwnerId);
                if (memberDto == null) throw new ArgumentException("無法取得會員資料");
                break;
            case TagOwnerType.Prospect:
                var prospectDto = await _memberService.GetProspectAsync(dto.TagOwnerId);
                if (prospectDto == null) throw new ArgumentException("無法取得潛客資料");
                break;
            case TagOwnerType.Product:
            case TagOwnerType.Account:
            default:
                throw new ArgumentException("目前僅提供會員及潛客貼標相關邏輯");
        }

        var tagDtos =
            await _uow.TagRepository.GetListAsync(
                m => dto.TagIds.Contains(m.Id) && m.ActivationStatus && m.EntityStatus);

        if (tagDtos == null) throw new ArgumentException("無法取得標籤資料");

        if (!tagDtos.Count.Equals(dto.TagIds.Count)) throw new ArgumentException("無法取得標籤資料");

        foreach (var tagDto in tagDtos)
        {
            // 行為標籤分類下的標籤，需判斷是否僅允許貼一張標籤於同一對象
            if (tagDto.CategoryType.Equals(TagCategoryType.Behavior))
            {
                var behaviorTagCategoryDto =
                    await _behaviorTagCategoryService.GetAsync(m => m.Id.Equals(tagDto.CategoryId) && m.EntityStatus);

                if (behaviorTagCategoryDto.IsUnique.HasValue && behaviorTagCategoryDto.IsUnique.Value)
                {
                    //取得該行為標籤類別下的所有標籤
                    var tags = await _uow.TagRepository.GetListAsync(m =>
                        m.CompanyId.Equals(dto.CompanyId) &&
                        m.CategoryType.Equals(TagCategoryType.Behavior) &&
                        m.CategoryId.Equals(behaviorTagCategoryDto.Id));

                    //取得某一對象該行為標籤類別下的所有標籤貼標紀錄
                    var tagTrackingList = await _uow.TagTrackingRepository.GetListAsync(m =>
                        tags.Select(n => n.Id).Contains(m.TagId) &&
                        m.TagOwnerId.Equals(dto.TagOwnerId) &&
                        m.EntityStatus);

                    foreach (var tagTracking in tagTrackingList)
                        await _uow.TagTrackingRepository.DeleteAsync(tagTracking.Id, Payload.MemberId);
                }
            }

            var id = SequentialGuidGenerator.Instance.GetGuid();

            await _uow.TagTrackingRepository.CreateAsync(new TagTracking
            {
                Id = id,
                TenantId = Payload.TenantId,
                CompanyId = dto.CompanyId,
                ChannelId = dto.ChannelId,
                EventId = dto.EventId,
                TagId = tagDto.Id,
                TagOwnerType = dto.TagOwnerType,
                TagOwnerId = dto.TagOwnerId,
                CreatorId = Payload.MemberId
            });

            ids.Add(id);
        }

        await _uow.SaveChangeAsync();

        return await GetListAsync(m => ids.Contains(m.Id));
    }

    /// <summary>
    /// 新增電商複數標籤追蹤檔 - 會員貼標 (目前用於TE電商)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<bool> CreatePluralMemberElectronicCommerceTagTrackingAsync(CreatePluralElectronicCommerceTagTrackingDto dto)
    {
        return await CreatePluralElectronicCommerceTagTrackingAsync(TagOwnerType.Member, dto);
    }

    /// <summary>
    /// 新增電商複數標籤追蹤檔 - 商品貼標 (目前用於TE電商)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<bool> CreatePluralProductElectronicCommerceTagTrackingAsync(CreatePluralElectronicCommerceTagTrackingDto dto)
    {
        return await CreatePluralElectronicCommerceTagTrackingAsync(TagOwnerType.Product, dto);
    }

    /// <summary>
    /// 新增電商複數標籤貼標
    /// </summary>
    /// <param name="tagOwnerType"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public async Task<bool> CreatePluralElectronicCommerceTagTrackingAsync(TagOwnerType tagOwnerType, CreatePluralElectronicCommerceTagTrackingDto dto)
    {

        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (!Guid.TryParse(_userId, out var creatorId)) throw new ValidationException("無法取得維護人員資訊");

        dto.CompanyId = companyId;
        Payload.MemberId = creatorId;

        var channelDto = await _memberService.GetChannelAsync(dto.ChannelId.Value);
        if (channelDto == null) throw new ArgumentException("無法取得渠道資料");

        // 檢查會員或是商品資料是否存在
        switch (tagOwnerType)
        {
            case TagOwnerType.Member:

                var eventIdList = dto.TagTracking.Select(s => s.EventId).Distinct().ToList();
                var eventDtos =
                    await _uow.SystemEventRepository.GetListAsync(
                        m => eventIdList.Contains(m.Id) && m.ActivationStatus && m.EntityStatus);

                if (eventDtos == null) throw new ArgumentException("無法取得貼標事件資料");
                if (!eventDtos.Count.Equals(eventIdList.Count)) throw new ArgumentException("無法取得貼標事件資料");

                var memberList = dto.TagTracking.Select(member => member.TagOwnerId).Distinct().ToList();
                var memberDtos = await _memberService.GetMemberListAsync(memberList);
                if (!memberDtos.Any()) throw new ArgumentException("無法取得會員資料");
                if (!memberDtos.Count.Equals(memberList.Count)) throw new ArgumentException("無法取得會員資料");
                break;

            case TagOwnerType.Product:
                var productList = dto.TagTracking.Select(product => product.TagOwnerId).Distinct().ToList();
                var productDtos = await _electronicCommerceService.GetProductListAsync(productList, _oldCompanyId);
                if (!productDtos.Any()) throw new ArgumentException("無法取得商品資料");
                if (!productDtos.Count.Equals(productList.Count)) throw new ArgumentException("無法取得商品資料");
                break;
        }

        var tagList = dto.TagTracking.SelectMany(s => s.TagIds).Distinct().ToList();

        var tagDtoList =
                await _uow.TagRepository.GetListAsync(
                    m => tagList.Contains(m.Id) && m.ActivationStatus && m.EntityStatus);
        if (!tagDtoList.Any()) throw new ArgumentException("無法取得標籤資料");
        if (!tagDtoList.Count.Equals(tagList.Count)) throw new ArgumentException("無法取得標籤資料");

        foreach (var tagTrackingDto in dto.TagTracking)
        {
            var tagDtos =
                await _uow.TagRepository.GetListAsync(
                    m => tagTrackingDto.TagIds.Contains(m.Id) && m.ActivationStatus && m.EntityStatus);

            foreach (var tagDto in tagDtos)
            {
                // 行為標籤分類下的標籤，需判斷是否僅允許貼一張標籤於同一對象
                if (tagDto.CategoryType.Equals(TagCategoryType.Behavior))
                {
                    var behaviorTagCategoryDto =
                        await _behaviorTagCategoryService.GetAsync(m => m.Id.Equals(tagDto.CategoryId) && m.EntityStatus);

                    if (behaviorTagCategoryDto.IsUnique.HasValue && behaviorTagCategoryDto.IsUnique.Value)
                    {
                        //取得該行為標籤類別下的所有標籤
                        var tags = await _uow.TagRepository.GetListAsync(m =>
                            m.CompanyId.Equals(tagTrackingDto.CompanyId) &&
                            m.CategoryType.Equals(TagCategoryType.Behavior) &&
                            m.CategoryId.Equals(behaviorTagCategoryDto.Id));

                        //取得某一對象該行為標籤類別下的所有標籤貼標紀錄
                        var tagTrackingList = await _uow.TagTrackingRepository.GetListAsync(m =>
                            tags.Select(n => n.Id).Contains(m.TagId) &&
                            m.TagOwnerId.Equals(tagTrackingDto.TagOwnerId) &&
                            m.EntityStatus);

                        foreach (var tagTracking in tagTrackingList)
                            await _uow.TagTrackingRepository.DeleteAsync(tagTracking.Id, Payload.MemberId);
                    }
                }

                var id = SequentialGuidGenerator.Instance.GetGuid();

                await _uow.TagTrackingRepository.CreateAsync(new TagTracking
                {
                    Id = id,
                    TenantId = Payload.TenantId,
                    CompanyId = dto.CompanyId,
                    ChannelId = dto.ChannelId,
                    EventId = tagTrackingDto.EventId.HasValue ? tagTrackingDto.EventId : null,
                    TagId = tagDto.Id,
                    TagOwnerType = tagOwnerType,
                    TagOwnerId = tagTrackingDto.TagOwnerId,
                    CreatorId = Payload.MemberId
                });
            }
        }

        await _uow.SaveChangeAsync();

        return true;

    }

    /// <summary>
    /// 批量新增標籤追蹤檔
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<bool> CreateBatchStickTagAsync(BatchStickTagDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (!Guid.TryParse(_userId, out var creatorId)) throw new ValidationException("無法取得維護人員資訊");

        dto.CompanyId = companyId;
        Payload.MemberId = creatorId;

        var successfulCount = 0;

        if (dto.ChannelId.HasValue)
        {
            var channelDto = await _memberService.GetChannelAsync(dto.ChannelId.Value);

            if (channelDto == null) throw new ArgumentException("無法取得渠道資料");
        }

        var memberDtos = await _memberService.GetMemberListAsync(dto.MemberIds);

        if (memberDtos == null) throw new ArgumentException("無法取得會員資料");

        if (!memberDtos.Count.Equals(dto.MemberIds.Count)) throw new ArgumentException("無法取得會員資料");

        var prospectDtos = await _memberService.GetProspectListAsync(dto.ProspectIds);

        if (prospectDtos == null) throw new ArgumentException("無法取得潛客資料");

        if (!prospectDtos.Count.Equals(dto.ProspectIds.Count)) throw new ArgumentException("無法取得潛客資料");

        var tagDtos =
            await _uow.TagRepository.GetListAsync(
                m => dto.TagIds.Contains(m.Id) && m.ActivationStatus && m.EntityStatus);

        if (tagDtos == null) throw new ArgumentException("無法取得標籤資料");

        if (!tagDtos.Count.Equals(dto.TagIds.Count)) throw new ArgumentException("無法取得標籤資料");

        var transactionCount = dto.MemberIds.Count + dto.ProspectIds.Count;

        foreach (var memberId in dto.MemberIds)
        {
            foreach (var tagDto in tagDtos)
            {
                // 行為標籤分類下的標籤，需判斷是否僅允許貼一張標籤於同一對象
                if (tagDto.CategoryType.Equals(TagCategoryType.Behavior))
                {
                    var behaviorTagCategoryDto =
                        await _behaviorTagCategoryService.GetAsync(
                            m => m.Id.Equals(tagDto.CategoryId) && m.EntityStatus);

                    if (behaviorTagCategoryDto.IsUnique.HasValue && behaviorTagCategoryDto.IsUnique.Value)
                    {
                        //取得該行為標籤類別下的所有標籤
                        var tags = await _uow.TagRepository.GetListAsync(m =>
                            m.CompanyId.Equals(dto.CompanyId) &&
                            m.CategoryType.Equals(TagCategoryType.Behavior) &&
                            m.CategoryId.Equals(behaviorTagCategoryDto.Id));

                        //取得某一對象該行為標籤類別下的所有標籤貼標紀錄
                        var tagTrackingList = await _uow.TagTrackingRepository.GetListAsync(m =>
                            tags.Select(n => n.Id).Contains(m.TagId) &&
                            m.TagOwnerId.Equals(memberId) &&
                            m.EntityStatus);

                        foreach (var tagTracking in tagTrackingList)
                            await _uow.TagTrackingRepository.DeleteAsync(tagTracking.Id, Payload.MemberId);
                    }
                }

                await _uow.TagTrackingRepository.CreateAsync(new TagTracking
                {
                    Id = SequentialGuidGenerator.Instance.GetGuid(),
                    TenantId = Payload.TenantId,
                    CompanyId = dto.CompanyId,
                    ChannelId = dto.ChannelId,
                    TagId = tagDto.Id,
                    TagOwnerType = TagOwnerType.Member,
                    TagOwnerId = memberId,
                    CreatorId = Payload.MemberId
                });
            }

            if (await _uow.SaveChangeAsync() > 0) successfulCount++;
        }

        foreach (var prospectId in dto.ProspectIds)
        {
            foreach (var tagDto in tagDtos)
            {
                // 行為標籤分類下的標籤，需判斷是否僅允許貼一張標籤於同一對象
                if (tagDto.CategoryType.Equals(TagCategoryType.Behavior))
                {
                    var behaviorTagCategoryDto =
                        await _behaviorTagCategoryService.GetAsync(
                            m => m.Id.Equals(tagDto.CategoryId) && m.EntityStatus);

                    if (behaviorTagCategoryDto.IsUnique.HasValue && behaviorTagCategoryDto.IsUnique.Value)
                    {
                        //取得該行為標籤類別下的所有標籤
                        var tags = await _uow.TagRepository.GetListAsync(m =>
                            m.CompanyId.Equals(dto.CompanyId) &&
                            m.CategoryType.Equals(TagCategoryType.Behavior) &&
                            m.CategoryId.Equals(behaviorTagCategoryDto.Id));

                        //取得某一對象該行為標籤類別下的所有標籤貼標紀錄
                        var tagTrackingList = await _uow.TagTrackingRepository.GetListAsync(m =>
                            tags.Select(n => n.Id).Contains(m.TagId) &&
                            m.TagOwnerId.Equals(prospectId) &&
                            m.EntityStatus);

                        foreach (var tagTracking in tagTrackingList)
                            await _uow.TagTrackingRepository.DeleteAsync(tagTracking.Id, Payload.MemberId);
                    }
                }

                await _uow.TagTrackingRepository.CreateAsync(new TagTracking
                {
                    Id = SequentialGuidGenerator.Instance.GetGuid(),
                    TenantId = Payload.TenantId,
                    CompanyId = dto.CompanyId,
                    ChannelId = dto.ChannelId,
                    TagId = tagDto.Id,
                    TagOwnerType = TagOwnerType.Prospect,
                    TagOwnerId = prospectId,
                    CreatorId = Payload.MemberId
                });
            }

            if (await _uow.SaveChangeAsync() > 0) successfulCount++;
        }

        await _uow.BatchTransactionLogRepository.CreateAsync(new BatchTransactionLog
        {
            Id = SequentialGuidGenerator.Instance.GetGuid(),
            TenantId = Payload.TenantId,
            CompanyId = dto.CompanyId,
            CommandType = CommandType.Create,
            TransactionCount = transactionCount,
            SuccessfulCount = successfulCount,
            FailureCount = transactionCount - successfulCount,
            CreatorId = Payload.MemberId
        });

        await _uow.SaveChangeAsync();

        return true;
    }

    /// <summary>
    /// 刪除新增標籤追蹤檔
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<bool> DeleteBatchStickTagAsync(BatchStickTagDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (!Guid.TryParse(_userId, out var deleterId)) throw new ValidationException("無法取得維護人員資訊");

        dto.CompanyId = companyId;
        Payload.MemberId = deleterId;

        var successfulCount = 0;

        if (dto.ChannelId.HasValue)
        {
            var channelDto = await _memberService.GetChannelAsync(dto.ChannelId.Value);

            if (channelDto == null) throw new ArgumentException("無法取得渠道資料");
        }

        var memberDtos = await _memberService.GetMemberListAsync(dto.MemberIds);

        if (memberDtos == null) throw new ArgumentException("無法取得會員資料");

        if (!memberDtos.Count.Equals(dto.MemberIds.Count)) throw new ArgumentException("無法取得會員資料");

        var prospectDtos = await _memberService.GetProspectListAsync(dto.ProspectIds);

        if (prospectDtos == null) throw new ArgumentException("無法取得潛客資料");

        if (!prospectDtos.Count.Equals(dto.ProspectIds.Count)) throw new ArgumentException("無法取得潛客資料");

        var tagDtos = await _uow.TagRepository.GetListAsync(m =>
            dto.TagIds.Contains(m.Id) &&
            m.ActivationStatus &&
            m.EntityStatus);

        if (tagDtos == null) throw new ArgumentException("無法取得標籤資料");

        if (!tagDtos.Count.Equals(dto.TagIds.Count)) throw new ArgumentException("無法取得標籤資料");

        var transactionCount = dto.MemberIds.Count + dto.ProspectIds.Count;

        foreach (var memberId in dto.MemberIds)
        {
            foreach (var tagId in dto.TagIds)
            {
                var tagTrackingDtos = await GetListAsync(m =>
                    m.TagId.Equals(tagId) &&
                    m.TagOwnerType.Equals(TagOwnerType.Member) &&
                    m.TagOwnerId.Equals(memberId) &&
                    m.EntityStatus);

                if (tagTrackingDtos == null) continue;

                foreach (var tagTrackingDto in tagTrackingDtos)
                    await _uow.TagTrackingRepository.DeleteAsync(tagTrackingDto.Id, Payload.MemberId);
            }

            if (await _uow.SaveChangeAsync() > 0) successfulCount++;
        }

        foreach (var prospectId in dto.ProspectIds)
        {
            foreach (var tagId in dto.TagIds)
            {
                var tagTrackingDtos = await GetListAsync(m =>
                    m.TagId.Equals(tagId) &&
                    m.TagOwnerType.Equals(TagOwnerType.Prospect) &&
                    m.TagOwnerId.Equals(prospectId) &&
                    m.EntityStatus);

                if (tagTrackingDtos == null) continue;

                foreach (var tagTrackingDto in tagTrackingDtos)
                    await _uow.TagTrackingRepository.DeleteAsync(tagTrackingDto.Id, Payload.MemberId);
            }

            if (await _uow.SaveChangeAsync() > 0) successfulCount++;
        }

        await _uow.BatchTransactionLogRepository.CreateAsync(new BatchTransactionLog
        {
            Id = SequentialGuidGenerator.Instance.GetGuid(),
            TenantId = Payload.TenantId,
            CompanyId = dto.CompanyId,
            CommandType = CommandType.Delete,
            TransactionCount = transactionCount,
            SuccessfulCount = successfulCount,
            FailureCount = transactionCount - successfulCount,
            CreatorId = Payload.MemberId
        });

        await _uow.SaveChangeAsync();

        return true;
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
