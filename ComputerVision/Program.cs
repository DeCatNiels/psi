using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Psi;
using Microsoft.Psi.CognitiveServices.Vision;
using Microsoft.Psi.Imaging;
using Microsoft.Psi.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerVision
{
    class Program
    {
        private static string subscriptionKey = "fffa68a1025f48ebbfae1b5f19e1397e";
        private static readonly string objectDetectionEndpoint = "https://roomextractioncv.cognitiveservices.azure.com/";

        private static string storeLocation = @"psiStore";
        //private static string videoPath = @"E:\videos\Kamera\Team 01\MAH00096.MP4";
        private static string videoPath = @"E:\videos\Action Cam\Team 11\2020_0120_093453_003A.MOV";
        //private static string videoPath = @"E:\videos\facedetectionTest.mp4";


        static void Main(string[] args)
        {
            using (var p = Pipeline.Create("objectDetection"))
            {
                Console.WriteLine("streaming video and detected objects to store\n ...");
                var store = Store.Create(p, "storedObjectDetection", storeLocation);

                if (!File.Exists(videoPath))
                {
                    Console.WriteLine("this file does not exist!");
                }

                MediaSource ms = new MediaSource(p, videoPath);
                //MediaSource ms2 = ms.DeepClone();
                //ms.Image.Write("image", store);

                ImageAnalyzerConfiguration objectDetectionConfig = new ImageAnalyzerConfiguration(subscriptionKey, "trial")
                {
                    VisualFeatures = new List<VisualFeatureTypes>() {
                        VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                        VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                        VisualFeatureTypes.Tags,
                        VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                        VisualFeatureTypes.Objects
                    }.ToArray()
                };
                ImageAnalyzer objectDetector = new ImageAnalyzer(p, objectDetectionConfig);

                ms.Image.Delay(TimeSpan.FromMilliseconds(200)).Process<Shared<Image>, Shared<Image>>(
                    (x, e, o) =>
                    {
                        if (e.SequenceId % 120 == 0)
                        {
                            o.Post(x, e.OriginatingTime);
                        }
                    }).PipeTo(objectDetector.In);

                //objectDetector.Out.Write("computerVision response", store);

                //ObjectVisualizer objVis = new ObjectVisualizer(p);

                FrameInfoAsString analyseComp = new FrameInfoAsString(p, "Test001");

                var imageVisData = objectDetector.Out.Join(ms.Image);

                imageVisData.Out.Item2().PipeTo(analyseComp.Image1In);
                imageVisData.Out.Item1().PipeTo(analyseComp.DetectedObjectsIn);

                //objVis.Out.Write("detected objects in Image", store);

                //FaceVisualizer faceVis = new FaceVisualizer(p);

                //imageVisData.Out.Item2().PipeTo(faceVis.ImageIn);
                //imageVisData.Out.Item1().PipeTo(faceVis.DetectedFacesIn);

                //faceVis.Out.Write("detected faces in Image", store);

                //imageVisData.Item1().Do(
                //    (x) =>
                //    {
                //        Console.WriteLine("--- categories ---");
                //        foreach (Category cat in x.Categories)
                //        {
                //            Console.WriteLine(cat.Name + " - " + cat.Detail + " - " + cat.Score + "\n");
                //        }
                //        Console.WriteLine("\n");

                //        Console.WriteLine("--- color ---");
                //        Console.WriteLine("accent color: " + x.Color.AccentColor);
                //        Console.WriteLine("accent color: " + x.Color.DominantColorBackground);
                //        Console.WriteLine("accent color: " + x.Color.DominantColorForeground);
                //        Console.WriteLine(" -- dominant colors: -- ");
                //        foreach (string col in x.Color.DominantColors)
                //        {
                //            Console.WriteLine(" - " + col + "\n");
                //        }
                //        Console.WriteLine("\n");

                //        Console.WriteLine("--- description ---");
                //        Console.WriteLine(" -- captions: -- ");
                //        foreach (ImageCaption cap in x.Description.Captions)
                //        {
                //            Console.WriteLine(cap.Text + ": " + cap.Confidence + "\n");
                //        }

                //        Console.WriteLine(" -- tags: -- ");
                //        foreach (string tag in x.Description.Tags)
                //        {
                //            Console.WriteLine(" - " + tag + "\n");
                //        }
                //        Console.WriteLine("\n");
                //    });

                p.PipelineCompleted += P_PipelineCompleted;

                p.Run();

                return;

            }
        }

        private static void P_PipelineCompleted(object sender, PipelineCompletedEventArgs e)
        {
            Console.WriteLine("Done!");
            Console.ReadKey(true);
        }
    }
}