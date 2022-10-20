using Microsoft.AspNetCore.Mvc;
using Telligent.Tag.Application.AppServices;

namespace Telligent.Tag.WebApi.Controllers;

/// <summary>
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class BehaviorTagCategoryController : ControllerBase
{
    private readonly BehaviorTagCategoryAppService _service;

    /// <summary>
    /// </summary>
    /// <param name="service"></param>
    public BehaviorTagCategoryController(BehaviorTagCategoryAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 取得啟用的行為標籤類別
    /// </summary>
    /// <returns>行為標籤類別</returns>
    [HttpGet("activated")]
    public async Task<IActionResult> GetActivatedAsync()
    {
        return Ok(await _service.GetActivatedAsync());
    }
}