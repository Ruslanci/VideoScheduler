using System;
using System.IO;
using System.Windows.Forms;

namespace AutoVideoPlayer
{
    public partial class MainForm : Form
    {
        private Timer checkTimeTimer;
        private VideoScheduler videoScheduler;
        private VideoLabelManager videoLabelManager;
        private string saveFilePath = "scheduledVideos.json";

        public MainForm()
        {
            InitializeComponent();
            InitializeMainForm();
        }

        public void InitializeMainForm()
        {
            videoScheduler = new VideoScheduler(saveFilePath);
            videoScheduler.OnPlayFile += PlayFile;
            videoScheduler.OnMediaError += HandleMediaError;

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
                ScheduledVideo video = videoScheduler.AddVideo(videoPath, chosenTime, (int)numberOfPlays);

                videoLabelManager.AddLabel(video.ToString());
            }
        }

        private void PlayFile(string url)
        {
            axWindowsMediaPlayer1.URL = url;
            axWindowsMediaPlayer1.Ctlcontrols.play();
            this.TopMost = true;
        }

        private void HandleMediaError(string message)
        {
            MessageBox.Show(message);
            this.Close();
        }

        private void Player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.fullScreen = true;
            }
            else if (e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                videoScheduler.HandleMediaEnd();
            }
        }

        private void Player_MediaError(object sender, AxWMPLib._WMPOCXEvents_MediaErrorEvent e)
        {
            videoScheduler.HandleMediaError();
        }

        private void CheckTimeTimer_Tick(object sender, EventArgs e)
        {
            videoScheduler.CheckAndPlayVideos();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.fullScreen = true;
            }
        }

        private void LoadScheduledVideos()
        {
            foreach (var video in videoScheduler.ScheduledVideos)
            {
                videoLabelManager.AddLabel(video.ToString());
            }
        }
    }
}
