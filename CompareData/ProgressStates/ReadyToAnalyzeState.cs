using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareData
{
    class ReadyToAnalyzeState : ProgressState
    {
        public override string GetDiscription()
        {
            return "You can now start analyzing the files. (click analyze to start)";
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
