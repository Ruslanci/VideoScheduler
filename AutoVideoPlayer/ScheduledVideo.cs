using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoVideoPlayer
{
    public class ScheduledVideo
    {
        public string VideoPath { get; set; }
        public DateTime ScheduledTime { get; set; }

        public bool IsPlaying { get; set; }

        public decimal TimesToRepeat { get; set; }

        public override string ToString()
        {
            return $"Видео: {Path.GetFileName(VideoPath)}. Время: {ScheduledTime.ToString("yyyy-MM-dd HH:mm")}. Повторов: {TimesToRepeat}";
        }
    }
}
