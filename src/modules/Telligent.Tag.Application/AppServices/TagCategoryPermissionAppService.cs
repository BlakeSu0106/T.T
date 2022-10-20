using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Telligent.Core.Application.Services;
using Telligent.Core.Domain.Repositories;
using Telligent.Tag.Application.Dtos.TagCategoryPermission;
using Telligent.Tag.Domain.Shared;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Application.AppServices;

public class TagCategoryPermissionAppService :
    CrudAppService<TagCategoryPermission, TagCategoryPermissionDto, CreateTagCategoryPermissionDto,
        UpdateTagCategoryPermissionDto>
{
    private readonly MemberAppService _memberService;
    private readonly UnitOfWork _uow;

    private string _companyId;
    private string _userId;

    public TagCategoryPermissionAppService(
        IRepository<TagCategoryPermission> repository,
        IMapper mapper,
        MemberAppService memberService,
        IHttpContextAccessor httpContextAccessor,
        UnitOfWork uow) : base(repository, mapper, httpContextAccessor)
    {
        _memberService = memberService;
        _uow = uow;

        if (httpContextAccessor.HttpContext == null) return;

        _companyId = httpContextAccessor.HttpContext.Request.Headers["Company"].ToString();
        _userId = httpContextAccessor.HttpContext.Request.Headers["User"].ToString();

        DataInitializeAsync().Wait();
    }

    /// <summary>
    /// 取得啟用的標籤類別使用權限
    /// </summary>
    /// <returns>標籤類別使用權限</returns>
    private async Task<IList<TagCategoryPermissionDto>> GetActivatedAsync()
    {
        return await GetListAsync(m =>
            m.EntityStatus &&
            m.ActivationStatus &&
            ((!m.ActivationStartTime.HasValue && !m.ActivationEndTime.HasValue) ||
             (m.ActivationStartTime.HasValue && m.ActivationEndTime.HasValue &&
              DateTime.Compare(m.ActivationStartTime.Value.Date, DateTime.Now.Date) <= 0 &&
              DateTime.Compare(m.ActivationEndTime.Value.Date, DateTime.Now.Date) >= 0)));
    }

    /// <summary>
    /// 取得啟用的標籤類別使用權限
    /// </summary>
    /// <param name="companyId">公司識別碼</param>
    /// <param name="type">標籤分類類別</param>
    /// <returns>標籤類別使用權限</returns>
    public async Task<IList<TagCategoryPermissionDto>> GetActivatedAsync(Guid companyId, TagCategoryType type)
    {
        var result = await GetActivatedAsync();

        return result.Where(m =>
            m.CompanyId.Equals(companyId) &&
            m.CategoryType.Equals(type)).ToList();
    }

    /// <summary>
    /// 取得標籤類別使用權限
    /// </summary>
    /// <param name="categoryId">標籤類別識別碼</param>
    /// <returns>標籤類別使用權限</returns>
    public async Task<TagCategoryPermissionDto> GetByCategoryIdAsync(Guid categoryId)
    {
        var permissions = await GetListAsync(m =>
            m.CategoryId.Equals(categoryId) &&
            m.EntityStatus);

        return permissions.FirstOrDefault();
    }

    /// <summary>
    /// 新增標籤類別使用權限
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>標籤類別使用權限</returns>
    /// <exception cref="ValidationException"></exception>
    public override async Task<TagCategoryPermissionDto> CreateAsync(CreateTagCategoryPermissionDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (!Guid.TryParse(_userId, out var creatorId)) throw new ValidationException("無法取得維護人員資訊");

        Payload.MemberId = creatorId;

        dto.CompanyId = companyId;

        var tagCategoryPermissionDtos = await GetListAsync(m =>
            m.CompanyId.Equals(dto.CompanyId) &&
            m.CategoryType.Equals(dto.CategoryType) &&
            m.CategoryId.Equals(dto.CategoryId) &&
            m.EntityStatus);

        if (tagCategoryPermissionDtos.Count > 0) throw new ValidationException("資料已存在");

        return await base.CreateAsync(dto);
    }

    /// <summary>
    /// 修改標籤類別使用權限
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public override async Task<bool> UpdateAsync(UpdateTagCategoryPermissionDto dto)
    {
        if (!Guid.TryParse(_userId, out var modifierId)) throw new ValidationException("無法取得維護人員資訊");

        Payload.MemberId = modifierId;

        return await base.UpdateAsync(dto);
    }

    /// <summary>
    /// 刪除標籤類別使用權限
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public override async Task<bool> DeleteAsync(Guid id)
    {
        if (!Guid.TryParse(_userId, out var deleterId)) throw new ValidationException("無法取得維護人員資訊");

        Payload.MemberId = deleterId;

        return await base.DeleteAsync(id);
    }

    /// <summary>
    /// 取得啟用的標籤類別
    /// </summary>
    /// <returns>標籤類別</returns>
    public async Task<IList<ActivatedTagCategoryDto>> GetActivatedTagCategoryAsync()
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        var activatedDtos = await GetActivatedAsync();

        if (!activatedDtos.Any()) return null;

        var tagCategoryPermissionDtos = activatedDtos.Where(m => m.CompanyId.Equals(companyId));

        var activatedTagCategoryDtos = tagCategoryPermissionDtos.Select(m => new ActivatedTagCategoryDto
        {
            CompanyId = companyId,
            CategoryType = m.CategoryType,
            CategoryId = m.CategoryId
        }).ToList();

        foreach (var activatedTagCategoryDto in activatedTagCategoryDtos)
        {
            var name = string.Empty;

            switch (activatedTagCategoryDto.CategoryType)
            {
                case TagCategoryType.Behavior:
                {
                    var tagCategory =
                        await _uow.BehaviorTagCategoryRepository.GetAsync(activatedTagCategoryDto.CategoryId);

                    if (tagCategory != null) name = tagCategory.Name;
                    break;
                }
                case TagCategoryType.Customization:
                {
                    var tagCategory =
                        await _uow.CustomizationTagCategoryRepository.GetAsync(activatedTagCategoryDto.CategoryId);

                    if (tagCategory != null) name = tagCategory.Name;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            activatedTagCategoryDto.Name = name;
        }

        return activatedTagCategoryDtos;
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