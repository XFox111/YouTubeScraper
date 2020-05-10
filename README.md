# ExtendedYouTubeAPI
![Nuget](https://img.shields.io/nuget/dt/ExtendedYouTubeAPI)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
![Platforms](https://img.shields.io/badge/platforms-.net-lightgrey)

![Twitter Follow](https://img.shields.io/twitter/follow/xfox111?style=social)
![GitHub followers](https://img.shields.io/github/followers/xfox111?label=Follow%20@xfox111&style=social)

C# library which is used to extend the abilities of YouTube API v3
## Features
- DASH manifests generation for videos
- HLS livestreams URLs extraction
- User's history management (list, add, delete, update)
- Watch later playlist management (list, add, delete)
- Video captions retrieval
- User's recommendations listing
- User's subscriptions videos listing
- Videos' URLs retrieval
- UWP authorization helpers

## Get started
- Download and install package from [NuGet](https://www.nuget.org/packages/ExtendedYouTubeAPI/)
### Authorization (UWP)
```
using System;
using Google.Apis.Auth.OAuth2;
using YouTube.Authorization;
using Windows.Security.Authentication.Web;

...

ClientSecrets secrets = new ClientSecrets	// Initialize your project secrets
{
	ClientId = "%CLIENT_ID%",
	ClientSecret = "%CLIENT_SECRET%"
}

string[] scopes = new string[]			// Define scopes you wanna access
{
	Oauth2Service.Scope.UserinfoProfile,
	Oauth2Service.Scope.UserinfoEmail,
	YouTubeService.Scope.YoutubeForceSsl
};

Uri requestString = AuthorizationHelpers.FormQueryString(secrets, scopes);	// Generate authorization link

WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.UseTitle, requestString, AuthorizationHelpers.Endpoint);	// Call authentication broker with generated query string and predefined endpoint. WebAuthenticationOptions.UseTitle is required

if (result.ResponseStatus == WebAuthenticationStatus.Success)	// Process response
{
	string successCode = AuthorizationHelpers.ParseSuccessCode(result.ResponseData);	// Retrieve success code
	YouTube.Authorization.UserCredential credential = await AuthorizationHelpers.ExchangeToken(ClientSecrets, successCode);	// Excahnge success token for UserCredential
	
	// Use UserCredential to create YouTube.ExtendedYouTubeService

	// Save refresh token for future use. Recommended way: PasswordVault
	PasswordVault passwordVault = new PasswordVault();
	PasswordCredential tokenData = new PasswordCredential("%ANY_ID%", "%AUTHORIZED_USER_ID%", credential.Token.RefreshToken)
	passwordVault.Add(tokenData);

	// Update stored refresh token on renew
	credential.RefreshTokenUpdated += (s, e) =>
	{
		tokenData.Password = credential.Token.RefreshToken
	}
}
else
{
	// Do something
}
```
### Service retrieval
```
using YouTube;
using YouTube.Authorization;
using Google.Apis.Services;

...

YouTubeExtendedService GetService(UserCredential credential = null)
{
	BaseClientService.Initializer initializer = new BaseClientService.Initializer
	{
		ApplicationName = "%APP_NAME%",
		ApiKey = "%API_KEY%",		// In case there's no UserCredential and usage is anonymous
		HttpClientInitializer = credential
	};

	ExtendedYouTubeService service = new ExtendedYouTubeService(initializer);

	returnt service
}
```

### DASH manifest retrieval
```
using YouTube;
using YouTube.Models;
using System.Collections.Generic;
using Windows.Media.Playback;

...

ExtendedYouTubeService service = new ExtendedYouTubeService();	// Get the Service
var request = service.DashManifests.List("%VIDEO_ID%");
// request.Id = "%VIDEO_ID%";	// Change video ID after request initialization

IReadOnlyList<DashManifest> manifests = await request.ExecuteAsync();	// Execute request. There will be manifests for all available qualities, including "Auto"

// DashManifest.Label - Quality label
// DashManifest.ValidUntil - After this date URLs in the manifest will be invalid
// DashManifest.Xml - XmlDocument instance of the manifest

Uri manifestUri = manifests[0].WriteManifest(TempFileStream);	// Save manifest to temporary file to access it with MediaPlayer

...

// Play video with manifest
MediaPlayer mediaPlayer = new MediaPlayer();
mediaPlayer.Source = MediaSource.CreateFromUri(manifestUri);
mediaPlayer.Play();
```

### Other data
Other resources are retrieved pretty much the same as in the vanilia library

## TODO
- Create API Reference page
- Document code
- Setup CI/CD
- Add more tests
- Implement user history management
- Implement search history management
- Implement video recommendations/subscirptions listing

## Special thanks
- [Tyrrrz](https://github.com/Tyrrrz) for [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) library which is used in the my one

## Copyrights
> ©2020 Michael "XFox" Gordeev