using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareData
{
    class DoneState : ProgressState
    {
        public override string GetDiscription()
        {
            return "analyzing is done!";
        }

        public override void NextStep()
        {
            this._progress.ProgressState = new NotEnoughFilesState();
        }

        public override void UndoStep()
        {
            this._progress.ProgressState = new ReadyToAnalyzeState();
        }
    }
}
