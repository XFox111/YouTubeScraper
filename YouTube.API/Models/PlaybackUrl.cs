namespace YouTube.Models
{
	public enum VideoFormat
	{
		/// <summary>
		/// MPEG-4 Part 2.
		/// </summary>
		Mp4V = 0,
		H263 = 1,
		/// <summary>
		/// MPEG-4 Part 10, H264, Advanced Video Coding (AVC).
		/// </summary>
		H264 = 2,
		Vp8 = 3,
		Vp9 = 4,
		Av1 = 5
	}
	public enum AudioFormat
	{
		/// <summary>
		/// MPEG-4 Part 3, Advanced Audio Coding (AAC).
		/// </summary>
		Aac = 0,
		Vorbis = 1,
		Opus = 2
	}
	public enum AudioQuality { Low, Medium, High }

	public class VideoPlaybackUrl
	{
		public string Quality { get; set; }
		public VideoFormat Format { get; set; }
		public string Url { get; set; }
		public System.Drawing.Size Resolution { get; set; }
		public bool HasAudio { get; set; }
		public int Bitrate { get; set; }
	}

	public class AudioPlaybackUrl
	{
		public AudioQuality Quality { get; set; }
		public AudioFormat Format { get; set; }
		public string Url { get; set; }
		public int Bitrate { get; set; }
	}
}