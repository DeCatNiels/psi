namespace CompareData
{
    class NotEnoughFilesState : ProgressState
    {
        public override string GetDiscription()
        {
            return "Not all files are selected yet.\nYou need a total of " + this._progress.AmountFilesNeeded + " files.";
        }

        public override void NextStep()
        {
            this._progress.FileCount += 1;
            if (this._progress.FileCount >= this._progress.AmountFilesNeeded)
            {
                this._progress.NeedMoreFiles = false;
                this._progress.ProgressState = new ReadyToAnalyzeState();
                this._progress.ProgressState.SetProgress(this._progress);
            }
        }

        public override void UndoStep()
        {
            this._progress.FileCount -= 1;
            this._progress.NeedMoreFiles = this._progress.FileCount < this._progress.AmountFilesNeeded;
        }
    }
}