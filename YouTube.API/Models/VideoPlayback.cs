using System;
using System.Collections.Generic;
using YoutubeExplode.Videos.Streams;

namespace YouTube.Models
{
	public class VideoPlayback
	{
		public string Id { get; set; }
		public PlaybackUrlsData PlaybackUrls { get; set; } = new PlaybackUrlsData();
		public IReadOnlyList<ClosedCaptionInfo> ClosedCaptions { get; set; }

		public class PlaybackUrlsData
		{
			public StreamManifest VideoFilesManifest { get; set; }
			public string LiveStreamUrl { get; set; }
			[Obsolete]
			public DateTime ValidUntil { get; set; }
		}
	}
}