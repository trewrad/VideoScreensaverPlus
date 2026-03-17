using Microsoft.Win32;
using System.Windows;

namespace VideoScreensaver
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();
        }

        // Fill out fields from current config
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var options = Config.ReadConfig();
            VideoPathTextbox.Text = options.VideoPaths != null ? string.Join(";", options.VideoPaths) : "";
            VolumeSlider.Value = options.Volume * 100;
            if (options.StretchMode == "Fill") StretchComboBox.SelectedIndex = 0;
            else StretchComboBox.SelectedIndex = 1;
            LoggingCheckBox.IsChecked = options.EnableLogging;
        }

        // Save changes to config
        private void ConfirmConfig(object sender, RoutedEventArgs e)
        {
            var paths = new System.Collections.Generic.List<string>(VideoPathTextbox.Text.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
            string stretch = StretchComboBox.SelectedIndex == 0 ? "Fill" : "UniformToFill";
            Config.WriteConfig(paths, VolumeSlider.Value / 100, stretch, LoggingCheckBox.IsChecked == true);
            Close();
        }

        // Discard changes
        private void CancelConfig(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Open dialog and select file
        private void BrowseVideo(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.CheckFileExists = true;
            open.CheckPathExists = true;
            open.Multiselect = true;
            open.ShowDialog();
            if (open.FileNames.Length > 0)
                VideoPathTextbox.Text = string.Join(";", open.FileNames);
        }

        
    }
}
