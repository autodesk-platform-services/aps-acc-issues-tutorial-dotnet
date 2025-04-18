using Autodesk.Authentication.Model;
using Autodesk.SDKManager;

public class Tokens
{
    public string? InternalToken;
    public string? PublicToken;
    public string? RefreshToken;
    public DateTime ExpiresAt;
}

public partial class APS
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _callbackUri;
    //for working with ACC Issue on server side
    private readonly List<Scopes> InternalTokenScopes = [Scopes.DataRead, Scopes.DataWrite,Scopes.AccountRead];
    //for working with APS Viewer on client side (future tutorial)
    private readonly List<Scopes> PublicTokenScopes = [Scopes.ViewablesRead];
    private SDKManager _SDKManager;


    public APS(string clientId, string clientSecret, string callbackUri)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _callbackUri = callbackUri;
        _SDKManager = SdkManagerBuilder.Create().Build();

    }
}