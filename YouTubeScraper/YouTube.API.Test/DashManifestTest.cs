using NUnit.Framework;
using System;
using System.Collections.Generic;
using YouTube.Models;

namespace YouTube.API.Test
{
    public class DashManifestTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ValidManifestTest()
        {
            YouTubeService service = new YouTubeService();
            IReadOnlyList<DashManifest> manifests = service.DashManifests.List("VC5-YkjMHuw").Execute();
            foreach (var i in manifests)
                Console.WriteLine(i.Label);
            Assert.IsNotNull(manifests);
            Assert.IsNotEmpty(manifests);
        }
    }
}