using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CompareData
{
    class Progress
    {
        private ProgressState _progressState;
        private Func<string> _visualizeProgress;

        // Gets or sets the state
        public ProgressState ProgressState
        {
            get { return _progressState; }
            set { _progressState = value; }
        }

        public Progress()
        {
            this._progressState = new NotEnoughFilesState();
            //this._visualizeProgress = visualizeProgress;
        }

        public void AddFile()
        {
            this.ProgressState.NextStep();
        }
    }
}
