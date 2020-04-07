using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompareData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Progress progress;
        public MainWindow()
        {
            InitializeComponent();
            this.progress = new Progress();
            this.UpdateUI();
        }

        private string[] videos;

        private void Video1_Button_Click(object sender, RoutedEventArgs e)
        {
            string videoPath = GetVideoPath();

            if (videoPath != null)
            {
                videos.Append(videoPath);
                progress.AddFile();
                this.UpdateUI();
            }
        }

        private void Analyze_Button_Click(object sender, RoutedEventArgs e)
        {
            progress.AddFile();
            this.UpdateUI();
        }

        private string GetVideoPath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Video files (*.mp4; *.mov)|*.mp4;*.mov";
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;

            return null;
        }

        private void UpdateUI()
        {
            progress_Label.Content = progress.ProgressState.GetDiscription();

            addVideo_Button.IsEnabled = progress.ProgressState.NeedMoreFiles;
            analyze_Button.IsEnabled = !progress.ProgressState.NeedMoreFiles;
        }
    }
}
