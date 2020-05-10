using Google.Apis.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTube.Generators;
using YouTube.Models;

namespace YouTube.Resources
{
	public class DashManifestsResource
	{
		IClientService Service { get; }
		public DashManifestsResource(IClientService service) =>
			Service = service;

		public ListRequest List(string videoId) =>
			new ListRequest(Service, videoId);

		public class ListRequest
		{
			public string Id { get; set; }
			IClientService Service { get; set; }

			public ListRequest(IClientService service, string id)
			{
				Id = id;
				Service = service;
			}

			public async Task<IReadOnlyList<DashManifest>> ExecuteAsync()
			{
				ManifestGenerator generator = new ManifestGenerator(Service, Id);

				return await generator.GenerateManifestsAsync();
			}

			public IReadOnlyList<DashManifest> Execute()
			{
				Task<IReadOnlyList<DashManifest>> task = ExecuteAsync();
				task.Wait();
				return task.Result;
			}
		}
	}
}