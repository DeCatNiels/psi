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
using ComputerVision;
using System.Threading;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Psi;
using Microsoft.Psi.CognitiveServices.Vision;
using Microsoft.Psi.Imaging;
using Microsoft.Psi.Media;
using Path = System.IO.Path;

namespace CompareData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Progress progress;

        private string apiKey = "f45886abb1e54abfad5ce83a4dbdca1b";
        private string apiEndpoint = "https://videoanalyzing.cognitiveservices.azure.com/";
        private string apiRegion = "WestEurope";
        private string storeLocation = @"C:\Users\11702349\Documents\3tin\stage\psiStroreRoomData";
        //private string storeLocation = @"E:\PSIVideoStores\computerVision";
        public MainWindow()
        {
            InitializeComponent();
            this.videos = new List<string>();
            this.storeCouples = new List<Tuple<string, string>>();
            //this.cognitiveServiceHandler = new CognitiveServiceHandler(apiKey, apiEndpoint, apiRegion, storeLocation);
            this.progress = new Progress();
            this.UpdateUI();
        }

        private List<string> videos;
        private List<Tuple<string, string>> storeCouples;
        private CognitiveServiceHandler cognitiveServiceHandler;

        private void Video1_Button_Click(object sender, RoutedEventArgs e)
        {
            string videoPath = GetVideoPath();

            if (videoPath != null)
            {
                videos.Add(videoPath);
                progress.Next();
                this.UpdateUI();
            }
        }

        private void Analyze_Button_Click(object sender, RoutedEventArgs e)
        {
            progress.Next();
            this.UpdateUI();

            Console.WriteLine("analyzing...");
            Thread.Sleep(2000);

            CognitiveServiceHandler cognitiveServiceHandler = new CognitiveServiceHandler(apiKey, apiEndpoint, apiRegion, storeLocation);
            List<CognitiveServices> cognitiveServices = new List<CognitiveServices>() { CognitiveServices.ObjectDetection };

            string storeNameVid1 = "objectDetection" + Path.GetFileName(videos[0]);
            string storeNameVid2 = "objectDetection" + Path.GetFileName(videos[1]);

            cognitiveServiceHandler.RunCognitiveServices(cognitiveServices, videos, storeName: storeNameVid1);
            Console.WriteLine("computer vision done!");

            cognitiveServiceHandler = new CognitiveServiceHandler(apiKey, apiEndpoint, apiRegion, storeLocation);
            var videosHelper = videos[0];
            videos[0] = videos[1];
            videos[1] = videosHelper;
            cognitiveServiceHandler.RunCognitiveServices(cognitiveServices, videos, storeName: storeNameVid2);

            storeCouples.Add(Tuple.Create(storeNameVid1, storeNameVid2));

            progress.Next();
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

            addVideo_Button.IsEnabled = progress.NeedMoreFiles;
            analyze_Button.IsEnabled = !progress.NeedMoreFiles;
        }

        private void compare_Button_Click(object sender, RoutedEventArgs e)
        {
            CognitiveServiceHandler cognitiveServiceHandler = new CognitiveServiceHandler(apiKey, apiEndpoint, apiRegion, storeLocation);

            //cognitiveServiceHandler.CompareVideoAnalasis(storeCouples[0].Item1, storeCouples[0].Item2);
            //cognitiveServiceHandler.CompareVideoAnalasis("objectDetectionsnippedSpeechTest.mp4", "objectDetectionsnippedVersion.mp4");
            //cognitiveServiceHandler.CompareVideoAnalasis("2vid object detection", new List<string>() { "2020_0110_100102_003A.MOV", "2020_0110_142146_003A.MOV" });
            cognitiveServiceHandler.CompareVideoAnalasis("objectDetection2020_0110_100102_003A.MOV", "objectDetection2020_0110_142146_003A.MOV");
            //cognitiveServiceHandler.CompareVideoAnalasis("objectDetection2020_0115_122025_003A.MOV", "objectDetection2020_0120_093453_003A.MOV");
        }
    }
}
