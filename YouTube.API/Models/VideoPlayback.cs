using System;
using System.Collections.Generic;

namespace YouTube.Models
{
	public class VideoPlayback
	{
		public string Id { get; set; }
		public PlaybackUrlsData PlaybackUrls { get; set; } = new PlaybackUrlsData();
		public IReadOnlyList<ClosedCaptionInfo> ClosedCaptions { get; set; }

		public class PlaybackUrlsData
		{
			public IReadOnlyList<VideoPlaybackUrl> Video { get; set; }
			public IReadOnlyList<AudioPlaybackUrl> Audio { get; set; }
			public string LiveStreamUrl { get; set; }
			public DateTime ValidUntil { get; set; }
		}
	}
}