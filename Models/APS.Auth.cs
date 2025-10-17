using Autodesk.Authentication;
using Autodesk.Authentication.Model;

public partial class APS
{
    public string GetAuthorizationURL()
    {
        var authenticationClient = new AuthenticationClient();
        return authenticationClient.Authorize(_clientId, ResponseType.Code, _callbackUri, InternalTokenScopes);
    }

    public async Task<Tokens> GenerateTokens(string code)
    {
        var authenticationClient = new AuthenticationClient();
        var internalAuth = await authenticationClient.GetThreeLeggedTokenAsync(_clientId, code, _callbackUri, clientSecret: _clientSecret);
        return new Tokens
        {
            InternalToken = internalAuth.AccessToken,
            RefreshToken = internalAuth.RefreshToken,
            ExpiresAt = DateTime.Now.ToUniversalTime().AddSeconds((double)internalAuth.ExpiresIn)
        };
    }

    public async Task<Tokens> RefreshTokens(Tokens tokens)
    {
        var authenticationClient = new AuthenticationClient();
        var internalAuth = await authenticationClient.RefreshTokenAsync(tokens.RefreshToken, _clientId, clientSecret: _clientSecret, scopes: InternalTokenScopes);
        return new Tokens
        {
            InternalToken = internalAuth.AccessToken,
            RefreshToken = internalAuth.RefreshToken,
            ExpiresAt = DateTime.Now.ToUniversalTime().AddSeconds((double)internalAuth.ExpiresIn)
        };
    }

    public async Task<UserInfo> GetUserProfile(Tokens tokens)
    {
        var authenticationClient = new AuthenticationClient();
        UserInfo userInfo = await authenticationClient.GetUserInfoAsync(tokens.InternalToken);
        return userInfo;
    }
}