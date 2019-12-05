using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace YouTube.Authorization
{
    public static class AuthorizationHelpers
    {
        public static Uri Endpoint => "https://accounts.google.com/o/oauth2/approval".ToUri();
        const string refreshEndpoint = "https://oauth2.googleapis.com/token";
        const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        const string redirectUrl = "urn:ietf:wg:oauth:2.0:oob";

        public static Uri FormQueryString(ClientSecrets clientSecrets, params string[] scopes)
        {
            string clientId = Uri.EscapeDataString(clientSecrets.ClientId);
            string scopeStr = string.Join(" ", scopes);

            return $"https://accounts.google.com/o/oauth2/auth?client_id={clientId}&redirect_uri={redirectUrl}&response_type=code&scope={scopeStr}".ToUri();
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
                throw new Exception(await response.Content.ReadAsStringAsync());

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

            AuthorizationCodeFlow authorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = responseData.scope.ToString().Split(' ')
            });

            UserCredential credential = new UserCredential(authorizationCodeFlow, "user", tokenResponse);

            return credential;
        }

        public static async Task<UserCredential> RestoreUser(ClientSecrets clientSecrets, string refreshToken)
        {
            using HttpClient client = new HttpClient();

            Dictionary<string, string> requestBody = new Dictionary<string, string>
            {
                { "client_id", clientSecrets.ClientId },
                { "client_secret", clientSecrets.ClientSecret },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            };

            HttpResponseMessage response = await client.PostAsync(refreshEndpoint, new FormUrlEncodedContent(requestBody));

            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());

            string responseString = await response.Content.ReadAsStringAsync();
            dynamic responseData = JsonConvert.DeserializeObject(responseString);

            TokenResponse tokenResponse = new TokenResponse
            {
                AccessToken = responseData.access_token,
                ExpiresInSeconds = responseData.expires_in,
                RefreshToken = refreshToken,
                TokenType = responseData.token_type
            };

            AuthorizationCodeFlow authorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets
            });

            UserCredential credential = new UserCredential(authorizationCodeFlow, "user", tokenResponse);

            return credential;
        }
    }
}
