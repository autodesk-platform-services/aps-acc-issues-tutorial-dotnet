using System.Reflection;
using Autodesk.Construction.Issues.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly ILogger<IssuesController> _logger;
    private readonly APS _aps;

    private JsonSerializerSettings settings = new JsonSerializerSettings
    {
         ContractResolver = new ForceIncludeNullsResolver(),
         NullValueHandling = NullValueHandling.Include
    };

    public IssuesController(ILogger<IssuesController> logger, APS aps)
    {
        _logger = logger;
        _aps = aps;
    }


    [HttpGet("issues")]
    public async Task<ActionResult<string>> ListIssues(string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var issues = await _aps.GetIssues(Request.Query["projectId"], tokens);  
        return JsonConvert.SerializeObject(issues,settings);
    }


    [HttpGet("subtypes")]
    public async Task<ActionResult<string>> ListIssuesSubTypes(string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var subtypes = await _aps.GetIssueSubTypes(Request.Query["projectId"], tokens);
        return JsonConvert.SerializeObject(subtypes,settings);
    }

    [HttpGet("rootcauses")]
    public async Task<ActionResult<string>> ListRootCauses(string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var rootcauses = await _aps.GetIssueRootcauses(Request.Query["projectId"], tokens);
        return JsonConvert.SerializeObject(rootcauses,settings);
    }

    [HttpGet("customAttDefs")]
    public async Task<ActionResult<string>> ListCustomAttDefs(string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var attdefs = await _aps.GetIssueCustomAttDefs(Request.Query["projectId"], tokens);
        return JsonConvert.SerializeObject(attdefs,settings);
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

        var status = await _aps.CreateOrModifyACCIssues(projectId, tokens, issues);

        return Ok(new { created = status.created, modified = status.modified, failed = status.failed });
    }

    [HttpGet("issueUserProfile")]
    public async Task<ActionResult<string>> IssueUserProfile(string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }
        var list = new List<User>(); //to feed the table view of client side. build a dummy json array.
        var userInfo = await _aps.GetIssueUserProfile(Request.Query["projectId"], tokens);
        list.Add(userInfo);
        return JsonConvert.SerializeObject(list,settings);;
    }


}
//include the fields whose value =null when  JsonConvert.SerializeObject
public class ForceIncludeNullsResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);

        // Always include properties, even if they're null
        prop.NullValueHandling = NullValueHandling.Include;
        prop.DefaultValueHandling = DefaultValueHandling.Include;

        return prop;
    }
}