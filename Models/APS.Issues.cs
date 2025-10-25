using Autodesk.Construction.Issues;
using Autodesk.Construction.Issues.Model;
using Newtonsoft.Json.Linq;

public partial class APS
{
    public async Task<IEnumerable<dynamic>> GetIssues(string projectId, Tokens tokens)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);
        var allIssues = new List<Issue>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var issues = await issueClient.GetIssuesAsync(projectId, accessToken: tokens.InternalToken, offset: offset);
            Console.WriteLine($"Fetched {issues.Results.Count} issues from index {offset} record.");

            allIssues.AddRange(issues.Results);
            offset += (int)issues.Pagination.Limit;
            totalResult = (int)issues.Pagination.TotalResults;

        } while (offset < totalResult);
        return allIssues;
    }

    // Issue Settings

    //get issue sub types
    public async Task<IEnumerable<dynamic>> GetIssueSubTypes(string projectId, Tokens tokens)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);
        var allSubIssueTypes = new List<TypesPageResultsSubtypes>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var issueTypes = await issueClient.GetIssuesTypesAsync(projectId, accessToken: tokens.InternalToken, include: "subtypes", offset: offset);

            Console.WriteLine($"Fetched {issueTypes.Results.Count}  sub types from index {offset} record");

            List<TypesPageResultsSubtypes> eachPage = issueTypes.Results
            .Where(type => type.Subtypes != null && type.Subtypes.Any()) // Skip type with empty subtypes
            .SelectMany(type => type.Subtypes)  // Flatten the non-empty subtypes lists
            .ToList();

            allSubIssueTypes.AddRange(eachPage);
            offset += (int)issueTypes.Pagination.Limit;
            totalResult = (int)issueTypes.Pagination.TotalResults;

        } while (offset < totalResult);
        return allSubIssueTypes;
    }

    //get issue root causes
    public async Task<IEnumerable<dynamic>> GetIssueRootcauses(string projectId, Tokens tokens)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);
        var allRootcauses = new List<RootCauseCategoriesPageResultsRootCauses>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var categories = await issueClient.GetRootCauseCategoriesAsync(projectId, accessToken: tokens.InternalToken, include: "rootcauses", offset: offset);
            List<RootCauseCategoriesPageResultsRootCauses> eachPage = categories.Results
            .Where(type => type.RootCauses != null && type.RootCauses.Any()) // Skip categories with empty rootcasues lists
            .SelectMany(type => type.RootCauses)  // Flatten the non-empty rootcasues lists
            .ToList();

            allRootcauses.AddRange(eachPage);
            offset += (int)categories.Pagination.Limit;
            totalResult = (int)categories.Pagination.TotalResults;

            Console.WriteLine($"Fetched {categories.Results.Count} root causes from index {offset} record.");

        } while (offset < totalResult);
        return allRootcauses;
    }

    //get custom attributes definitions
    public async Task<IEnumerable<dynamic>> GetIssueCustomAttDefs(string projectId, Tokens tokens)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);
        var allCustomAttDefs = new List<AttrDefinitionPageResults>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var attdefs = await issueClient.GetAttributeDefinitionsAsync(projectId, accessToken: tokens.InternalToken, offset: offset);
            Console.WriteLine($"Fetched {attdefs.Results.Count} custom attributes definitions from index {offset} record.");

            allCustomAttDefs.AddRange(attdefs.Results);
            offset += (int)attdefs.Pagination.Limit;
            totalResult = (int)attdefs.Pagination.TotalResults;

        } while (offset < totalResult);
        return allCustomAttDefs;
    }

    //create or modify issues
    public async Task<TaskRes> CreateOrModifyACCIssues(string projectId, Tokens tokens, JArray body)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);

        var taskRes = new TaskRes()
        {
            created = new List<succeded>(),
            modified = new List<succeded>(),
            failed = new List<failed>()
        };

        foreach (JToken eachItem in body)
        {
            Issue issue = eachItem.ToObject<Issue>();

            try
            {
                //build issue payload with non-null properties only
                var issuePayload = new IssuePayload();
                var inputDataProperties = typeof(Issue).GetProperties();
                var issuePayloadProperties = typeof(IssuePayload).GetProperties();

                foreach (var property in inputDataProperties)
                {
                    var value = property.GetValue(issue);
                    if (value != null)
                    {
                        // Find the corresponding property in the IssuePayload class
                        var matchingProperty = issuePayloadProperties.FirstOrDefault(p => p.Name == property.Name && p.CanWrite);
                        if (matchingProperty != null)
                        {
                            Type propertyType = matchingProperty.PropertyType;
                            Type actualType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                            // If the property is an enum, parse the string value to the enum type
                            if (actualType.IsEnum)
                            {
                                var enumValue = Enum.Parse(actualType, value.ToString(), true);
                                matchingProperty.SetValue(issuePayload, enumValue);
                            }
                            else
                            {
                                matchingProperty.SetValue(issuePayload, value);
                            }
                        }
                    }
                }

                if ((string)eachItem["id"] == null || (string)eachItem["id"] == "")
                {
                    //create new issue
                    Issue res = await issueClient.CreateIssueAsync(projectId, issuePayload, accessToken: tokens.InternalToken);
                    taskRes.created.Add(new succeded { id = res.Id, csvRowNum = (string)eachItem["csvRowNum"] });
                    Console.WriteLine($"Created one new issue with id: {res.Id}"); 
                }
                else
                {
                    //modify issue
                    Issue res = await issueClient.PatchIssueDetailsAsync(projectId, issue.Id, issuePayload, accessToken: tokens.InternalToken);
                    taskRes.modified.Add(new succeded { id = res.Id, csvRowNum = (string)eachItem["csvRowNum"] });
                    Console.WriteLine($"Modified one issue with id: {res.Id}");
                }
            }
            catch (Exception e)
            {
                taskRes.failed.Add(new failed { csvRowNum = (string)eachItem["csvRowNum"], reason = e.ToString() });
                Console.WriteLine($"Failed to create/modify issue at csv row number: {(string)eachItem["csvRowNum"]}. Error: {e.ToString()}");
            }
        }
        return taskRes;

    }

    //get user permission in Issue
    public async Task<User> GetIssueUserProfile(string projectId, Tokens tokens)
    {
        IssuesClient issueClient = new IssuesClient(_SDKManager);
        var userInfo = await issueClient.GetUserProfileAsync(projectId, accessToken: tokens.InternalToken);
        return userInfo;
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

}
