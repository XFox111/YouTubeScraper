using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouTube.Resources
{
    public class WatchLaterResource
    {
        IClientService Service { get; }

        public WatchLaterResource(IClientService service) =>
            Service = service;

        public ListRequest List()
        {
            return new ListRequest();
        }

        public InsertRequest Insert(string videoId, string part) =>
            new InsertRequest(Service, videoId, part);

        public DeleteRequest Delete(string videoId) =>
            new DeleteRequest(Service, videoId);

        public class ListRequest { }
        public class InsertRequest
        {
            IClientService Service { get; set; }
            public string Id { get; set; }
            public string Part { get; set; }

            public InsertRequest(IClientService service, string videoId, string part)
            {
                Service = service;
                Id = videoId;
                Part = part;
            }

            public async Task<PlaylistItem> ExecuteAsync()
            {
                PlaylistItem playlist = new PlaylistItem
                {
                    Snippet = new PlaylistItemSnippet
                    {
                        PlaylistId = "WL",
                        ResourceId = new ResourceId
                        {
                            VideoId = Id,
                            Kind = "youtube#video"
                        }
                    }
                };
                PlaylistItemsResource.InsertRequest request = (Service as YouTubeService).PlaylistItems.Insert(playlist, Part);

                return await request.ExecuteAsync();
            }

            public PlaylistItem Execute()
            {
                Task<PlaylistItem> task = ExecuteAsync();
                task.Wait();
                return task.Result;
            }
        }
        public class DeleteRequest
        {
            IClientService Service { get; set; }
            public string Id { get; set; }

            public DeleteRequest(IClientService service, string videoId)
            {
                Service = service;
                Id = videoId;
            }

            public async Task ExecuteAsync()
            {
                ConfigurableHttpClient client = Service.HttpClient;
                string data = await client.GetStringAsync($"https://youtube.com/watch?v={Id}&disable_polymer=true&bpctr=9999999999&hl=en");
                string plid = Regex.Match(data, @"(?<=plid=).?\w+").Value;
                IHtmlDocument html = await new HtmlParser().ParseDocumentAsync(data);
                string sessionToken = html.GetElementsByTagName("input").FirstOrDefault(i => i.GetAttribute("name") == "session_token")?.GetAttribute("value");
                Dictionary<string, string> body = new Dictionary<string, string>
                {
                    { "video_ids", Id },
                    { "full_list_id", "WL" },
                    { "plid", plid },
                    { "session_token", sessionToken }
                };
                HttpResponseMessage response = await client.PostAsync("https://www.youtube.com/playlist_video_ajax?action_delete_from_playlist=1", new FormUrlEncodedContent(body));
                string responseStr = await response.Content.ReadAsStringAsync();
                if (!responseStr.Contains("SUCCESS"))
                    throw new Exception(responseStr);
            }

            public void Execute() =>
                ExecuteAsync().Wait();
        }
    }
}
