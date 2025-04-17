# ACC Issue API Tutorial (.Net)

![platforms](https://img.shields.io/badge/platform-windows%20%7C%20osx%20%7C%20linux-lightgray.svg)
[![.net](https://img.shields.io/badge/net-8.0-blue.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![license](https://img.shields.io/:license-mit-green.svg)](https://opensource.org/licenses/MIT)

[![Issue API of Autodesk Construction Cloud](https://img.shields.io/badge/acc%20issue%20api-v1-yellowgreen)](https://forge.autodesk.com/en/docs/acc/v1/overview/field-guide/issues/)

[Autodesk Platform Services](https://aps.autodesk.com) application built by following
the [ACC Issue](https://tutorials.autodesk.io/tutorials/acc-issue/) tutorial
from https://get-started.aps.autodesk.com/.

## Description
APS application that allows the authenticated user in [Autodesk Construction Cloud](https://www.autodesk.com/products/autodesk-docs/overview) to extract project issues and issues settings: sub types, root causes, custom attribute of a 
given ACC project. This application renders these data in a tabular view. The data can also be exported to CSV
file easily. With the data from a CSV, the application demos the scenario that imports them to create new issues or modify the existing issues.
 

![thumbnail](thumbnail.png)

## Development

### Prerequisites

- [APS credentials](https://aps.autodesk.com/en/docs/oauth/v2/tutorials/create-app)
- [Autodesk Construction Cloud](https://fieldofviewblog.wordpress.com/2017/08/31/bim-360-acc-account-for-development/)
- Provisioned access to [Autodesk Construction Cloud](https://tutorials.autodesk.io/#provision-access-in-other-products)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Terminal (for example, [Windows Command Prompt](https://en.wikipedia.org/wiki/Cmd.exe) or [macOS Terminal](https://support.apple.com/guide/terminal/welcome/mac))


### Setup & Run

- Clone this repository
- Install dependencies: `dotnet restore`
- Setup environment variables:
  - `APS_CLIENT_ID` - your APS application client ID
  - `APS_CLIENT_SECRET` - your APS application client secret
  - `APS_CALLBACK_URL` - URL for your users to be redirected to after they successfully log in with their Autodesk account
    - For local development, the callback URL is `http://localhost:8080/api/auth/callback`
    - For applications deployed to a custom domain, the callback URL is `http://<your-domain>/api/auth/callback` or `https://<your-domain>/api/auth/callback`
    - Do not forget to update the callback URL for your application in https://aps.autodesk.com/myapps as well
- Run the server: `dotnet run`

> When using [Visual Studio Code](https://code.visualstudio.com), you can specify the env. variables listed above in a _.env_ file in this folder, and run & debug the application directly from the editor.

## Main Functionalities

- Export Issue Records: The application can extract all issues data from a project and render them in a table view. It also allows the user to save as the data to a csv file
 1. login with a valid user of this ACC project.
 2. in the left tree panel, nagivate to the project to inspect
 3. click this project, the application will start to dump the issue data. Finally it renders the data in the table view. By default, the rows are sorted by **displayId**. The complicated objects (array, json, json array) will be converted to string
 4. It also extracts the records of **Issue SubTypes**, **Root Causes** and **Custom Attributes Definitions** of this project.
 5. click the toolbar button of table view, the issues view can be saved to a csv file. Because it is hard to manage the complicated objects (array, json, json array) in csv, they will be hard-coded with the string '<Complicated Objects>'. 

- Import Records from CSV to Create or Modify Issues: The application allows the user to import a csv file, with that it creates the new issue or modify the existing issues.
 1. login with a valid user of this ACC project.
 2. in the left tree panel, nagivate to the project to inspect
 3. click this project, the application will start to dump the issue data. Finally it renders the data in the table view.
 4. ensure the issue tab is activated, click the toolbar button of table view, it will ask to select a csv file. Then process the records accordingly. Finally, the table view will be refreshed with the updated issues.
    
    > It is strongly recommended to use the same pattern of the csv that is exported in the previous function (__Export Issue Records__). 

    > To add new issue, leave the column **id** empty

    > To modify the existing issue, keep the column **id** with the issue id, update other fields with new values.

    > A few fields are required when working with creating issues(POST). A few fields are supported when working with modifying issues (PATCH). Please check API references for more details.     > to make it clearer, this application narrow down with [some supported fields](https://github.com/xiaodongliang/aps-acc-issues-tutorial-nodejs/blob/main/wwwroot/table.js#L6).

    > the row number of the records is not used with these scenarios, but will be used when tracking the status. 

    > the status of created/modified/failed are tracked and will be printed out in the browser console after the process. 

## Limitations
 - not all supported fields of creating issue or modifying issues are demoed
 - to read value of csv, each cell is parsed by comma (,) but if the value itself contains comma, it would cause problem. So you may need to design the code more deliberately with your requirement. 
 - values of complicated objects are not saved to csv. you could simplify the object to individual values for working with csv.
 - the info of the logged user is not shown up. you could use GET: User/Me to fetch it
 - rate limit
 
## Troubleshooting

Please contact us via https://aps.autodesk.com/en/support/get-help.

## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).
Please see the [LICENSE](LICENSE) file for more details.
