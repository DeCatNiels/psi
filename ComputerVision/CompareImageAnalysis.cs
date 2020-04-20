using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Psi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerVision
{
    class CompareImageAnalysis
    {
        private ImageAnalysis imageAnalysis1 = null;
        public CompareImageAnalysis(Pipeline pipeline)
        {
            // create imageAnalysis1 receiver
            this.Analysis1 = pipeline.CreateReceiver<ImageAnalysis>(this, ReceiveAnalysis1, nameof(this.Analysis1));
            
            // create imageAnalysis1 receiver
            this.Analysis2 = pipeline.CreateReceiver<ImageAnalysis>(this, ReceiveAnalysis2, nameof(this.Analysis2));

            //create difference emitter
            this.Out = pipeline.CreateEmitter<FrameComparison>(this, nameof(this.Out));
        }

        private void ReceiveAnalysis2(ImageAnalysis imageAnalysis2, Envelope e)
        {
            Console.WriteLine("   ----------   ----------   ----------");
            List<ImageTag> similarTags = new List<ImageTag>();
            List<ImageTag> differentTagsImage1 = new List<ImageTag>();
            List<ImageTag> differentTagsImage2 = new List<ImageTag>();
            foreach (ImageTag tag in imageAnalysis1.Tags)
            {
                if (imageAnalysis2.Tags.Contains(tag))
                {
                    similarTags.Add(tag);
                }
                else
                {
                    differentTagsImage1.Add(tag);
                }
            }
            foreach (ImageTag tag in imageAnalysis2.Tags)
            {
                if (!similarTags.Contains(tag))
                {
                    differentTagsImage2.Add(tag);
                }
            }

            FrameComparison frameComp = new FrameComparison(similarTags, differentTagsImage1, differentTagsImage2);
            Console.WriteLine(frameComp.toJsonString());

            this.Out.Post(frameComp, e.OriginatingTime);
        }

        private void ReceiveAnalysis1(ImageAnalysis analysis1, Envelope e)
        {
            this.imageAnalysis1 = analysis1;
        }

        // Receiver that encapsulates the Image input stream
        public Receiver<ImageAnalysis> Analysis1 { get; private set; }

        // Receiver that encapsulates the Detected object input stream
        public Receiver<ImageAnalysis> Analysis2 { get; private set; }

        // Emitter that encapsulates the output stream
        public Emitter<FrameComparison> Out { get; private set; }


    }
}
