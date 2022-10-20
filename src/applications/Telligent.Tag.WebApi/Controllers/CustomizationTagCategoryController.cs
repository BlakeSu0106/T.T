using Microsoft.AspNetCore.Mvc;
using Telligent.Tag.Application.AppServices;
using Telligent.Tag.Application.Dtos.CustomizationTagCategory;

namespace Telligent.Tag.WebApi.Controllers;

/// <summary>
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CustomizationTagCategoryController : ControllerBase
{
    private readonly CustomizationTagCategoryAppService _service;

    /// <summary>
    /// </summary>
    /// <param name="service"></param>
    public CustomizationTagCategoryController(CustomizationTagCategoryAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 取得啟用的自訂標籤類別
    /// </summary>
    /// <returns>自訂標籤類別</returns>
    [HttpGet("activated")]
    public async Task<IActionResult> GetActivatedAsync()
    {
        return Ok(await _service.GetActivatedAsync());
    }

    /// <summary>
    /// 取得自訂標籤類別(含啟用/停用)
    /// </summary>
    /// <returns>自訂標籤類別</returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    /// <summary>
    /// 新增自訂標籤類別
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>自訂標籤類別</returns>
    [HttpPost]
    public async Task<IActionResult> PostAsync(CreateCustomizationTagCategoryDto dto)
    {
        return Ok(await _service.CreateAsync(dto));
    }

    /// <summary>
    /// 更新自訂標籤類別
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>true/false</returns>
    [HttpPut]
    public async Task<IActionResult> PutAsync(UpdateCustomizationTagCategoryDto dto)
    {
        return Ok(await _service.UpdateAsync(dto));
    }

    /// <summary>
    /// 刪除自訂標籤類別
    /// </summary>
    /// <param name="id">自訂標籤類別識別碼</param>
    /// <returns>true/false</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        return Ok(await _service.DeleteAsync(id));
    }
}