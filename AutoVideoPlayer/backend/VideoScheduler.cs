using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WMPLib;

namespace VideoScheduler
{
    public class VideoScheduler
    {
        public Queue<ScheduledVideo> ScheduledVideos { get; private set; }
        private ScheduledVideo currentVideo;
        private string saveFilePath;
        private WindowsMediaPlayer wmp;

        public event Action<string> OnPlayFile;
        public event Action<string> OnMediaError;

        public VideoScheduler(string saveFilePath)
        {
            this.saveFilePath = saveFilePath;
            ScheduledVideos = new Queue<ScheduledVideo>();
            wmp = new WindowsMediaPlayer();
            LoadScheduledVideos();
        }

        public ScheduledVideo AddVideo(string videoPath, DateTime scheduledTime, int timesToRepeat)
        {
            TimeSpan videoDuration = GetVideoDuration(videoPath);

            ScheduledVideo scheduledVideo = new ScheduledVideo
            {
                VideoPath = videoPath,
                ScheduledTime = scheduledTime,
                TimesToRepeat = timesToRepeat,
                VideoDuration = videoDuration,
                IsPlaying = false
            };

            ScheduledVideos.Enqueue(scheduledVideo);
            SaveScheduledVideos();

            return scheduledVideo;
        }

        public void CheckAndPlayVideos()
        {
            DateTime now = DateTime.Now;

            foreach (var scheduledVideo in ScheduledVideos)
            {
                if (scheduledVideo.TimesToRepeat <= 0) continue;

                if (scheduledVideo.ScheduledTime.TimeOfDay.Minutes <= now.TimeOfDay.Minutes && scheduledVideo.IsPlaying == false)
                {
                    PlayFile(scheduledVideo.VideoPath);
                    scheduledVideo.IsPlaying = true;
                    break;
                }
            }
        }

        private TimeSpan GetVideoDuration(string videoPath)
        {
            if (string.IsNullOrEmpty(videoPath))
            {
                throw new ArgumentException("Путь до видео не может быть пустым.", nameof(videoPath));
            }

            try
            {
                IWMPMedia mediaInfo = wmp.newMedia(videoPath);
                double durationInSeconds = mediaInfo.duration;

                return TimeSpan.FromSeconds(durationInSeconds);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при получении длины видео: {ex.Message}", ex);
            }
        }

        private void PlayFile(string url)
        {
            OnPlayFile?.Invoke(url);
        }

        public void HandleMediaEnd()
        {
            if (currentVideo != null && currentVideo.TimesToRepeat > 0)
            {
                currentVideo.TimesToRepeat--;
                if (currentVideo.TimesToRepeat > 0)
                {
                    PlayFile(currentVideo.VideoPath);
                }
                else
                {
                    currentVideo = null;
                    ScheduledVideos.Dequeue();
                }
            }
        }

        public void HandleMediaError()
        {
            OnMediaError?.Invoke("Ошибка при проигрывании медиафайла.");
        }

        private void SaveScheduledVideos()
        {
            var scheduledVideosList = ScheduledVideos.ToList();
            string json = JsonConvert.SerializeObject(scheduledVideosList, Formatting.Indented);
            File.WriteAllText(saveFilePath, json);
        }

        private void LoadScheduledVideos()
        {
            if (File.Exists(saveFilePath))
            {
                try
                {
                    string json = File.ReadAllText(saveFilePath);
                    var scheduledVideosList = JsonConvert.DeserializeObject<List<ScheduledVideo>>(json);

                    if (scheduledVideosList != null)
                    {
                        var validVideos = scheduledVideosList.Where(video => video.EndTime > DateTime.Now).ToList();
                        ScheduledVideos = new Queue<ScheduledVideo>(validVideos);
                    }
                    else
                    {
                        ScheduledVideos = new Queue<ScheduledVideo>();
                    }
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"Error parsing JSON: {jsonEx.Message}");
                    ScheduledVideos = new Queue<ScheduledVideo>();
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine($"Error reading file: {ioEx.Message}");
                    ScheduledVideos = new Queue<ScheduledVideo>();
                }
            }
        }
    }
}
