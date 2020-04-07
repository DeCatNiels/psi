using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareData
{
    class AnalyzingState : ProgressState
    {
        public override string GetDiscription()
        {
            return "The files are being analyzed.";
        }

        public override void NextStep()
        {
            throw new NotImplementedException();
        }

        public override void UndoStep()
        {
            throw new NotImplementedException();
        }
    }
}
