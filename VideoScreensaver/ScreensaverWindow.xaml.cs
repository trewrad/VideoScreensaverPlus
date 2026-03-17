using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace VideoScreensaver
{
    public partial class ScreensaverWindow : Window
    {
        private List<string> _playlist;
        private int _currentVideoIndex = 0;
        private int _screenIndex;

        public ScreensaverWindow(System.Drawing.Rectangle bounds, List<string> playlist, double volume, string stretchMode, int screenIndex)
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.WindowState = WindowState.Normal;
            
            this.Left = bounds.Left;
            this.Top = bounds.Top;
            this.Width = bounds.Width;
            this.Height = bounds.Height;
            
            App.Log(string.Format("Init Window {0}: Left={1}, Top={2}, Width={3}, Height={4}", screenIndex, Left, Top, Width, Height));

            _playlist = playlist ?? new List<string>();
            _screenIndex = screenIndex;

            ScreensaverVideo.Volume = volume;

            if (stretchMode == "Fill")
                ScreensaverVideo.Stretch = Stretch.Fill;
            else if (stretchMode == "UniformToFill")
                ScreensaverVideo.Stretch = Stretch.UniformToFill;
            else
                ScreensaverVideo.Stretch = Stretch.Uniform;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;

            if (_playlist == null || _playlist.Count == 0) return;

            var states = PlaybackState.Load();
            TimeSpan startPosition = TimeSpan.Zero;

            PlaybackState.ScreenState savedState;
            if (states.TryGetValue(_screenIndex, out savedState))
            {
                int index = _playlist.IndexOf(savedState.VideoPath);
                if (index >= 0)
                {
                    _currentVideoIndex = index;
                    startPosition = savedState.Position;
                }
            }

            PlayCurrentVideo(startPosition);
        }

        private void PlayCurrentVideo(TimeSpan position)
        {
            if (_playlist.Count == 0) return;
            
            ScreensaverVideo.Source = new Uri(_playlist[_currentVideoIndex]);
            ScreensaverVideo.Position = position;
            ScreensaverVideo.Play();
        }

        private void PlayNextVideo(object sender, RoutedEventArgs e)
        {
            if (_playlist.Count == 0) return;

            _currentVideoIndex++;
            if (_currentVideoIndex >= _playlist.Count)
            {
                _currentVideoIndex = 0;
            }

            PlayCurrentVideo(TimeSpan.Zero);
        }

        private void SaveCurrentPosition()
        {
            if (_playlist != null && _playlist.Count > 0 && ScreensaverVideo.Source != null)
            {
                PlaybackState.Save(_screenIndex, _playlist[_currentVideoIndex], ScreensaverVideo.Position);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveCurrentPosition();
            base.OnClosed(e);
        }

        private Point mouseLocation;
        private bool InitialMouseSet = true;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (InitialMouseSet) 
            {
                mouseLocation = e.GetPosition(this);
                InitialMouseSet = false;
            }
            else
            {
                if (Math.Abs(mouseLocation.X - e.GetPosition(this).X) > 5 || Math.Abs(mouseLocation.Y - e.GetPosition(this).Y) > 5)
                {
                    SaveCurrentPosition();
                    Application.Current.Shutdown();
                }
                mouseLocation = e.GetPosition(this);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveCurrentPosition();
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            SaveCurrentPosition();
            Application.Current.Shutdown();
        }
    }
}
