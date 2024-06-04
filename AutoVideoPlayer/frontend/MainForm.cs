using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace VideoScheduler
{
    public partial class MainForm : Form
    {
        private Timer checkTimeTimer;
        private VideoScheduler videoScheduler;
        private VideoLabelManager videoLabelManager;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
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

            videoLabelManager = new VideoLabelManager(this, 10, 0, 70);

            LoadScheduledVideos();

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Выйти", null, OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "Video Scheduler";
            trayIcon.Icon = new System.Drawing.Icon("appicon.ico");
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;
            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            this.FormClosing += MainForm_FormClosing;
            this.Resize += MainForm_Resize;
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
            string playerPath = FindVideoPlayerPath();

            if (string.IsNullOrEmpty(playerPath))
            {
                MessageBox.Show("Не удалось найти подходящий видеоплеер для воспроизведения видео.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = playerPath,
                    Arguments = GetPlayerArguments(playerPath, url),
                    UseShellExecute = true
                };

                Process.Start(startInfo);
                this.TopMost = true;
            }
            catch (Exception ex)
            {
                HandleMediaError(ex.Message);
            }
        }

        private string FindVideoPlayerPath()
        {
            // Check for VLC player
            string vlcPath = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
            if (File.Exists(vlcPath))
            {
                return vlcPath;
            }

            vlcPath = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe";
            if (File.Exists(vlcPath))
            {
                return vlcPath;
            }

            // Check for Windows Media Player
            string wmpPath = Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Windows Media Player\wmplayer.exe");
            if (File.Exists(wmpPath))
            {
                return wmpPath;
            }

            wmpPath = Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Windows Media Player\wmplayer.exe");
            if (File.Exists(wmpPath))
            {
                return wmpPath;
            }

            // Add more checks for other players if needed

            return null;
        }

        private string GetPlayerArguments(string playerPath, string videoPath)
        {
            if (playerPath.ToLower().Contains("vlc"))
            {
                return $"--fullscreen \"{videoPath}\"";
            }
            else if (playerPath.ToLower().Contains("wmplayer"))
            {
                return $"\"{videoPath}\" /fullscreen";
            }

            // Add more arguments for other players if needed

            return $"\"{videoPath}\"";
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

        private void LoadScheduledVideos()
        {
            foreach (var video in videoScheduler.ScheduledVideos)
            {
                videoLabelManager.AddLabel(video.ToString());
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            trayIcon.ShowBalloonTip(1000, "Video Scheduler", "Приложение свернуто в трей.", ToolTipIcon.Info);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                trayIcon.ShowBalloonTip(1000, "Video Scheduler", "Приложение свернуто в трей.", ToolTipIcon.Info);
            }
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
    }
}
