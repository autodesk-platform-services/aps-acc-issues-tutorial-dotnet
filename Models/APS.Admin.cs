using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Construction.AccountAdmin;
using Autodesk.Construction.AccountAdmin.Model;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class APS
{ 
    // By ACC Issue API, it only tells the user id with related fields.
    //to fetch the information of the user, call the endpoint below and map with the user id.
    public async Task<IEnumerable<dynamic>> GetProjectUsersACC( string projectId, Tokens tokens)
    {
        AdminClient adminClient = new AdminClient(_SDKManager);
        var allUsers = new List<ProjectUser>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var users = await adminClient.GetProjectUsersAsync(projectId,accessToken:tokens.InternalToken,offset:offset);
            allUsers.AddRange(users.Results);
            offset += (int)users.Pagination.Limit;
            totalResult = (int)users.Pagination.TotalResults;
        } while (offset < totalResult);
        return allUsers;
    }

    // // By ACC Issue API, it only tells the company id with related fields.
    // //to fetch the information of the company, call the endpoint below and map with the company id.
    // public async Task<IEnumerable<dynamic>> GetProjectCompanyACC( string projectId, Tokens tokens)
    // {
    //     AdminClient adminClient = new AdminClient(_SDKManager);
    //     var allCompanies = new List<CompanyResponse>();
    //     var offset = 0;
    //     var totalResult = 0;
    //     do
    //     {
    //         var companies = await adminClient.GetProjectCompaniesAsync(tokens.InternalToken, projectId,offset:offset);
    //         allCompanies.AddRange(companies);
    //         offset += companies.Count;
    //         //totalResult = (int)companies.Pagination.TotalResults;
    //     }// while (offset < totalResult);
    //      while ( companies != 0);
    //     return allCompanies;
    // }
 
}