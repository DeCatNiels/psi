namespace CompareData
{
    class NotEnoughFilesState : ProgressState
    {
        public override string GetDiscription()
        {
            return "Not all files are selected yet.\nYou need a total of " + this.AmountFilesNeeded + " files.";
        }

        public override void NextStep()
        {
            this.FileCount += 1;
            if (this.FileCount >= this.AmountFilesNeeded)
            {
                this.NeedMoreFiles = false;
                this._progress.ProgressState = new ReadyToAnalyzeState();
            }
        }

        public override void UndoStep()
        {
            this.FileCount -= 1;
            this.NeedMoreFiles = this.FileCount < this.AmountFilesNeeded;
        }
    }
}