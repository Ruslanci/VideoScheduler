using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;
using WMPLib;

namespace AutoVideoPlayer
{
    public partial class MainForm : Form
    {

        private Timer checkTimeTimer;
        private Queue<ScheduledVideo> scheduledVideos;
        private ScheduledVideo currentVideo;
        private VideoLabelManager videoLabelManager;
        private string saveFilePath = "scheduledVideos.json";


        public MainForm()
        {
            InitializeComponent();
            InitializeMainForm();
        }

        public void InitializeMainForm()
        {
            scheduledVideos = new Queue<ScheduledVideo>();

            checkTimeTimer = new Timer();
            checkTimeTimer.Interval = 1000;
            checkTimeTimer.Tick += CheckTimeTimer_Tick;
            checkTimeTimer.Start();

            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.PlayStateChange += Player_PlayStateChange;
            axWindowsMediaPlayer1.MediaError += Player_MediaError;
            this.Activated += Form1_Activated;

            videoLabelManager = new VideoLabelManager(this, 10, 0, 70);

            LoadScheduledVideos();
        }

        private void chooseVideoButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video Files|*.mp4;*.avi;*.mkv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DateTime chosenTime;
                decimal numberOfPlays;
                using (var form = new AddVideoForm())
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        chosenTime = form.ChosenTime;
                        numberOfPlays = form.NumberOfPlays;
                    }
                    else
                    {
                        return;
                    }
                }


                string videoPath = ofd.FileName;
                TimeSpan videoDuration = GetVideoDuration(videoPath);

                ScheduledVideo scheduledVideo = new ScheduledVideo
                {
                    VideoPath = videoPath,
                    ScheduledTime = chosenTime,
                    TimesToRepeat = (int)numberOfPlays,
                    VideoDuration = videoDuration,
                    IsPlaying = false
                };
                scheduledVideos.Enqueue(scheduledVideo);
                SaveScheduledVideos();


                Label videoLabel = new Label();
                videoLabel.Text = scheduledVideo.ToString();
                videoLabel.AutoSize = true;
                videoLabel.Location = new Point(chooseVideoButton.Bottom, chooseVideoButton.Bottom + 15);


                videoLabelManager.AddLabel(scheduledVideo.ToString());
            }
        }



        private TimeSpan GetVideoDuration(string videoPath)
        {
            if (string.IsNullOrEmpty(videoPath))
            {
                throw new ArgumentException("Video path cannot be null or empty.", nameof(videoPath));
            }

            try
            {
                WindowsMediaPlayer wmp = new WindowsMediaPlayer();
                IWMPMedia mediaInfo = wmp.newMedia(videoPath);
                double durationInSeconds = mediaInfo.duration;

                return TimeSpan.FromSeconds(durationInSeconds);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving the video duration: {ex.Message}", ex);
            }
        }

        private void PlayFile(string url)
        {
            axWindowsMediaPlayer1.URL = url;
            axWindowsMediaPlayer1.Ctlcontrols.play();
            this.TopMost = true;
        }

        private void Player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.fullScreen = true;
            }
            else if (e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                if (currentVideo != null && currentVideo.TimesToRepeat > 0)
                {
                    currentVideo.TimesToRepeat--;
                    if (currentVideo.TimesToRepeat > 0)
                    {
                        axWindowsMediaPlayer1.Ctlcontrols.play();
                    }
                    else
                    {
                        currentVideo = null;
                        scheduledVideos.Dequeue();
                    }
                }
            }
        }

        private void Player_MediaError(object sender, AxWMPLib._WMPOCXEvents_MediaErrorEvent e)
        {
            MessageBox.Show("Ошибка при проигрывании медиафайла.");
            this.Close();
        }

        private void CheckTimeTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            foreach (var scheduledVideo in scheduledVideos)
            {
                if (scheduledVideo.TimesToRepeat <= 0)
                {

                }
                if (scheduledVideo.ScheduledTime.TimeOfDay.Minutes <= now.TimeOfDay.Minutes && scheduledVideo.IsPlaying == false)
                {
                    PlayFile(scheduledVideo.VideoPath);
                    scheduledVideo.IsPlaying = true;
                    break;
                }
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.fullScreen = true;
            }
        }

    

        private void SaveScheduledVideos()
        {
            var scheduledVideosList = scheduledVideos.ToList();
            string json = JsonConvert.SerializeObject(scheduledVideosList, Formatting.Indented);
            File.WriteAllText(saveFilePath, json);
        }

        private void LoadScheduledVideos()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                var scheduledVideosList = JsonConvert.DeserializeObject<List<ScheduledVideo>>(json);

                var validVideos = scheduledVideosList.Where(video => video.EndTime > DateTime.Now).ToList();
                scheduledVideos = new Queue<ScheduledVideo>(validVideos);

                foreach (var video in scheduledVideos)
                {
                    videoLabelManager.AddLabel(video.ToString());
                }
            }
        }

    }
}
