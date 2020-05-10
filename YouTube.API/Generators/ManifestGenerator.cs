using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Google.Apis.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using YouTube.Models;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YouTube.Generators
{
	internal class ManifestGenerator
	{
		IClientService ClientService { get; }
		YoutubeClient Client { get; }
		string Id { get; }
		Video Meta { get; set; }
		StreamManifest UrlsSet { get; set; }

		public ManifestGenerator(IClientService service, string id)
		{
			ClientService = service;
			Id = id;
			Client = new YoutubeClient(service.HttpClient);
		}

		public async Task<IReadOnlyList<DashManifest>> GenerateManifestsAsync()
		{
			Meta = await Client.Videos.GetAsync(Id);

			if (Meta == null)
				throw new FileNotFoundException("Video not found. Check video ID and visibility preferences");

			if (!string.IsNullOrWhiteSpace(await Client.Videos.Streams.GetHttpLiveStreamUrlAsync(Id)))
				throw new NotSupportedException("This is livestream. Use 'YouTubeClient.VideoPlayback.List()' to get playback URLs");

			UrlsSet = await Client.Videos.Streams.GetManifestAsync(Id);

			List<DashManifest> list = new List<DashManifest>
			{
				await GenerateManifest("Auto")
			};

			foreach (string i in UrlsSet.GetVideo().Select(k => k.VideoQualityLabel).Distinct())
				list.Add(await GenerateManifest(i));

			return list.AsReadOnly();
		}

		async Task<DashManifest> GenerateManifest(string quality)
		{
			XmlDocument manifest = new XmlDocument();
			manifest.LoadXml(Properties.Resources.DashManifestTemplate);

			manifest["MPD"].SetAttribute("mediaPresentationDuration", XmlConvert.ToString(Meta.Duration));

			StreamInfo streamInfo = await GetInfoAsync(quality);

			foreach (var i in streamInfo.Video)
			{
				string rep = GetVideoRepresentation(i);
				manifest.GetElementsByTagName("ContentComponent")[0].InnerXml += rep;
			}

			foreach (var i in streamInfo.Audio)
				manifest.GetElementsByTagName("ContentComponent")[1].InnerXml += GetAudioRepresentation(i);

			return new DashManifest(quality, manifest);
		}

		string GetVideoRepresentation(StreamInfo.VideoInfo info) =>
			$@"<Representation bandwidth=""{GetBandwidth(info.Label)}"" id=""{info.Itag}"" mimeType=""{info.MimeType}"" codecs=""{info.Codecs}"" fps=""{info.Fps}"" height=""{info.Height}"" width=""{info.Width}"">
                <BaseURL>{WebUtility.UrlEncode(info.Url)}</BaseURL>
                <SegmentBase indexRange=""{info.IndexRange}"">
                    <Initialization range=""{info.InitRange}""/>
                </SegmentBase>
            </Representation>";

		string GetAudioRepresentation(StreamInfo.AudioInfo info) =>
			$@"<Representation bandwidth=""200000"" id=""{info.Itag}"" sampleRate=""{info.SampleRate}"" numChannels=""{info.ChannelsCount}"" mimeType=""{info.MimeType}"" codecs=""{info.Codecs}"">
                <BaseURL>{WebUtility.UrlEncode(info.Url)}</BaseURL>
                <SegmentBase indexRange=""{info.IndexRange}"">
                    <Initialization range=""{info.InitRange}""/>
                </SegmentBase>
            </Representation>";

		async Task<StreamInfo> GetInfoAsync(string quality)
		{
			StreamInfo info = new StreamInfo();

			string response = await ClientService.HttpClient.GetStringAsync($"https://youtube.com/watch?v={Id}&disable_polymer=true&bpctr=9999999999&hl=en");
			IHtmlDocument videoEmbedPageHtml = new HtmlParser().ParseDocument(response);

			#region I don't know what the fuck is this
			string playerConfigRaw = Regex.Match(videoEmbedPageHtml.Source.Text,
				@"ytplayer\.config = (?<Json>\{[^\{\}]*(((?<Open>\{)[^\{\}]*)+((?<Close-Open>\})[^\{\}]*)+)*(?(Open)(?!))\})")
				.Groups["Json"].Value;
			JToken playerConfigJson = JToken.Parse(playerConfigRaw);

			var playerResponseRaw = playerConfigJson.SelectToken("args.player_response").Value<string>();
			JToken playerResponseJson = JToken.Parse(playerResponseRaw);
			string errorReason = playerResponseJson.SelectToken("playabilityStatus.reason")?.Value<string>();
			if (!string.IsNullOrWhiteSpace(errorReason))
				throw new InvalidDataException($"Video [{Id}] is unplayable. Reason: {errorReason}");

			List<Dictionary<string, string>> adaptiveStreamInfosUrl = playerConfigJson.SelectToken("args.adaptive_fmts")?.Value<string>().Split(',').Select(SplitQuery).ToList();
			List<Dictionary<string, string>> video =
				quality == "Auto" ?
					adaptiveStreamInfosUrl.FindAll(i => i.ContainsKey("quality_label")) :
					adaptiveStreamInfosUrl.FindAll(i => i.ContainsValue(quality.Substring(0, quality.IndexOf('p'))));
			List<Dictionary<string, string>> audio = adaptiveStreamInfosUrl.FindAll(i => i.ContainsKey("audio_sample_rate"));
			#endregion

			foreach (var i in video)
				info.Video.Add(new StreamInfo.VideoInfo
				{
					IndexRange = i["index"],
					Url = i["url"],
					Itag = i["itag"],
					Fps = i["fps"],
					Height = i["size"].Split('x')[1],
					Width = i["size"].Split('x')[0],
					Codecs = i["type"].Split('"')[1],
					MimeType = i["type"].Split(';')[0],
					Label = i["quality_label"]
				});

			foreach (var i in audio)
				info.Audio.Add(new StreamInfo.AudioInfo
				{
					ChannelsCount = i["audio_channels"],
					IndexRange = i["index"],
					SampleRate = i["audio_sample_rate"],
					Codecs = i["type"].Split('"')[1],
					MimeType = i["type"].Split(';')[0],
					Url = i["url"],
					Itag = i["itag"]
				});

			return info;
		}

		/// <summary>
		/// I don't know what the fuck is this either
		/// </summary>
		public Dictionary<string, string> SplitQuery(string query)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			string[] paramsEncoded = query.TrimStart('?').Split('&');
			foreach (string paramEncoded in paramsEncoded)
			{
				string param = WebUtility.UrlDecode(paramEncoded);

				// Look for the equals sign
				int equalsPos = param.IndexOf('=');
				if (equalsPos <= 0)
					continue;

				// Get the key and value
				string key = param.Substring(0, equalsPos);
				string value = equalsPos < param.Length
					? param.Substring(equalsPos + 1)
					: string.Empty;

				// Add to dictionary
				dic[key] = value;
			}

			return dic;
		}

		string GetBandwidth(string quality) =>
			quality.Split('p')[0] switch
			{
				"4320" => "16763040‬",
				"3072" => "11920384",
				"2880" => "11175360",
				"2160" => "8381520",
				"1440" => "5587680‬",
				"1080" => "4190760",
				"720" => "2073921",
				"480" => "869460",
				"360" => "686521",
				"240" => "264835",
				_ => "100000",
			};

		class StreamInfo
		{
			public class VideoInfo
			{
				public string IndexRange { get; set; }
				public string InitRange => $"0-{int.Parse(IndexRange.Split('-')[0]) - 1}";
				public string Itag { get; set; }
				public string Fps { get; set; }
				public string Url { get; set; }
				public string Codecs { get; set; }
				public string MimeType { get; set; }
				public string Height { get; set; }
				public string Width { get; set; }
				public string Label { get; set; }
			}
			public class AudioInfo
			{
				public string IndexRange { get; set; }
				public string InitRange => $"0-{int.Parse(IndexRange.Split('-')[0]) - 1}";
				public string SampleRate { get; set; }
				public string ChannelsCount { get; set; }
				public string Codecs { get; set; }
				public string MimeType { get; set; }
				public string Url { get; set; }
				public string Itag { get; set; }
			}

			public List<VideoInfo> Video { get; } = new List<VideoInfo>();
			public List<AudioInfo> Audio { get; } = new List<AudioInfo>();
		}
	}
}