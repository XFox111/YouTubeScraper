using System;
using System.Collections.Generic;
using System.Text;

namespace YouTube.Models
{
    public class ClosedCaptionTrack
    {
        public ClosedCaptionInfo Info { get; set; }
        public IReadOnlyList<ClosedCaption> Captions { get; set; }

        public class ClosedCaption
        {
            public TimeSpan Offset { get; set; }
            public TimeSpan Duration { get; set; }
            public string Content { get; set; }
        }
    }
}
