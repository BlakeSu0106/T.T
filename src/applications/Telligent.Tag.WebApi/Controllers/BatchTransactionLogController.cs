using Microsoft.AspNetCore.Mvc;
using Telligent.Tag.Application.AppServices;

namespace Telligent.Tag.WebApi.Controllers;

/// <summary>
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class BatchTransactionLogController : ControllerBase
{
    private readonly BatchTransactionLogAppService _service;

    /// <summary>
    /// </summary>
    /// <param name="service"></param>
    public BatchTransactionLogController(BatchTransactionLogAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// 取得批量交易記錄檔
    /// </summary>
    /// <returns>批量交易記錄檔</returns>
    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        return Ok(await _service.GetAsync());
    }
}