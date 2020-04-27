using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Psi;
using Microsoft.Psi.CognitiveServices.Vision;
using Microsoft.Psi.Imaging;
using Microsoft.Psi.Media;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerVision
{
    public class CognitiveServiceHandler
    {
        public string ApiKey { get; private set; }
        public string ApiEndpoint { get; private set; }
        public string StoreLocation { get; private set; }
        public string Region { get; private set; }

        public double PercentOfFramesAnalyzing { get; private set; }


        private VisualFeatureTypes[] defaultVisualFeatureTypes = new List<VisualFeatureTypes>() 
                    {
                        VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                        VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                        VisualFeatureTypes.Tags,
                        VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                        VisualFeatureTypes.Objects
                    }.ToArray();

        public CognitiveServiceHandler(string apiKey, string apiEndpoint, string region, string resultStoreLocation)
        {
            ApiKey = apiKey;
            ApiEndpoint = apiEndpoint;
            StoreLocation = resultStoreLocation;
            Region = region;
            PercentOfFramesAnalyzing = 0.02;
        }

        public void RunCognitiveServices(List<CognitiveServices> servicesToUse, List<string> videoPaths, VisualFeatureTypes[] objectDetectionFeatures = null, string storeName = null)
        {
            foreach (CognitiveServices cognitiveService in servicesToUse)
            {
                switch (cognitiveService)
                {
                    case CognitiveServices.Face:
                        Console.WriteLine("face is not implemented yet");
                        break;
                    case CognitiveServices.ObjectDetection:
                        if (objectDetectionFeatures == null)
                        {
                            objectDetectionFeatures = defaultVisualFeatureTypes;
                        }
                        ObjectDetectionRun(objectDetectionFeatures, videoPaths, PercentOfFramesAnalyzing, storeName);
                        break;
                    case CognitiveServices.ObjectDetectionTwoVids:
                        if (objectDetectionFeatures == null)
                        {
                            objectDetectionFeatures = defaultVisualFeatureTypes;
                        }
                        ObjectDetectionOfTwoVids(objectDetectionFeatures, videoPaths, PercentOfFramesAnalyzing, storeName);
                        break;
                    case CognitiveServices.Speech:
                        Console.WriteLine("face is not implemented yet");
                        break;
                    default:
                        Console.WriteLine("there is no service for this: " + cognitiveService.ToString());
                        break;
                }
            }
        }

        public void ObjectDetectionOfTwoVids(VisualFeatureTypes[] objectDetectionFeatures, List<string> videoPaths, double percentageFrames, string storeName = null)
        {
            if (!File.Exists(videoPaths[0]) && !File.Exists(videoPaths[1]))
            {
                Console.WriteLine("this files arent provided!");
                return;
            }
            ShellFile shellFileVid1 = ShellFile.FromFilePath(videoPaths[0]);
            ShellFile shellFileVid2 = ShellFile.FromFilePath(videoPaths[1]);
            int fps = Convert.ToInt32(shellFileVid1.Properties.System.Video.FrameRate.Value / 1000);
            Console.WriteLine("fps: " + fps);
            int azureHandelRate = 1;
            int analyzeEveryXFrames = Convert.ToInt32(fps / azureHandelRate);

            if (storeName == null)
            {
                storeName = "storeObjectDetection" + shellFileVid1.Name + shellFileVid2.Name;
            }

            using (var p = Pipeline.Create("CognitiveServices objectDetection"))
            {
                Console.WriteLine("streaming video and detected objects to store\n ...");
                var store = Store.Create(p, storeName, StoreLocation);


                ImageAnalyzerConfiguration objectDetectionConfig = new ImageAnalyzerConfiguration(ApiKey, Region, ApiEndpoint)
                {
                    VisualFeatures = new List<VisualFeatureTypes>() {
                        VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                        VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                        VisualFeatureTypes.Tags,
                        VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                        VisualFeatureTypes.Objects
                    }.ToArray()
                };

                MediaSource ms1 = new MediaSource(p, videoPaths[0]);
                MediaSource ms2 = new MediaSource(p, videoPaths[1]);
                ms1.Image.Write("image: " + shellFileVid1.Name, store);
                ms2.Image.Write("image: " + shellFileVid2.Name, store);

                ImageAnalyzer objectDetector1 = new ImageAnalyzer(p, objectDetectionConfig);
                ImageAnalyzer objectDetector2 = new ImageAnalyzer(p, objectDetectionConfig);

                ms1.Image.Delay(TimeSpan.FromMilliseconds(200)).Process<Shared<Image>, Shared<Image>>(
                    (x, e, o) =>
                    {
                        if (e.SequenceId % analyzeEveryXFrames == 0)
                        {
                            o.Post(x, e.OriginatingTime);
                            Console.WriteLine("   -----------   " + shellFileVid1.Name + "   ----------   ");
                            Console.WriteLine("sec id: " + e.SequenceId);
                            Console.WriteLine("orig time: " + e.OriginatingTime);
                            Console.WriteLine("created time: " + e.Time);

                        }
                    }).PipeTo(objectDetector1.In);

                ms2.Image.Delay(TimeSpan.FromMilliseconds(200)).Process<Shared<Image>, Shared<Image>>(
                    (x, e, o) =>
                    {
                        if (e.SequenceId % analyzeEveryXFrames == 0)
                        {
                            o.Post(x, e.OriginatingTime);
                            Console.WriteLine("   -----------   " + shellFileVid2.Name + "   ----------   ");
                            Console.WriteLine("sec id: " + e.SequenceId);
                            Console.WriteLine("orig time: " + e.OriginatingTime);
                            Console.WriteLine("created time: " + e.Time);

                        }
                    }).PipeTo(objectDetector2.In);

                objectDetector1.Out.Write("computerVision response: " + shellFileVid1.Name, store);
                objectDetector2.Out.Write("computerVision response: " + shellFileVid2.Name, store);

                p.PipelineCompleted += P_PipelineCompleted;

                p.Run();

                return;

            }
        }

        public void ObjectDetectionRun(VisualFeatureTypes[] objectDetectionFeatures, List<string> videoPaths, double percentageFrames, string storeName = null)
        {
            if (!File.Exists(videoPaths[0]))
            {
                Console.WriteLine("this file does not exist!");
                return;
            }
            ShellFile shellFile = ShellFile.FromFilePath(videoPaths[0]);
            int fps = Convert.ToInt32(shellFile.Properties.System.Video.FrameRate.Value / 1000);
            Console.WriteLine("fps: " + fps);
            int azureHandelRate = 1;
            int analyzeEveryXFrames = Convert.ToInt32(fps / azureHandelRate);

            if(storeName == null)
            {
                storeName = "storeObjectDetection" + shellFile.Name;
            }

            using (var p = Pipeline.Create("CognitiveServices objectDetection"))
            {
                Console.WriteLine("streaming video and detected objects to store\n ...");
                var store = Store.Create(p, storeName, StoreLocation);


                ImageAnalyzerConfiguration objectDetectionConfig = new ImageAnalyzerConfiguration(ApiKey, Region, ApiEndpoint)
                {
                    VisualFeatures = new List<VisualFeatureTypes>() {
                        VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                        VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                        VisualFeatureTypes.Tags,
                        VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                        VisualFeatureTypes.Objects
                    }.ToArray()
                };

                MediaSource ms = new MediaSource(p, videoPaths[0]);
                ms.Image.Write("image", store);

                ImageAnalyzer objectDetector = new ImageAnalyzer(p, objectDetectionConfig);

                ms.Image.Delay(TimeSpan.FromMilliseconds(50)).Process<Shared<Image>, Shared<Image>>(
                    (x, e, o) =>
                    {
                        if (e.SequenceId % analyzeEveryXFrames == 0)
                        {
                            o.Post(x, e.OriginatingTime);
                            Console.WriteLine("sec id: " + e.SequenceId);
                            Console.WriteLine("orig time: " + e.OriginatingTime);
                            Console.WriteLine("created time: " + e.Time);
                            Console.WriteLine("   -----------   ----------   ----------   ");

                        }
                    }).PipeTo(objectDetector.In);

                objectDetector.Out.Write("computerVision response", store);

                p.PipelineCompleted += P_PipelineCompleted;

                p.Run();

                return;

            }
        }

        public void VisualizeObjectDetection()
        {
            using (var p = Pipeline.Create("CognitiveServices visualize objectDetection"))
            {
                Console.WriteLine("visualizing objectdetection\n ...");
                var store = Store.Create(p, "visualizedObjectDetection", StoreLocation);
                var inputStore = Store.Open(p, "storedObjectDetection", StoreLocation);

                var imageStream = inputStore.OpenStream<Shared<Image>>("image");
                var computerVisionStream = inputStore.OpenStream<ImageAnalysis>("computerVision response");

                ObjectVisualizer objVis = new ObjectVisualizer(p);

                var imageVisData = computerVisionStream.Out.Join(imageStream.Out);

                imageVisData.Out.Item2().PipeTo(objVis.ImageIn);
                imageVisData.Out.Item1().PipeTo(objVis.DetectedObjectsIn);

                objVis.Out.Write("image: detected obj", store);

                FaceVisualizer faceVis = new FaceVisualizer(p);

                imageVisData.Out.Item2().PipeTo(faceVis.ImageIn);
                imageVisData.Out.Item1().PipeTo(faceVis.DetectedFacesIn);

                faceVis.Out.Write("detected faces in Image", store);

                p.PipelineCompleted += P_PipelineCompleted;

                p.Run();

                return;
            }
        }

        public void ExportDetectedObjects()
        {
            using (var p = Pipeline.Create("CognitiveServices export detected objects"))
            {
                Console.WriteLine("exporting objectdetection\n ...");
                var inputStore = Store.Open(p, "storedObjectDetection", StoreLocation);

                FrameInfoAsString analyseComp = new FrameInfoAsString(p, "Test001");

                var imageStream = inputStore.OpenStream<Shared<Image>>("image");
                var computerVisionStream = inputStore.OpenStream<ImageAnalysis>("computerVision response");
                var imageVisData = computerVisionStream.Out.Join(imageStream.Out);

                imageVisData.Out.Item2().PipeTo(analyseComp.Image1In);
                imageVisData.Out.Item1().PipeTo(analyseComp.DetectedObjectsIn);

                p.PipelineCompleted += P_PipelineCompleted;

                p.Run();

                return;
            }
        }
        
        public void CompareVideoAnalasis(string videoStoreName1, string videoStoreName2)
        {
            using (var p = Pipeline.Create("object detection differences"))
            {
                Console.WriteLine("comparing videos\n ...");
                var videoStore1 = Store.Open(p, videoStoreName1, StoreLocation);
                var videoStore2 = Store.Open(p, videoStoreName2, StoreLocation);

                var computerVisionStream1 = videoStore1.OpenStream<ImageAnalysis>("computerVision response");
                var computerVisionStream2 = videoStore2.OpenStream<ImageAnalysis>("computerVision response");

                var computerVisionImageStream1 = videoStore1.OpenStream<Shared<Image>>("image");
                var computerVisionImageStream2 = videoStore2.OpenStream<Shared<Image>>("image");

                //var joinedData = computerVisionStream1.Join(computerVisionImageStream1, Reproducible.Nearest<Shared<Image>>(RelativeTimeInterval.Past())).Join(computerVisionStream2, Reproducible.Nearest<ImageAnalysis>(RelativeTimeInterval.Past())).Join(computerVisionImageStream2, Reproducible.Nearest<Shared<Image>>(RelativeTimeInterval.Past()));

                //var joinedStream1 = computerVisionImageStream1.Join(computerVisionStream1);
                //var joinedStream2 = computerVisionImageStream2.Join(computerVisionStream2);

                var compareData = computerVisionStream1.Join(computerVisionStream2, Reproducible.Nearest<ImageAnalysis>(RelativeTimeInterval.Past()));
                //var imageData = computerVisionImageStream1.Join(computerVisionImageStream2, Reproducible.Nearest<Shared<Image>>(RelativeTimeInterval.Past()));
                //compareData.Join(imageData);

                CompareImageAnalysis compare = new CompareImageAnalysis(p);

                //compareData.Item1().Out.Do((x, e) => Console.WriteLine("stream1 input: " + e.OriginatingTime));
                //compareData.Item2().Out.Do((x, e) => Console.WriteLine("stream2 input: " + e.OriginatingTime));

                compareData.Item1().PipeTo(compare.Analysis1);
                compareData.Item2().PipeTo(compare.Analysis2);

                var store = Store.Create(p, "visualizedObjectDetection", StoreLocation);



                p.PipelineCompleted += P_PipelineCompleted;

                p.Run(ReplayDescriptor.ReplayAll);

                return;
            }
        }



        private static void P_PipelineCompleted(object sender, PipelineCompletedEventArgs e)
        {
            Console.WriteLine("Done!");
            //Console.ReadKey(true);
        }
    }
}
