using System;
using System.Windows;

namespace VideoScreensaver
{
    // Code adapted from: https://www.harding.edu/fmccown/screensaver/screensaver.html

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void Log(string message)
        {
            try { System.IO.File.AppendAllText("screensaver_log.txt", string.Format("{0}: {1}\n", DateTime.Now, message)); }
            catch { }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log(string.Format("App started with arguments: {0}", (e.Args.Length > 0 ? string.Join(" ", e.Args) : "none")));
            Config.CheckConfig();
            if (e.Args.Length > 0)
            {
                string firstArgument = e.Args[0].ToLower().Trim();
                string secondArgument = null;

                // Handle cases where arguments are separated by colon.
                // Examples: /c:1234567 or /P:1234567
                if (firstArgument.Length > 2)
                {
                    secondArgument = firstArgument.Substring(3).Trim();
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (e.Args.Length > 1)
                {
                    secondArgument = e.Args[1];
                }

                if (firstArgument == "/c")      // Configuration mode
                {
                    ConfigurationWindow ConfigWindow = new ConfigurationWindow();
                    ConfigWindow.Closed += CloseApplication;
                    ConfigWindow.Show();
                }
                else if (firstArgument == "/p") // Preview mode
                {
                    Current.Shutdown(); // Not worried about implementing preview mode.
                }
                else if (firstArgument == "/s") // Full-screen mode
                {
                    var options = Config.ReadConfig();
                    var allScreens = System.Windows.Forms.Screen.AllScreens;
                    int screenCount = allScreens.Length;
                    var videoPaths = options.VideoPaths ?? new System.Collections.Generic.List<string>();

                    for (int screenIndex = 0; screenIndex < screenCount; screenIndex++)
                    {
                        var screen = allScreens[screenIndex];
                        var playlistForScreen = new System.Collections.Generic.List<string>();

                        if (videoPaths.Count > 0)
                        {
                            int videosPerScreen = (int)Math.Ceiling((double)videoPaths.Count / screenCount);
                            int startIndex = screenIndex * videosPerScreen;

                            for (int i = 0; i < videosPerScreen; i++)
                            {
                                if (startIndex + i < videoPaths.Count) playlistForScreen.Add(videoPaths[startIndex + i]);
                            }

                            if (playlistForScreen.Count == 0) playlistForScreen.Add(videoPaths[screenIndex % videoPaths.Count]);
                        }

                        ScreensaverWindow FullScreensaver = new ScreensaverWindow(screen.Bounds, playlistForScreen, options.Volume, options.StretchMode, screenIndex);
                        Log(string.Format("Creating window for screen {0} at Left: {1}, Top: {2}, Width: {3}, Height: {4}", screenIndex, screen.Bounds.Left, screen.Bounds.Top, screen.Bounds.Width, screen.Bounds.Height));
                        FullScreensaver.Closed += CloseApplication;
                        FullScreensaver.Show();
                        Log(string.Format("Window for screen {0} shown.", screenIndex));
                    }
                }
                else    // Undefined argument
                {
                    MessageBox.Show("Sorry, but the command line argument \"" + firstArgument + "\" is not valid.", "VideoScreensaver", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else    // No arguments, treat like /c
            {
                ConfigurationWindow ConfigWindow = new ConfigurationWindow();
                ConfigWindow.Closed += CloseApplication;
                ConfigWindow.Show();
            }
        }

        private void CloseApplication(object sender, EventArgs e)
        {
            Current.Shutdown();
        }
    }
}
