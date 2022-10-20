using Microsoft.AspNetCore.Mvc;
using Telligent.Tag.Application.AppServices;
using Telligent.Tag.Application.Dtos.Tag;

namespace Telligent.Tag.WebApi.Controllers;

/// <summary>
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TagController : ControllerBase
{
    private readonly TagAppService _service;

    /// <summary>
    /// </summary>
    /// <param name="service"></param>
    public TagController(TagAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 取得標籤類別使用權限
    /// </summary>
    /// <returns>標籤類別使用權限</returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetAsync()
    {
        return Ok(await _service.GetAsync());
    }

    /// <summary>
    /// 取得標籤類別使用權限
    /// </summary>
    /// <param name="categoryId">標籤類別代碼</param>
    /// <returns>標籤類別使用權限</returns>
    [HttpGet("category")]
    public async Task<IActionResult> GetByCategoryIdAsync(Guid categoryId)
    {
        return Ok(await _service.GetByCategoryIdAsync(categoryId));
    }

    /// <summary>
    /// 取得標籤依照分類頁碼
    /// </summary>
    /// <param name="categoryId">標籤類別代碼</param>
    /// <param name="limit">顯示數量</param>
    /// <param name="offset">標籤類別代碼</param>
    /// <returns>標籤統計摘要</returns>
    [HttpGet("category/page")]
    public async Task<IActionResult> GetSummaryAsync(Guid categoryId, int limit, int offset)
    {
        return Ok(await _service.GetSummaryAsync(categoryId, limit, offset));
    }

    /// <summary>
    /// 取得標籤依照分類關鍵字
    /// </summary>
    /// <param name="categoryId">標籤類別代碼</param>
    /// <param name="keyword">關鍵字</param>
    /// <returns>標籤統計摘要</returns>
    [HttpGet("category/keyword")]
    public async Task<IActionResult> GetCategoryWithKeywordAsync(Guid categoryId, string keyword)
    {
        return Ok(await _service.GetCategoryWithKeywordAsync(categoryId, keyword));
    }

    /// <summary>
    /// 依會員識別碼，取得標籤清單
    /// </summary>
    /// <param name="memberId">會員識別碼</param>
    /// <returns>標籤清單</returns>
    [HttpGet("member")]
    public async Task<IActionResult> GetByMemberIdAsync(Guid memberId)
    {
        return Ok(await _service.GetByMemberIdAsync(memberId));
    }

    /// <summary>
    /// 取得標籤池明細(會員)
    /// </summary>
    /// <param name="dto">查詢條件</param>
    /// <returns></returns>
    [HttpPost("pool/member")]
    public async Task<IActionResult> GetPoolTagByMemberAsync(QueryPoolTagDto dto)
    {
        return Ok(await _service.GetPoolTagByMemberAsync(dto));
    }

    /// <summary>
    /// 取得標籤池明細(潛客)
    /// </summary>
    /// <param name="dto">查詢條件</param>
    /// <returns></returns>
    [HttpPost("pool/prospect")]
    public async Task<IActionResult> GetPoolTagByProspectAsync(QueryPoolTagDto dto)
    {
        return Ok(await _service.GetPoolTagByProspectAsync(dto));
    }

    /// <summary>
    /// 取得標籤池明細(會員)
    /// </summary>
    /// <param name="dto">查詢條件</param>
    /// <returns></returns>
    [HttpPost("pool/category/member")]
    public async Task<IActionResult> GetCategoryTagByMemberAsync(QueryPoolTagDto dto)
    {
        return Ok(await _service.GetCategoryTagByMemberAsync(dto));
    }

    /// <summary>
    /// 取得標籤池明細(潛客)
    /// </summary>
    /// <param name="dto">查詢條件</param>
    /// <returns></returns>
    [HttpPost("pool/category/prospect")]
    public async Task<IActionResult> GetCategoryTagByProspectAsync(QueryPoolTagDto dto)
    {
        return Ok(await _service.GetCategoryTagByProspectAsync(dto));
    }

    /// <summary>
    /// 依潛客識別碼，取得標籤清單
    /// </summary>
    /// <param name="prospectId">潛客識別碼</param>
    /// <returns>標籤清單</returns>
    [HttpGet("prospect")]
    public async Task<IActionResult> GetByProspectIdAsync(Guid prospectId)
    {
        return Ok(await _service.GetByProspectIdAsync(prospectId));
    }

    /// <summary>
    /// 依標籤名稱取回標籤
    /// </summary>
    /// <param name="dto">Query</param>
    /// <returns>標籤清單</returns>
    [HttpPost("name")]
    public async Task<IActionResult> GetByNameAsync(TagDto dto)
    {
        return Ok(await _service.GetByNameAsync(dto));
    }

    /// <summary>
    /// 檢查標籤是否存在
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>true/false</returns>
    [HttpPost("exist")]
    public async Task<IActionResult> CheckDataExistAsync(CreateTagDto dto)
    {
        return Ok(await _service.CheckDataExistAsync(dto));
    }

    /// <summary>
    /// 新增標籤
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>標籤</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateMultiTagsDto dto)
    {
        return Ok(await _service.CreateAsync(dto));
    }

    /// <summary>
    /// 修改標籤
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>標籤</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateAsync(UpdateTagDto dto)
    {
        return Ok(await _service.UpdateAsync(dto));
    }

    /// <summary>
    /// 刪除標籤
    /// </summary>
    /// <param name="id">標籤識別碼</param>
    /// <returns>true/false</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        return Ok(await _service.DeleteAsync(id));
    }
}
