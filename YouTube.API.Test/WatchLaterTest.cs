using System;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;
using NUnit.Framework;
using YouTube.Authorization;
using YouTube.Resources;

namespace YouTube.API.Test
{
	public class WatchLaterTest
	{
		const string testVideoId = "NkGbcQwWxqk";
		ExtendedYouTubeService service;

		[SetUp]
		public void Setup()
		{
			var task = AuthorizationHelpers.ExchangeToken(new Google.Apis.Auth.OAuth2.ClientSecrets
			{
				ClientId = "CLIENT_ID",
				ClientSecret = "CLIENT_SECRET"
			}, "SUCCESS_CODE");
			task.Wait();

			UserCredential credential = task.Result;
			service = new ExtendedYouTubeService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential,
				ApplicationName = "FoxTube"
			});
		}

		[Test]
		public void AddVideoTest()
		{
			WatchLaterResource.InsertRequest request = service.WatchLater.Insert(testVideoId, "snippet");
			PlaylistItem item = request.Execute();
			Console.WriteLine(JsonConvert.SerializeObject(item));
			Assert.IsNotNull(item);
		}

		[Test]
		public void DeleteVideoTest()
		{
			WatchLaterResource.DeleteRequest request = service.WatchLater.Delete(testVideoId);
			request.Execute();
			Assert.Pass();
		}
	}
}