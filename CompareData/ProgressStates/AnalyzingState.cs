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
            this._progress.ProgressState = new DoneState();
            this._progress.ProgressState.SetProgress(this._progress);
        }

        public override void UndoStep()
        {
            this._progress.ProgressState = new ReadyToAnalyzeState();
        }
    }
}
