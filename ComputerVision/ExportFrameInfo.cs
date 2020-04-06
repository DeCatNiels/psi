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
    class ExportFrameInfo
    {
        // Variable used to store the last input string
        private Shared<Microsoft.Psi.Imaging.Image> lastReceivedImageInput = null;
        private string dir = "";

        public ExportFrameInfo(Pipeline pipeline, string storeName)
        {
            this.dir = "ComputerVisionAnalyzing\\" + storeName + "\\frame"; ;

            // create detectedObjects receiver
            this.DetectedObjectsIn = pipeline.CreateReceiver<ImageAnalysis>(this, ReceiveDetectedObjects, nameof(this.DetectedObjectsIn));

            this.Image1In = pipeline.CreateReceiver<Shared<Microsoft.Psi.Imaging.Image>>(this, this.ReceivedImage, nameof(this.Image1In));

            // create input receiver
            //this.In = pipeline.CreateReceiver<Tuple<ImageAnalysis, Shared<Microsoft.Psi.Imaging.Image>>>(this, this.ReceiveAsync, nameof(this.In));
        }

        // Receiver that encapsulates the Image input stream
        public Receiver<Shared<Microsoft.Psi.Imaging.Image>> Image1In { get; private set; }

        // Receiver that encapsulates the Detected object input stream
        public Receiver<ImageAnalysis> DetectedObjectsIn { get; private set; }

        // The receive method for the DetectedObjectsIn receiver. This executes every time a message arrives on DetectedObjectsIn.
        private void ReceiveDetectedObjects(ImageAnalysis input, Envelope envelope)
        {
            Directory.CreateDirectory(dir + envelope.SequenceId);

            using(StreamWriter file = new StreamWriter(dir + envelope.SequenceId + "\\stream1Analisis.txt"))
            {
                file.WriteLine("--- categories ---");
                foreach (Category cat in input.Categories)
                {
                    file.WriteLine(cat.Name + " - " + cat.Detail + " - " + cat.Score + "\n");
                }
                file.WriteLine("\n");

                file.WriteLine("--- color ---");
                file.WriteLine("accent color: " + input.Color.AccentColor);
                file.WriteLine("accent color: " + input.Color.DominantColorBackground);
                file.WriteLine("accent color: " + input.Color.DominantColorForeground);
                file.WriteLine(" -- dominant colors: -- ");
                foreach (string col in input.Color.DominantColors)
                {
                    file.WriteLine(" - " + col + "\n");
                }
                file.WriteLine("\n");

                file.WriteLine("--- description ---");
                file.WriteLine(" -- captions: -- ");
                foreach (ImageCaption cap in input.Description.Captions)
                {
                    file.WriteLine(cap.Text + ": " + cap.Confidence + "\n");
                }

                file.WriteLine(" -- tags: -- ");
                foreach (string tag in input.Description.Tags)
                {
                    file.WriteLine(" - " + tag + "\n");
                }
                file.WriteLine("\n");
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
