using Microsoft.AspNetCore.Mvc;
using Telligent.Tag.Application.AppServices;
using Telligent.Tag.Application.Dtos.TagCategoryPermission;

namespace Telligent.Tag.WebApi.Controllers;

/// <summary>
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TagCategoryPermissionController : ControllerBase
{
    private readonly TagCategoryPermissionAppService _service;

    /// <summary>
    /// </summary>
    /// <param name="service"></param>
    public TagCategoryPermissionController(TagCategoryPermissionAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 取得標籤類別清單
    /// </summary>
    /// <returns>標籤類別清單</returns>
    [HttpGet("activated")]
    public async Task<IActionResult> GetAsync()
    {
        return Ok(await _service.GetActivatedTagCategoryAsync());
    }

    /// <summary>
    /// 新增標籤類別使用權限
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> PostAsync(CreateTagCategoryPermissionDto dto)
    {
        return Ok(await _service.CreateAsync(dto));
    }

    /// <summary>
    /// 更新標籤類別使用權限
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>true/false</returns>
    [HttpPut]
    public async Task<IActionResult> PutAsync(UpdateTagCategoryPermissionDto dto)
    {
        return Ok(await _service.UpdateAsync(dto));
    }

    /// <summary>
    /// 刪除標籤類別使用權限
    /// </summary>
    /// <param name="id">標籤類別使用權限識別碼</param>
    /// <returns>true/false</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        return Ok(await _service.DeleteAsync(id));
    }
}