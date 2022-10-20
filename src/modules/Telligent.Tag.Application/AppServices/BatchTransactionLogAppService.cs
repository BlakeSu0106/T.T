using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Telligent.Core.Application.Services;
using Telligent.Core.Domain.Repositories;
using Telligent.Tag.Application.Dtos.BatchTransactionLog;
using Telligent.Tag.Domain.Tags;

namespace Telligent.Tag.Application.AppServices;

public class BatchTransactionLogAppService : CrudAppService<BatchTransactionLog, BatchTransactionLogDto,
    CreateBatchTransactionLogDto, BatchTransactionLogDto>
{
    private readonly MemberAppService _memberService;
    private readonly UnitOfWork _uow;

    private string _companyId;

    public BatchTransactionLogAppService(
        IRepository<BatchTransactionLog> repository,
        IMapper mapper,
        MemberAppService memberService,
        IHttpContextAccessor httpContextAccessor,
        UnitOfWork uow)
        : base(repository, mapper, httpContextAccessor)
    {
        _memberService = memberService;
        _uow = uow;

        if (httpContextAccessor.HttpContext == null) return;

        _companyId = httpContextAccessor.HttpContext.Request.Headers["Company"].ToString();

        DataInitializeAsync().Wait();
    }

    /// <summary>
    /// 取得批量交易記錄檔
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public async Task<IList<BatchTransactionLogDto>> GetAsync()
    {
        if (!Guid.TryParse(_companyId, out var companyId)) throw new ValidationException("無法取得公司資訊");

        var batchTransactionLogDtos = new List<BatchTransactionLogDto>();
        var batchTransactionLogs =
            await _uow.BatchTransactionLogRepository.GetListAsync(m =>
                m.CompanyId.Equals(companyId) && m.EntityStatus && m.CreationTime.HasValue && (DateTime.Compare(m.CreationTime.Value, DateTime.Now.AddYears(-1)) >= 0));

        foreach (var batchTransactionLog in batchTransactionLogs)
        {
            var userDto = batchTransactionLog.CreatorId.HasValue
                ? await _memberService.GetUserAsync(batchTransactionLog.CreatorId.Value.ToString())
                : null;

            var batchTransactionLogDto = Mapper.Map<BatchTransactionLog, BatchTransactionLogDto>(batchTransactionLog);

            batchTransactionLogDto.CreatorName = userDto?.Name;

            batchTransactionLogDtos.Add(batchTransactionLogDto);
        }

        return batchTransactionLogDtos;
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