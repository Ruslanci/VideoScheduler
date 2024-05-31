using System;
using System.IO;

namespace AutoVideoPlayer
{
    public class ScheduledVideo
    {
        public string VideoPath { get; set; }
        public DateTime ScheduledTime { get; set; }

        public bool IsPlaying { get; set; }

        public int TimesToRepeat { get; set; }

        public TimeSpan VideoDuration { get; set; }

        public DateTime EndTime
        {
            get
            {
                return ScheduledTime.Add(TimeSpan.FromTicks(VideoDuration.Ticks * (long)TimesToRepeat));
            }
        }

        public override string ToString()
        {
            return $"Видео: {Path.GetFileName(VideoPath)}\nВремя начала: {ScheduledTime:yyyy-MM-dd HH:mm:00}\nВремя окончания: {EndTime:yyyy-MM-dd HH:mm:ss}\nПовторов: {TimesToRepeat}\n";
        }
    }
}
