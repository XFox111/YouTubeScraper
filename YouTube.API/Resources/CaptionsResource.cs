using Google.Apis.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTube.Models;
using YoutubeExplode;

namespace YouTube.Resources
{
	public class CaptionsResource : Google.Apis.YouTube.v3.CaptionsResource
	{
		IClientService Service { get; }
		public CaptionsResource(IClientService service) : base(service) =>
			Service = service;

		public LoadRequest Load(ClosedCaptionInfo captionInfo) =>
			new LoadRequest(Service, captionInfo);

		public class LoadRequest
		{
			public ClosedCaptionInfo CaptionInfo { get; set; }
			IClientService Service { get; set; }

			public LoadRequest(IClientService service, ClosedCaptionInfo captionInfo)
			{
				CaptionInfo = captionInfo;
				Service = service;
			}

			public async Task<ClosedCaptionTrack> ExecuteAsync()
			{
				YoutubeClient client = new YoutubeClient(Service.HttpClient);
				var response = await client.Videos.ClosedCaptions.GetAsync(CaptionInfo.TrackInfo);
				List<ClosedCaptionTrack.ClosedCaption> captions = new List<ClosedCaptionTrack.ClosedCaption>();
				foreach (var i in response.Captions)
					captions.Add(new ClosedCaptionTrack.ClosedCaption
					{
						Offset = i.Offset,
						Duration = i.Duration,
						Content = i.Text
					});

				return new ClosedCaptionTrack
				{
					Info = CaptionInfo,
					Captions = captions.AsReadOnly()
				};
			}

			public ClosedCaptionTrack Execute()
			{
				Task<ClosedCaptionTrack> task = ExecuteAsync();
				task.Wait();
				return task.Result;
			}
		}
	}
}