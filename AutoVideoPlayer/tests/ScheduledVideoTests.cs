using NUnit.Framework;
using System;

namespace VideoScheduler
{
    [TestFixture]
    public class ScheduledVideoTests
    {
        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            var scheduledVideo = new ScheduledVideo
            {
                VideoPath = "video.mp4",
                ScheduledTime = new DateTime(2024, 6, 1, 12, 0, 0),
                TimesToRepeat = 3
            };

            string result = scheduledVideo.ToString();

            Assert.Equals("Видео: video.mp4. Время: 2024-06-01 12:00. Повторов: 3", result);
        }

        [Test]
        public void IsPlaying_SetToTrue_IsPlayingIsTrue()
        {
            var scheduledVideo = new ScheduledVideo();

            scheduledVideo.IsPlaying = true;

            Assert.Equals(scheduledVideo.IsPlaying, true);
        }

        [Test]
        public void IsPlaying_SetToFalse_IsPlayingIsFalse()
        {
            var scheduledVideo = new ScheduledVideo { IsPlaying = true };

            scheduledVideo.IsPlaying = false;

            Assert.Equals(scheduledVideo.IsPlaying, false);
        }
    }
}
