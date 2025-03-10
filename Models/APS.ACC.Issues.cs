using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Construction.Issues;
using Autodesk.Construction.Issues.Model;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Autodesk.DataManagement.Model;

public partial class APS
{
    public async Task<IEnumerable<dynamic>> GetIssues(string projectId, Tokens tokens)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);
        var allIssues = new List<Autodesk.Construction.Issues.Model.Results>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var issues = await issueClient.GetIssuesAsync(projectId, accessToken: tokens.InternalToken,offset:offset);
            allIssues.AddRange(issues.Results);
            offset += (int)issues.Pagination.Limit;
            totalResult = (int)issues.Pagination.TotalResults;
        } while (offset < totalResult);
        return allIssues;
    }

    public async Task<IEnumerable<dynamic>> GetIssueSubTypes(string projectId, Tokens tokens)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);
        var allSubIssueTypes = new List<IssueTypeResultsSubtypes>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var issueTypes = await issueClient.GetIssuesTypesAsync(projectId, accessToken: tokens.InternalToken, include: "subtypes",offset:offset);
            List<IssueTypeResultsSubtypes> eachPage = issueTypes.Results
            .Where(type => type.Subtypes != null && type.Subtypes.Any()) // Skip type with empty subtypes
            .SelectMany(type => type.Subtypes)  // Flatten the non-empty subtypes lists
            .ToList();

            allSubIssueTypes.AddRange(eachPage);
            offset += (int)issueTypes.Pagination.Limit;
            totalResult = (int)issueTypes.Pagination.TotalResults;
        } while (offset < totalResult);
        return allSubIssueTypes;
    }

    public async Task<IEnumerable<dynamic>> GetIssueRootcauses(string projectId, Tokens tokens)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);
        var allRootcauses = new List<IssueRootCauseResultsRootCauses>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var categories = await issueClient.GetRootCauseCategoriesAsync(projectId, accessToken: tokens.InternalToken, include: "rootcauses",offset:offset);
            List<IssueRootCauseResultsRootCauses> eachPage = categories.Results
            .Where(type => type.RootCauses != null && type.RootCauses.Any()) // Skip categories with empty rootcasues lists
            .SelectMany(type => type.RootCauses)  // Flatten the non-empty rootcasues lists
            .ToList();

            allRootcauses.AddRange(eachPage);
            offset += (int)categories.Pagination.Limit;
            totalResult = (int)categories.Pagination.TotalResults;
        } while (offset < totalResult);
        return allRootcauses;
    }

    public async Task<IEnumerable<dynamic>> GetIssueCustomAttDefs(string projectId, Tokens tokens)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);
        var allCustomAttDefs = new List<AttrDefinitionResults>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var attdefs = await issueClient.GetAttributeDefinitionsAsync(projectId, accessToken: tokens.InternalToken,offset:offset);

            allCustomAttDefs.AddRange(attdefs.Results);
            offset += (int)attdefs.Pagination.Limit;
            totalResult = (int)attdefs.Pagination.TotalResults;
        } while (offset < totalResult);
        return allCustomAttDefs;
    }

    public async Task<TaskRes> CreateOrModifyACCIssues(string projectId, Tokens tokens, JArray body)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);
 
        var taskRes = new TaskRes(){
            created=new List<succeded>(),
            modified=new List<succeded>(),
            failed=new List<failed>()
        };

        foreach (JToken eachItem in body){
            Autodesk.Construction.Issues.Model.Results issue = 
                 eachItem.ToObject<Autodesk.Construction.Issues.Model.Results>(); 

            try{ 
                //some attributes are enum with IssuePayload
                //value of Autodesk.Construction.Issues.Model.Results  is string
                //need to convert to enum.
                Status status = (Status)Enum.Parse(typeof(Status),CapitalizeFirstLetter(issue.Status));
                AssignedToType assignedToType = (AssignedToType)Enum.Parse(typeof(AssignedToType),CapitalizeFirstLetter(issue.AssignedToType));

                IssuePayload issuePayload = new IssuePayload
                {
                    Title = issue.Title,
                    Description =issue.Description,
                    Status =status, 
                    IssueSubtypeId = issue.IssueSubtypeId,
                    DueDate =issue.DueDate,

                    //problems with the two attriubtes
                    //checking with SDK team

                    //AssignedTo =issue.AssignedTo,
                    //AssignedToType = AssignedToType.User,

                    RootCauseId = issue.RootCauseId,
                    Published = issue.Published
                };  

                if ((string)eachItem["id"] == null || (string)eachItem["id"] =="" ){
                    //create new issue
                    Issue res = await issueClient.CreateIssueAsync(projectId, issuePayload, accessToken: tokens.InternalToken);
                    taskRes.created.Add(new succeded { id = res.Id, csvRowNum =(string) eachItem["csvRowNum"] });
                }else{
                    //modify issue
                    Issue res = await issueClient.PatchIssueDetailsAsync(projectId, issue.Id, issuePayload, accessToken: tokens.InternalToken);
                    taskRes.modified.Add(new succeded { id = res.Id, csvRowNum =(string) eachItem["csvRowNum"] });
                }
            }
            catch (Exception e)
            {
               taskRes.failed.Add(new failed { csvRowNum =(string) eachItem["csvRowNum"], reason=e.ToString() });
            }
        }  
        return taskRes;

    } 
     
    public class TaskRes
    {
        public List<succeded> created { get; set; }
        public List<succeded> modified { get; set; }
        public List<failed> failed { get; set; }

    }
    public class succeded
    {
        public string id { get; set; }
        public string csvRowNum { get; set; }
    }
    public class failed
    {
        public string csvRowNum { get; set; }
        public string reason { get; set; }
    }

    public static string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;  // Return the input if it's null or empty
        }
        
        // Capitalize the first letter and make the rest lowercase
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

}