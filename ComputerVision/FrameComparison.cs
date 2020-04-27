using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerVision
{
    class FrameComparison
    {
        private FeatureComparison<ImageTag> tagComparison;
        private FeatureComparison<DetectedObject> objectComparison;
        private Tuple<IList<FaceDescription>, IList<FaceDescription>> faceComparison;

        public FrameComparison(FeatureComparison<ImageTag> tagComparison = null, FeatureComparison<DetectedObject> objectComparison = null, Tuple<IList<FaceDescription>, IList<FaceDescription>> faceComparison = null)
        {
            this.tagComparison = tagComparison;
            this.objectComparison = objectComparison;
            this.faceComparison = faceComparison;
        }

        public string toJsonString()
        {
            return "{tag comparison: " + tagComparison.toJsonString() + "object comparison: " + objectComparison.toJsonString() + "faces: " +  JsonConvert.SerializeObject(faceComparison, Formatting.Indented);
            //return JsonConvert.SerializeObject(faceComparison, Formatting.Indented);
        }

    }
}
