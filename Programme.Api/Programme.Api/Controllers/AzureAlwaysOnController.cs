using Microsoft.AspNetCore.Mvc;

namespace Programme.Api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class AzureAlwaysOnController : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        // Azure hits this endpoint periodically to keep the app alive if AlwaysOn is turned on (default)
        // Requests to this endpoint will be filtered from all logging, and won't show in Swagger.

        return Ok();
    }
}