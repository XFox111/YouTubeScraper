using Google.Apis.Services;

namespace YouTube.Resources
{
    public class CaptionsResource
    {
        IClientService Service { get; }
        public CaptionsResource(IClientService service) =>
            Service = service;
    }
}
