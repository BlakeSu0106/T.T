using Microsoft.AspNetCore.Mvc;
using Telligent.Tag.Application.AppServices;
using Telligent.Tag.Application.Dtos.Event;

namespace Telligent.Tag.WebApi.Controllers;

/// <summary>
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly EventAppService _service;

    /// <summary>
    /// </summary>
    /// <param name="service"></param>
    public EventController(EventAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 取得事件設定檔(含事件標籤設定檔)
    /// </summary>
    /// <param name="id">事件識別碼</param>
    /// <returns>事件設定檔(含事件標籤設定檔)</returns>
    [HttpGet]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        return Ok(await _service.GetAsync(id));
    }

    /// <summary>
    /// 新增事件設定檔(含事件標籤設定檔)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>事件設定檔(含事件標籤設定檔)</returns>
    [HttpPost]
    public async Task<IActionResult> PostAsync(CreateEventDto dto)
    {
        return Ok(await _service.CreateAsync(dto));
    }

    /// <summary>
    /// 修改事件設定檔(含事件標籤設定檔)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>true/false</returns>
    [HttpPut]
    public async Task<IActionResult> PutAsync(UpdateEventDto dto)
    {
        return Ok(await _service.UpdateAsync(dto));
    }

    /// <summary>
    /// 刪除事件設定檔(含事件標籤設定檔)
    /// </summary>
    /// <param name="id">事件識別碼</param>
    /// <returns>true/false</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        return Ok(await _service.DeleteAsync(id));
    }
}