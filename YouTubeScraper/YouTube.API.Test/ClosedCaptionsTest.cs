using NUnit.Framework;
using System.Linq;
using YouTube.Models;

namespace YouTube.API.Test
{
    public class ClosedCaptionsTest
    {
        [Test]
        public void ValidCaptionsTest()
        {
            YouTubeService service = new YouTubeService();
            ClosedCaptionInfo info = service.VideoPlayback.List("VC5-YkjMHuw").Execute().ClosedCaptions.FirstOrDefault();
            ClosedCaptionTrack track = service.Captions.Load(info).Execute();
            Assert.IsNotNull(track);
            Assert.IsNotEmpty(track.Captions);
        }
    }
}
