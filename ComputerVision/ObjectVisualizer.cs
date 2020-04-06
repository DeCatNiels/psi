using Microsoft.Psi;
using Microsoft.Psi.Imaging;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace ComputerVision
{
    class ObjectVisualizer
    {
        // Variable used to store the last input string
        private Shared<Microsoft.Psi.Imaging.Image> lastReceivedImageInput = null;
        //private DateTime originatingTime;

        // Constructor
        public ObjectVisualizer(Pipeline pipeline)
        {
            // create detectedObjects receiver
            this.DetectedObjectsIn = pipeline.CreateReceiver<ImageAnalysis>(this, ReceiveDetectedObjects, nameof(this.DetectedObjectsIn));

            this.ImageIn = pipeline.CreateReceiver<Shared<Microsoft.Psi.Imaging.Image>>(this, this.ReceivedImage, nameof(this.ImageIn));

            // create input receiver
            //this.In = pipeline.CreateReceiver<Tuple<ImageAnalysis, Shared<Microsoft.Psi.Imaging.Image>>>(this, this.ReceiveAsync, nameof(this.In));

            // create image emitter
            this.Out = pipeline.CreateEmitter<Shared<Microsoft.Psi.Imaging.Image>>(this, nameof(this.Out));
        }

        /// <summary>
        /// Gets the input imageAnalysis and images.
        /// </summary>
        //public Receiver<Tuple<ImageAnalysis, Shared<Microsoft.Psi.Imaging.Image>>> In { get; }

        // Receiver that encapsulates the Image input stream
        public Receiver<Shared<Microsoft.Psi.Imaging.Image>> ImageIn { get; private set; }

        // Receiver that encapsulates the Detected object input stream
        public Receiver<ImageAnalysis> DetectedObjectsIn { get; private set; }

        // Emitter that encapsulates the output stream
        public Emitter<Shared<Microsoft.Psi.Imaging.Image>> Out { get; private set; }

        // The receive method for the DetectedObjectsIn receiver. This executes every time a message arrives on DetectedObjectsIn.
        private void ReceiveDetectedObjects(ImageAnalysis input, Envelope envelope)
        {
            //Console.WriteLine("originating time image: " + this.originatingTime);
            //Console.WriteLine("originating time imageAnalysis: " + envelope.OriginatingTime);

            foreach (DetectedObject detectedObject in input.Objects)
            {
                try
                {
                    if (lastReceivedImageInput != null)
                    {
                        Rectangle rect = new Rectangle(detectedObject.Rectangle.X, detectedObject.Rectangle.Y, detectedObject.Rectangle.W, detectedObject.Rectangle.H);
                        Point textPoint = new Point(detectedObject.Rectangle.X + 10, detectedObject.Rectangle.Y + 10);
                        lastReceivedImageInput.Resource.DrawRectangle(rect, Color.Red, 3);
                        lastReceivedImageInput.Resource.DrawText(detectedObject.ObjectProperty + "\n" + detectedObject.Confidence, textPoint, new SolidBrush(Color.Red));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("new exception: " + ex.Message);
                    Console.WriteLine("stacktrace: " + ex.StackTrace);
                }
            }

            this.Out.Post(lastReceivedImageInput, envelope.OriginatingTime);

        }

        // The receive method for the ImageIn receiver. This executes every time a message arrives on ImageIn.
        /*
         * START - save the image in the store.
         */
        private void ReceivedImage(Shared<Microsoft.Psi.Imaging.Image> input, Envelope envelope)
        {
            this.lastReceivedImageInput = Shared.Create(input.Resource.DeepClone());
            //this.originatingTime = envelope.OriginatingTime;
        }
        /*
        * END - DRAW OBJECTS ON IMAGE
        */
    }
}
