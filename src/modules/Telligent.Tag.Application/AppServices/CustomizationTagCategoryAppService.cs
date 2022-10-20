using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Telligent.Core.Application.Services;
using Telligent.Core.Domain.Repositories;
using Telligent.Core.Infrastructure.Generators;
using Telligent.Tag.Application.Dtos.CustomizationTagCategory;
using Telligent.Tag.Domain.Shared;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Application.AppServices;

public class CustomizationTagCategoryAppService : CrudAppService<CustomizationTagCategory, CustomizationTagCategoryDto,
    CreateCustomizationTagCategoryDto, UpdateCustomizationTagCategoryDto>
{
    private readonly MemberAppService _memberService;
    private readonly TagCategoryPermissionAppService _tagCategoryPermissionService;
    private readonly TagAppService _tagService;
    private readonly UnitOfWork _uow;

    private string _companyId;
    private string _userId;

    public CustomizationTagCategoryAppService(
        IRepository<CustomizationTagCategory> repository,
        IMapper mapper,
        MemberAppService memberService,
        TagAppService tagService,
        TagCategoryPermissionAppService tagCategoryPermissionService,
        IHttpContextAccessor httpContextAccessor,
        UnitOfWork uow)
        : base(repository, mapper, httpContextAccessor)
    {
        _memberService = memberService;
        _tagService = tagService;
        _tagCategoryPermissionService = tagCategoryPermissionService;
        _uow = uow;

        if (httpContextAccessor.HttpContext == null) return;

        _companyId = httpContextAccessor.HttpContext.Request.Headers["Company"].ToString();
        _userId = httpContextAccessor.HttpContext.Request.Headers["User"].ToString();

        DataInitializeAsync().Wait();
    }

    /// <summary>
    /// 取得啟用的自訂標籤類別
    /// </summary>
    /// <returns>自訂標籤類別</returns>
    public async Task<IList<CustomizationTagCategoryDto>> GetActivatedAsync()
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        var tagCategoryPermissionDtos =
            await _tagCategoryPermissionService.GetActivatedAsync(companyId, TagCategoryType.Customization);

        if (!tagCategoryPermissionDtos.Any()) return null;

        return await GetListAsync(m =>
            tagCategoryPermissionDtos.Select(p => p.CategoryId).Contains(m.Id) &&
            m.CompanyId.Equals(companyId) && m.ActivationStatus && m.EntityStatus);
    }

    /// <summary>
    /// 取得自訂標籤類別(含啟用/停用)
    /// </summary>
    /// <returns>自訂標籤類別</returns>
    public override async Task<IList<CustomizationTagCategoryDto>> GetAllAsync()
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        return await GetListAsync(m => m.CompanyId.Equals(companyId) && m.EntityStatus);
    }

    /// <summary>
    /// 取得啟用的行為標籤類別名稱
    /// </summary>
    /// <returns>行為標籤類別名稱</returns>
    public async Task<CustomizationTagCategoryDto> GetCustomizationTagCategoryNameAsync(Guid categoryId)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        var tagCategoryPermissionDtos =
            await _tagCategoryPermissionService.GetActivatedAsync(companyId, TagCategoryType.Behavior);

        if (!tagCategoryPermissionDtos.Any()) return null;

        return await GetAsync(m =>
            m.Id.Equals(categoryId) && m.EntityStatus);
    }

    /// <summary>
    /// 新增自訂標籤類別
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>自訂標籤類別</returns>
    public override async Task<CustomizationTagCategoryDto> CreateAsync(CreateCustomizationTagCategoryDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (!Guid.TryParse(_userId, out var creatorId)) throw new ValidationException("無法取得維護人員資訊");

        Payload.MemberId = creatorId;

        var customizationTagCategoryDtos = await GetListAsync(m =>
            m.Name.Equals(dto.Name) && m.CompanyId.Equals(companyId) && m.EntityStatus);

        if (customizationTagCategoryDtos.Count > 0) throw new ValidationException("資料已存在");

        var customizationTagCategory = Mapper.Map<CustomizationTagCategory>(dto);

        customizationTagCategory.Id = SequentialGuidGenerator.Instance.GetGuid();
        customizationTagCategory.CompanyId = companyId;
        customizationTagCategory.TenantId = Payload.TenantId;
        customizationTagCategory.ActivationStatus = true;
        customizationTagCategory.CreatorId = creatorId;

        await _uow.CustomizationTagCategoryRepository.CreateAsync(customizationTagCategory);

        await _uow.TagCategoryPermissionRepository.CreateAsync(new TagCategoryPermission
        {
            Id = SequentialGuidGenerator.Instance.GetGuid(),
            TenantId = Payload.TenantId,
            CompanyId = customizationTagCategory.CompanyId,
            CategoryType = TagCategoryType.Customization,
            CategoryId = customizationTagCategory.Id,
            ActivationStatus = true,
            CreatorId = creatorId
        });

        await _uow.SaveChangeAsync();

        return await GetAsync(customizationTagCategory.Id);
    }

    /// <summary>
    /// 更新自訂標籤類別
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>true/false</returns>
    public override async Task<bool> UpdateAsync(UpdateCustomizationTagCategoryDto dto)
    {
        if (!Guid.TryParse(_userId, out var modifierId)) throw new ValidationException("無法取得維護人員資訊");

        Payload.MemberId = modifierId;

        var customizationTagCategory = Mapper.Map<CustomizationTagCategory>(dto);

        var customizationTagCategoryDto = await GetAsync(customizationTagCategory.Id);

        var customizationTagCategoryDtos = await GetListAsync(m =>
            m.Name.Equals(customizationTagCategory.Name) &&
            m.CompanyId.Equals(customizationTagCategoryDto.CompanyId) &&
            !m.Id.Equals(customizationTagCategory.Id) && m.EntityStatus);

        if (customizationTagCategoryDtos.Count > 0) throw new ValidationException("資料重複");

        return await base.UpdateAsync(dto);
    }

    /// <summary>
    /// 刪除自訂標籤類別
    /// </summary>
    /// <param name="id">自訂標籤類別識別碼</param>
    /// <returns>true/false</returns>
    public override async Task<bool> DeleteAsync(Guid id)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        //1. 刪除自訂標籤類別 > customization_tag_category
        //2. 因刪除自訂標籤類別，所以將自訂標籤類別使用權限一併刪除 > tag_category_permission
        //3. 因刪除自訂標籤類別，所以將該類別下的所有標籤一併刪除 > tag
        var tagCategoryPermission = await _tagCategoryPermissionService.GetByCategoryIdAsync(id);
        var tags = await _tagService.GetByCategoryIdAsync(companyId, id);

        if (!Guid.TryParse(_userId, out var deleterId)) throw new ValidationException("無法取得維護人員資訊");
        if (tags.Count > 0) throw new ValidationException("該自訂標籤類別下已有標籤存在");

        if (tagCategoryPermission != null)
            await _uow.TagCategoryPermissionRepository.DeleteAsync(tagCategoryPermission.Id, deleterId);

        await _uow.CustomizationTagCategoryRepository.DeleteAsync(id, deleterId);

        await _uow.SaveChangeAsync();

        return true;
    }

    /// <summary>
    /// 資料彙整
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public override async Task<CustomizationTagCategoryDto> SetAdditionPropertiesAsync(CustomizationTagCategoryDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (dto == null) return null;

        var tagDtos = await _uow.TagRepository.GetListAsync(m =>
            m.CompanyId.Equals(companyId) &&
            m.CategoryId.Equals(dto.Id) &&
            m.EntityStatus);

        dto.TagQuantity = tagDtos.Count;

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