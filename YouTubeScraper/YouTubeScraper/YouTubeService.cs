using YouTube.Resources;

namespace YouTube
{
    public partial class YouTubeService : Google.Apis.YouTube.v3.YouTubeService
    {
        public DashManifestsResource DashManifests => new DashManifestsResource(this);
        public VideoPlaybackResource VideoPlayback => new VideoPlaybackResource(this);
        public new CaptionsResource Captions => new CaptionsResource(this);
        public HistoryResource History { get; }
        public WatchLaterResource WatchLater => new WatchLaterResource(this);
        // TODO: Add Activities override for recomendations and subscriptions
        
        public YouTubeService() : base() { }

        public YouTubeService(Initializer initializer) : base(initializer) { }
    }
}
