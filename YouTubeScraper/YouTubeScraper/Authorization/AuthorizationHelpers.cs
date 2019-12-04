using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Oauth2.v2;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YouTube.Authorization
{
    public static class AuthorizationHelpers
    {
        public static Uri Endpoint => "https://accounts.google.com/o/oauth2/approval".ToUri();
        public static string RedirectUrl => Uri.EscapeDataString(redirectUrl);

        const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        const string redirectUrl = "urn:ietf:wg:oauth:2.0:oob";

        public static Uri FormQueryString(ClientSecrets clientSecrets, params string[] scopes)
        {
            string clientId = Uri.EscapeDataString(clientSecrets.ClientId);
            string scopeStr = string.Join(' ', scopes);

            return $"https://accounts.google.com/o/oauth2/auth?client_id={clientId}&redirect_uri={RedirectUrl}&response_type=code&scope={scopeStr}".ToUri();
        }

        public static async Task<UserCredential> ExchangeToken(ClientSecrets clientSecrets, string responseToken)
        {
            using HttpClient client = new HttpClient();

            Dictionary<string, string> requestBody = new Dictionary<string, string>
            {
                { "code", responseToken },
                { "redirect_uri", redirectUrl },
                { "grant_type", "authorization_code" },
                { "client_id", clientSecrets.ClientId },
                { "client_secret", clientSecrets.ClientSecret }
            };

            HttpResponseMessage response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));

            if (!response.IsSuccessStatusCode)
                return null;

            string responseString = await response.Content.ReadAsStringAsync();
            dynamic responseData = JsonConvert.DeserializeObject(responseString);

            TokenResponse tokenResponse = new TokenResponse
            {
                AccessToken = responseData.access_token,
                ExpiresInSeconds = responseData.expires_in,
                RefreshToken = responseData.refresh_token,
                Scope = responseData.scope,
                TokenType = responseData.token_type
            };

            AuthorizationCodeFlow authorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = clientSecrets,
                Scopes = responseData.scope.ToString().Split(' ')
            });

            return new UserCredential(authorizationCodeFlow, "user", tokenResponse);
        }
    }
}
