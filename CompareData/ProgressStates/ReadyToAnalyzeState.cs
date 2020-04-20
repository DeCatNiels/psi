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
            this._progress.ProgressState = new AnalyzingState();
            this._progress.ProgressState.SetProgress(this._progress);
        }

        public override void UndoStep()
        {
            this._progress.ProgressState = new NotEnoughFilesState();
        }
    }
}
