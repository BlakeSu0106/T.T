using Microsoft.AspNetCore.Mvc;
using Telligent.Tag.Application.AppServices;
using Telligent.Tag.Application.Dtos;
using Telligent.Tag.Application.Dtos.TagTracking;

namespace Telligent.Tag.WebApi.Controllers;

/// <summary>
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TagTrackingController : ControllerBase
{
    private readonly TagTrackingAppService _service;

    /// <summary>
    /// </summary>
    /// <param name="service"></param>
    public TagTrackingController(TagTrackingAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 新增標籤追蹤檔
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> PostAsync(CreateTagTrackingDto dto)
    {
        return Ok(await _service.CreateTagTrackingAsync(dto));
    }

    /// <summary>
    /// 新增電商複數標籤追蹤檔 - 會員貼標 (目前用於TE電商)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("electronicCommerce/member")]
    public async Task<IActionResult> PostMemberElectronicCommerceTagTrackingAsync(CreatePluralElectronicCommerceTagTrackingDto dto)
    {
        return Ok(await _service.CreatePluralMemberElectronicCommerceTagTrackingAsync(dto));
    }

    /// <summary>
    /// 新增電商複數標籤追蹤檔 - 商品貼標 (目前用於TE電商)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("electronicCommerce/product")]
    public async Task<IActionResult> PostProductElectronicCommerceTagTrackingAsync(CreatePluralElectronicCommerceTagTrackingDto dto)
    {
        return Ok(await _service.CreatePluralProductElectronicCommerceTagTrackingAsync(dto));
    }

    /// <summary>
    /// 批量新增標籤
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("batch")]
    public async Task<IActionResult> PostBatchStickTagAsync(BatchStickTagDto dto)
    {
        return Ok(await _service.CreateBatchStickTagAsync(dto));
    }

    /// <summary>
    /// 批量刪除標籤
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpDelete("batch")]
    public async Task<IActionResult> DeleteBatchStickTagAsync(BatchStickTagDto dto)
    {
        return Ok(await _service.DeleteBatchStickTagAsync(dto));
    }
}