using AxWMPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoVideoPlayer
{
    public partial class Form1 : Form
    {

        private Timer checkTimeTimer;
        private Queue<ScheduledVideo> scheduledVideos;
        private ScheduledVideo currentVideo;


        public Form1()
        {
            InitializeComponent();

            scheduledVideos = new Queue<ScheduledVideo>();

            checkTimeTimer = new Timer();
            checkTimeTimer.Interval = 1000; // Check every second
            checkTimeTimer.Tick += CheckTimeTimer_Tick;
            checkTimeTimer.Start();

            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.PlayStateChange += Player_PlayStateChange;
            axWindowsMediaPlayer1.MediaError += Player_MediaError;
            this.Activated += Form1_Activated;
            this.Deactivate += Form1_Deactivate;
        }

        private void chooseVideoButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video Files|*.mp4;*.avi;*.mkv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Prompt user to set the time
                DateTime chosenTime;
                decimal numberOfPlays;
                using (var form = new SetTimeForm())
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        chosenTime = form.ChosenTime;
                        numberOfPlays = form.NumberOfPlays;
                    }
                    else
                    {
                        // User cancelled, handle accordingly
                        return;
                    }
                }


                // Now you have the chosen video path and time
                string videoPath = ofd.FileName;

                ScheduledVideo scheduledVideo = new ScheduledVideo
                {
                    VideoPath = videoPath, // Assuming videoPath is set by chooseVideoButton_Click
                    ScheduledTime = chosenTime,
                    TimesToRepeat = numberOfPlays,
                    IsPlaying = false
                };
                scheduledVideos.Enqueue(scheduledVideo);


                Label videoLabel = new Label();
                videoLabel.Text = scheduledVideo.ToString();
                videoLabel.AutoSize = true;
                videoLabel.Location = new Point(chooseVideoButton.Bottom, chooseVideoButton.Bottom + 15);


                // Add labels to the form
                this.Controls.Add(videoLabel);
            }
        }



        private void PlayFile(string url)
        {
            axWindowsMediaPlayer1.URL = url;
            axWindowsMediaPlayer1.Ctlcontrols.play();
            this.TopMost = true;
        }

        private void PlayNextVideo()
        {
            if (scheduledVideos.Count > 0)
            {
                currentVideo = scheduledVideos.Peek();
                currentVideo.IsPlaying = true;
                PlayFile(currentVideo.VideoPath);
            }
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
                // Compare only the time part
                if (scheduledVideo.ScheduledTime.TimeOfDay.Minutes <= now.TimeOfDay.Minutes && scheduledVideo.IsPlaying == false)
                {
                    PlayFile(scheduledVideo.VideoPath);
                    scheduledVideo.IsPlaying = true;
                    break; // Play only one video at a time
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

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.fullScreen = true;
            }
        }
    }
}
