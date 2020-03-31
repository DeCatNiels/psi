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

        private static string storeLocation = @"E:\PSIVideoStores\computerVision";
        //private static string videoPath = @"E:\videos\Kamera\Team 01\MAH00096.MP4";
        private static string videoPath = @"E:\videos\Action Cam\Team 01\2020_0110_100102_003A.MOV";
        //private static string videoPath = @"E:\videos\snippedVersion.mp4";


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
                ms.Image.Write("image", store);

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

                ms.Image.Delay(TimeSpan.FromMilliseconds(1000)).Process<Shared<Image>, Shared<Image>>(
                    (x, e, o) =>
                    {
                        if (e.SequenceId % 2 == 0)
                        {
                            //Console.WriteLine("sequenceId: " + e.SequenceId);
                            //Console.WriteLine("Process pipe to object detection: " + e.OriginatingTime);
                            o.Post(x, e.OriginatingTime);

                        }
                    }).PipeTo(objectDetector.In);


                //ms.Image.PipeTo(objectDetector.In, DeliveryPolicy.LatestMessage);
                objectDetector.Out.Write("computerVision response", store);
                //objectDetector.Out.Do((t, e) => { if (t != null) Console.WriteLine("detected objects: " + t.Description.Captions[0].Text); else Console.WriteLine("nothing was returned"); });

                ObjectVisualizer objVis = new ObjectVisualizer(p);

                var imageVisData = objectDetector.Out.Join(ms.Image);

                imageVisData.Out.Item2().PipeTo(objVis.ImageIn);
                imageVisData.Out.Item1().PipeTo(objVis.DetectedObjectsIn);

                //imageVisData.Out.PipeTo(objVis.In);

                //imageVisData.Out.Process<Tuple<ImageAnalysis, Shared<Image>>, Shared<Image>>(
                //    (x, e, o) =>
                //    {
                //        var (analyseRes, image) = x;
                //        o.Post(image, e.OriginatingTime);
                //    }).PipeTo(objVis.ImageIn);

                //objectDetector.Out.Process<ImageAnalysis, ImageAnalysis>(
                //    (x,e,o) =>
                //    {
                //        Console.WriteLine("sequenceId imageAnalisis going to draw: " + e.SequenceId);
                //        Console.WriteLine("Process pipe imageAnalisis to draw: " + e.OriginatingTime);
                //        o.Post(x, e.OriginatingTime);
                //    }).PipeTo(objVis.DetectedObjectsIn);

                //ms.Image.Process<Shared<Image>, Shared<Image>>(
                //    (x, e, o) =>
                //    {
                //        Console.WriteLine("sequenceId Shared image going to draw: " + e.SequenceId);
                //        Console.WriteLine("Process pipe Shared image to draw: " + e.OriginatingTime);
                //        o.Post(x, e.OriginatingTime);
                //    }).PipeTo(objVis.ImageIn);

                objVis.Out.Write("detected objects in Image", store);

                p.PipelineCompleted += P_PipelineCompleted;

                p.Run();

                return;

            }
        }

        private static void P_PipelineCompleted(object sender, PipelineCompletedEventArgs e)
        {
            Console.WriteLine("Done!");
        }
    }
}