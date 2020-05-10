using NUnit.Framework;
using System;
using System.Collections.Generic;
using YouTube.Models;

namespace YouTube.API.Test
{
	public class DashManifestTest
	{
		[Test]
		public void ValidManifestTest()
		{
			ExtendedYouTubeService service = new ExtendedYouTubeService();
			IReadOnlyList<DashManifest> manifests = service.DashManifests.List("NkGbcQwWxqk").Execute();
			foreach (var i in manifests)
				Console.WriteLine(i.Label);
			Assert.IsNotNull(manifests);
			Assert.IsNotEmpty(manifests);
		}
	}
}