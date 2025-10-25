using Autodesk.Construction.AccountAdmin;
using Autodesk.Construction.AccountAdmin.Model;

public partial class APS
{
    // The ACC Issue API only returns the user ID along with related fields.
    // To retrieve additional user information, use the endpoint below and map it to the user ID.
    public async Task<IEnumerable<dynamic>> GetProjectUsersACC(string projectId, Tokens tokens)
    {
        AdminClient adminClient = new AdminClient(_SDKManager);
        var allUsers = new List<ProjectUser>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var users = await adminClient.GetProjectUsersAsync(projectId, accessToken: tokens.InternalToken, offset: offset);
            Console.WriteLine($"Fetched {users.Results.Count} users from index {offset} record.");
            allUsers.AddRange(users.Results);
            offset += (int)users.Pagination.Limit;
            totalResult = (int)users.Pagination.TotalResults;

        } while (offset < totalResult);
        return allUsers;
    } 
}