using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerVision
{
    public class FrameComparison
    {
        public List<ImageTag> simmularities;
        public List<ImageTag> differencesFrame1;
        public List<ImageTag> differencesFrame2;

        public FrameComparison(List<ImageTag> simmularities, List<ImageTag> differencesFrame1, List<ImageTag> differencesFrame2)
        {
            this.simmularities = simmularities;
            this.differencesFrame1 = differencesFrame1;
            this.differencesFrame2 = differencesFrame2;
        }

        public string toJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
