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
            Console.WriteLine("   ----------   new frame incomming   ----------   ");

            FeatureComparison<ImageTag> tagComp = getTagComp(imageAnalysis1.Tags, imageAnalysis2.Tags);

            FeatureComparison<DetectedObject> objComp = getObjectComp(imageAnalysis1.Objects, imageAnalysis2.Objects);

            //Console.WriteLine(" | | | | | |   tag comparison   | | | | | | ");
            //Console.WriteLine(tagComp.toJsonString());
            //Console.WriteLine();

            //Console.WriteLine(" | | | | | |   object comparison   | | | | | | ");
            //Console.WriteLine(objComp.toJsonString());
            //Console.WriteLine();

            var fc = new FrameComparison(tagComparison: tagComp, objectComparison: objComp, faceComparison: Tuple.Create(imageAnalysis1.Faces, imageAnalysis2.Faces));
            Console.WriteLine(fc.toJsonString());

            this.Out.Post(fc, e.OriginatingTime);
        }

        private FeatureComparison<ImageTag> getTagComp(IList<ImageTag> tagsFrame1, IList<ImageTag> tagsFrame2)
        {
            Dictionary<string, List<(ImageTag, ImageTag)>> similarTags = new Dictionary<string, List<(ImageTag, ImageTag)>>();
            Dictionary<string, List<ImageTag>> differentTagsImage1 = new Dictionary<string, List<ImageTag>>();
            Dictionary<string, List<ImageTag>> differentTagsImage2 = new Dictionary<string, List<ImageTag>>();
            foreach (ImageTag tag in tagsFrame1)
            {
                bool found = false;
                foreach (ImageTag tag2 in tagsFrame2)
                {
                    if (tag.Name == tag2.Name)
                    {
                        similarTags.Add( tag.Name, new List<(ImageTag, ImageTag)>() { (tag, tag2) });
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    differentTagsImage1.Add(tag.Name, new List<ImageTag>() { tag });
                }
            }
            foreach (ImageTag tag in tagsFrame2)
            {
                if (!similarTags.ContainsKey(tag.Name))
                {
                    differentTagsImage2.Add(tag.Name, new List<ImageTag>() { tag });
                }
            }

            return new FeatureComparison<ImageTag>(similarTags, differentTagsImage1, differentTagsImage2);
        }

        private FeatureComparison<DetectedObject> getObjectComp(IList<DetectedObject> objectsFrame1, IList<DetectedObject> objectsFrame2)
        {
            Dictionary<string, List<(DetectedObject, DetectedObject)>> similarObjects = new Dictionary<string, List<(DetectedObject, DetectedObject)>>();
            Dictionary<string, List<DetectedObject>> differentObjectsImage1 = new Dictionary<string, List<DetectedObject>>();
            Dictionary<string, List<DetectedObject>> differentObjectsImage2 = new Dictionary<string, List<DetectedObject>>();
            List<DetectedObject> addedObj = new List<DetectedObject>();
            foreach (DetectedObject obj in objectsFrame1)
            {
                bool found = false;
                foreach (DetectedObject obj2 in objectsFrame2)
                {
                    if (!addedObj.Contains(obj2) && obj.ObjectProperty == obj2.ObjectProperty)
                    {
                        if (similarObjects.ContainsKey(obj.ObjectProperty))
                        {
                            similarObjects[obj.ObjectProperty].Add((obj, obj2));
                        }
                        else
                        {
                            similarObjects.Add(obj.ObjectProperty, new List<(DetectedObject, DetectedObject)>() { (obj, obj2) });
                        }
                        addedObj.Add(obj2);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    if (differentObjectsImage1.ContainsKey(obj.ObjectProperty))
                    {
                        differentObjectsImage1[obj.ObjectProperty].Add(obj);
                    }
                    else
                    {
                        differentObjectsImage1.Add(obj.ObjectProperty, new List<DetectedObject>() { obj });
                    }
                }
            }
            foreach (DetectedObject obj in objectsFrame2)
            {
                if (!addedObj.Contains(obj))
                {
                    if (differentObjectsImage2.ContainsKey(obj.ObjectProperty))
                    {
                        differentObjectsImage2[obj.ObjectProperty].Add(obj);
                    }
                    else
                    {
                        differentObjectsImage2.Add(obj.ObjectProperty, new List<DetectedObject>() { obj });
                    }
                }
            }

            return new FeatureComparison<DetectedObject>(similarObjects, differentObjectsImage1, differentObjectsImage2);
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
