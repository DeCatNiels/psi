using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Psi;
using Microsoft.Psi.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ComputerVision
{
    class FrameInfoAsString
    {
        // Variable used to store the last input string
        private Shared<Microsoft.Psi.Imaging.Image> lastReceivedImageInput = null;
        private string dir = "";

        public FrameInfoAsString(Pipeline pipeline, string storeName)
        {
            this.dir = "ComputerVisionAnalyzing\\" + storeName + "\\frame";

            // create detectedObjects receiver
            this.DetectedObjectsIn = pipeline.CreateReceiver<ImageAnalysis>(this, ReceiveDetectedObjects, nameof(this.DetectedObjectsIn));

            this.Image1In = pipeline.CreateReceiver<Shared<Microsoft.Psi.Imaging.Image>>(this, this.ReceivedImage, nameof(this.Image1In));

            // create string emitter
            this.Out = pipeline.CreateEmitter<string>(this, nameof(this.Out));
        }

        // Receiver that encapsulates the Image input stream
        public Receiver<Shared<Microsoft.Psi.Imaging.Image>> Image1In { get; private set; }

        // Receiver that encapsulates the Detected object input stream
        public Receiver<ImageAnalysis> DetectedObjectsIn { get; private set; }

        // Emitter that encapsulates the output stream
        public Emitter<string> Out { get; private set; }

        // The receive method for the DetectedObjectsIn receiver. This executes every time a message arrives on DetectedObjectsIn.
        private void ReceiveDetectedObjects(ImageAnalysis input, Envelope envelope)
        {
            Directory.CreateDirectory(dir + envelope.SequenceId);

            using(StringWriter detectedInfo = new StringWriter())
            {
                detectedInfo.WriteLine("--- categories ---");
                foreach (Category cat in input.Categories)
                {
                    detectedInfo.WriteLine(cat.Name + " - " + cat.Detail + " - " + cat.Score + "\n");
                }
                detectedInfo.WriteLine("\n");

                detectedInfo.WriteLine("--- color ---");
                detectedInfo.WriteLine("accent color: " + input.Color.AccentColor);
                detectedInfo.WriteLine("accent color: " + input.Color.DominantColorBackground);
                detectedInfo.WriteLine("accent color: " + input.Color.DominantColorForeground);
                detectedInfo.WriteLine(" -- dominant colors: -- ");
                foreach (string col in input.Color.DominantColors)
                {
                    detectedInfo.WriteLine(" - " + col + "\n");
                }
                detectedInfo.WriteLine("\n");

                detectedInfo.WriteLine("--- description ---");
                detectedInfo.WriteLine(" -- captions: -- ");
                foreach (ImageCaption cap in input.Description.Captions)
                {
                    detectedInfo.WriteLine(cap.Text + ": " + cap.Confidence + "\n");
                }

                detectedInfo.WriteLine(" -- tags: -- ");
                foreach (string tag in input.Description.Tags)
                {
                    detectedInfo.WriteLine(" - " + tag + "\n");
                }
                detectedInfo.WriteLine("\n");

                this.Out.Post(detectedInfo.GetStringBuilder().ToString(), envelope.OriginatingTime);
            }
        }

        // The receive method for the ImageIn receiver. This executes every time a message arrives on ImageIn.
        /*
         * START - save the image in the store.
         */
        private void ReceivedImage(Shared<Microsoft.Psi.Imaging.Image> input, Envelope envelope)
        {
            Directory.CreateDirectory(dir + envelope.SequenceId);

            Bitmap managedImage = input.Resource.ToManagedImage();
            managedImage.Save(dir + envelope.SequenceId + "\\stream1Image.png", System.Drawing.Imaging.ImageFormat.Png);
        }
        /*
        * END - DRAW OBJECTS ON IMAGE
        */
    }
}
