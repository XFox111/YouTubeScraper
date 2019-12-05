using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Auth.OAuth2;
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
            var task = AuthorizationHelpers.ExchangeToken(new ClientSecrets
            {
                ClientId = "1096685398208-u95rcpkqb4e1ijfmb8jdq3jsg37l8igv.apps.googleusercontent.com",
                ClientSecret = "IU5bbdjwvmx8ttJoXQ7e6JWd"
            }, "4/twFMhT4xSaAxls-rEayp8MxFI2Oy0knUdDbAXKnfyMkbDHaNyqhV6uM");
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
