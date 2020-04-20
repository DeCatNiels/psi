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

        private int _fileCount = 0;
        private int _amountFilesNeeded;
        private bool _needMoreFiles = true;

        public bool NeedMoreFiles
        {
            get { return _needMoreFiles; }
            set { _needMoreFiles = value; }
        }
        public int FileCount
        {
            get { return _fileCount; }
            set { _fileCount = value; }
        }
        public int AmountFilesNeeded
        {
            get { return _amountFilesNeeded; }
            set { _amountFilesNeeded = value; }
        }


        // Gets or sets the state
        public ProgressState ProgressState
        {
            get { return _progressState; }
            set { _progressState = value; }
        }

        public Progress()
        {
            this.ProgressState = new NotEnoughFilesState();
            this.ProgressState.SetProgress(this);
        }

        public void Next()
        {
            this.ProgressState.NextStep();
        }

        public void Undo()
        {
            this.ProgressState.UndoStep();
        }
    }
}
