using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using YouTube.Models;

namespace YouTube.API.Test
{
    public class VideoPlaybackTest
    {
        [Test]
        public void ValidVideoPlaybackTest()
        {
            YouTubeService service = new YouTubeService();
            VideoPlayback info = service.VideoPlayback.List("VC5-YkjMHuw").Execute();
            Assert.NotNull(info);
        }
    }
}
