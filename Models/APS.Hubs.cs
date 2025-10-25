using Autodesk.DataManagement;
using Autodesk.DataManagement.Model;

public partial class APS
{
    public async Task<IEnumerable<HubData>> GetHubs(Tokens tokens)
    {
        var dataManagementClient = new DataManagementClient();
        var hubs = await dataManagementClient.GetHubsAsync(accessToken: tokens.InternalToken);
        return hubs.Data;
    }

    public async Task<IEnumerable<ProjectData>> GetProjects(string hubId, Tokens tokens)
    {
        var dataManagementClient = new DataManagementClient();
        var projects = await dataManagementClient.GetHubProjectsAsync(hubId, accessToken: tokens.InternalToken);
        return projects.Data;
    } 
}