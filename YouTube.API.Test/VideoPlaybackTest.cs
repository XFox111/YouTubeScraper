using NUnit.Framework;
using YouTube.Models;

namespace YouTube.API.Test
{
	public class VideoPlaybackTest
	{
		[Test]
		public void ValidVideoPlaybackTest()
		{
			ExtendedYouTubeService service = new ExtendedYouTubeService();
			VideoPlayback info = service.VideoPlayback.List("VC5-YkjMHuw").Execute();
			Assert.NotNull(info);
		}
	}
}