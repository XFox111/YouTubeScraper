using System;
using Google.Apis.YouTube.v3;
using YouTubeScraper.Resources;

namespace YouTubeScraper
{
    public class YouTubeScraper : YouTubeService
    {
        public HistoryResource History { get; }
        public WatchLaterResource WatchLater { get; }
        public DashManifestsResource DashManifests { get; set; }
        // TODO: Add Activities override for recomendations and subscriptions
        
        public YouTubeScraper()
        {
            
        }
    }
}
