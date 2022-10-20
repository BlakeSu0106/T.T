using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Telligent.Core.Application.Services;
using Telligent.Core.Domain.Repositories;
using Telligent.Tag.Application.Dtos.BehaviorTagCategory;
using Telligent.Tag.Domain.Shared;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Application.AppServices;

public class BehaviorTagCategoryAppService :
    CrudAppService<BehaviorTagCategory, BehaviorTagCategoryDto, CreateBehaviorTagCategoryDto,
        UpdateBehaviorTagCategoryDto>
{
    private readonly MemberAppService _memberService;
    private readonly TagCategoryPermissionAppService _tagCategoryPermissionService;
    private readonly UnitOfWork _uow;

    private string _companyId;

    public BehaviorTagCategoryAppService(
        IRepository<BehaviorTagCategory> repository,
        IMapper mapper,
        MemberAppService memberService,
        TagCategoryPermissionAppService tagCategoryPermissionService,
        IHttpContextAccessor httpContextAccessor,
        UnitOfWork uow) : base(repository, mapper, httpContextAccessor)
    {
        _memberService = memberService;
        _tagCategoryPermissionService = tagCategoryPermissionService;
        _uow = uow;

        if (httpContextAccessor.HttpContext == null) return;

        _companyId = httpContextAccessor.HttpContext.Request.Headers["Company"].ToString();

        DataInitializeAsync().Wait();
    }

    /// <summary>
    /// 取得啟用的行為標籤類別
    /// </summary>
    /// <returns>行為標籤類別</returns>
    public async Task<IList<BehaviorTagCategoryDto>> GetActivatedAsync()
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        var tagCategoryPermissionDtos =
            await _tagCategoryPermissionService.GetActivatedAsync(companyId, TagCategoryType.Behavior);

        if (!tagCategoryPermissionDtos.Any()) return null;

        return await GetListAsync(m =>
            tagCategoryPermissionDtos.Select(n => n.CategoryId).Contains(m.Id) && m.EntityStatus);
    }

    /// <summary>
    /// 資料彙整
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public override async Task<BehaviorTagCategoryDto> SetAdditionPropertiesAsync(BehaviorTagCategoryDto dto)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");
        if (dto == null) return null;

        var tagDtos = await _uow.TagRepository.GetListAsync(m =>m.CompanyId.Equals(companyId) && m.CategoryId.Equals(dto.Id) && m.EntityStatus);

        dto.TagQuantity = tagDtos.Count;

        return dto;
    }

    /// <summary>
    /// 取得啟用的行為標籤類別名稱
    /// </summary>
    /// <returns>行為標籤類別名稱</returns>
    public async Task<BehaviorTagCategoryDto> GetBehaviorCategoryNameAsync(Guid categoryId)
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        var tagCategoryPermissionDtos =
            await _tagCategoryPermissionService.GetActivatedAsync(companyId, TagCategoryType.Behavior);

        if (!tagCategoryPermissionDtos.Any()) return null;

        return await GetAsync(m =>
            m.Id.Equals(categoryId) && m.EntityStatus);
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
    }
}