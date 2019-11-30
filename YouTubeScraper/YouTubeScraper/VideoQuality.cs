using System;
using System.Collections.Generic;
using System.Text;

namespace YouTube
{
    public static class VideoQuality
    {
        public static string Auto => QualityConstants.Auto;
        public static string Low144 => QualityConstants.Low144;
        public static string Low240 => QualityConstants.Low240;
        public static string Medium360 => QualityConstants.Medium360;
        public static string Meduim480 => QualityConstants.Meduim480;
        public static string High720 => QualityConstants.High720;
        public static string High720p60 => QualityConstants.High720p60;
        public static string High1080 => QualityConstants.High1080;
        public static string High1080p60 => QualityConstants.High1080p60;
        public static string High1440 => QualityConstants.High1440;
        public static string High2160 => QualityConstants.High2160;
        public static string High2880 => QualityConstants.High2880;
        public static string High3072 => QualityConstants.High3072;
        public static string High4320 => QualityConstants.High4320;

        public static class QualityConstants
        {
            public const string Auto = "auto";
            public const string Low144 = "144p";
            public const string Low240 = "240p";
            public const string Medium360 = "360p";
            public const string Meduim480 = "480p";
            public const string High720 = "720p";
            public const string High720p60 = "720p60";
            public const string High1080 = "1080p";
            public const string High1080p60 = "1080p60";
            public const string High1440 = "1440p";
            public const string High2160 = "2160p";
            public const string High2880 = "2880p";
            public const string High3072 = "3072p";
            public const string High4320 = "4320p";
        }
    }
}
