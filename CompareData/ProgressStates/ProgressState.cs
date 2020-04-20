namespace CompareData
{
    abstract class ProgressState
    {
        public Progress _progress;
        public void SetProgress(Progress progress, int amountFiles = 2)
        {
            this._progress = progress;
            this._progress.AmountFilesNeeded = amountFiles;
        }

        public abstract void NextStep();

        public abstract void UndoStep();

        public abstract string GetDiscription();
    }
}