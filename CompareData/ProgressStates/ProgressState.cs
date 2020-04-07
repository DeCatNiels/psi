namespace CompareData
{
    abstract class ProgressState
    {
        protected Progress _progress;
        private int _fileCount = 0;
        private int _amountFilesNeeded;
        private bool _needMoreFiles = true;

        public bool NeedMoreFiles { get; protected set; }
        public int FileCount { get; protected set; }
        public int AmountFilesNeeded { get; set; }

        public void SetProgress(Progress progress, int amountFiles = 2)
        {
            this._progress = progress;
            this._amountFilesNeeded = amountFiles;
        }

        public abstract void NextStep();

        public abstract void UndoStep();

        public abstract string GetDiscription();
    }
}