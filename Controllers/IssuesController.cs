using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly ILogger<IssuesController> _logger;
    private readonly APS _aps;

    public IssuesController(ILogger<IssuesController> logger, APS aps)
    {
        _logger = logger;
        _aps = aps;
    }
 

    [HttpGet("issues")]
    public async Task<ActionResult<string>> ListIssues( string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var issues = await _aps.GetIssues(Request.Query["projectId"], tokens);
        return JsonConvert.SerializeObject(issues);
    }

  
    [HttpGet("subtypes")]
    public async Task<ActionResult<string>> ListIssuesSubTypes( string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var subtypes = await _aps.GetIssueSubTypes(Request.Query["projectId"], tokens);
        return JsonConvert.SerializeObject(subtypes);
    }

    [HttpGet("rootcauses")]
    public async Task<ActionResult<string>> ListRootCauses( string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var rootcauses = await _aps.GetIssueRootcauses(Request.Query["projectId"], tokens);
        return JsonConvert.SerializeObject(rootcauses);
    }

    [HttpGet("customAttDefs")]
    public async Task<ActionResult<string>> ListCustomAttDefs( string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var attdefs = await _aps.GetIssueCustomAttDefs(Request.Query["projectId"], tokens);
        return JsonConvert.SerializeObject(attdefs);
    } 

    //create new issue or modify issue
    [HttpPost("issues")]
    public async Task<ActionResult> CreateOrModifyIssues([FromBody] JObject content)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null) 
        {
            return Unauthorized(); 
        }
     
        string projectId = content["projectId"].Value<string>();
        dynamic issues = content["data"].Value<dynamic>();
 
        var status = await _aps.CreateOrModifyACCIssues(projectId, tokens,issues);

        return Ok(new {created = status.created ,modified = status.modified,failed = status.failed  });
    }


}