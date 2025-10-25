using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly APS _aps;

    public AdminController(ILogger<AdminController> logger, APS aps)
    {
        _logger = logger;
        _aps = aps;
    }
 

    [HttpGet("projectUsers")]
    public async Task<ActionResult<string>> ListProjectUsers( string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var projectUsers = await _aps.GetProjectUsersACC(projectId, tokens);
        return JsonConvert.SerializeObject(projectUsers);
    } 

}