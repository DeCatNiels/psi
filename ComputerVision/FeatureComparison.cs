using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerVision
{
    public class FeatureComparison<T>
    {
        public Dictionary<string, List<(T, T)>> similarities;
        public Dictionary<string, List<T>> differencesFrame1;
        public Dictionary<string, List<T>> differencesFrame2;

        public FeatureComparison(Dictionary<string, List<(T, T)>> simmularities, Dictionary<string, List<T>> differencesFrame1, Dictionary<string, List<T>> differencesFrame2)
        {
            this.similarities = simmularities;
            this.differencesFrame1 = differencesFrame1;
            this.differencesFrame2 = differencesFrame2;
        }

        public string toJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
