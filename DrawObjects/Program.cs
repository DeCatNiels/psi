using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Psi;
using Microsoft.Psi.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var p = Pipeline.Create("DrawObjects"))
            {
                var store = Store.Open(p, "storedObjectDetection", @"E:\PSIVideoStores\computerVision");
                var saveStore = Store.Create(p, "objectsVisualised", @"E:\PSIVideoStores\objectsVisualized");

                var images = store.OpenStream<Shared<Image>>("image");
                var analysedImageResult = store.OpenStream<ImageAnalysis>("computerVision response");

                ObjectVisualizer objVis = new ObjectVisualizer(p);
                analysedImageResult.Out.PipeTo(objVis.DetectedObjectsIn, DeliveryPolicy.LatestMessage);

                images.PipeTo(objVis.ImageIn, DeliveryPolicy.LatestMessage);

                objVis.Out.Write("detected objects in Image", saveStore);

                p.Run();

                return;

            }

        }
    }
}
