
namespace IICURas.Models
{
    public class SummaryViewModel
    {
        public int TotalRecord { private get; set; }

        public int numberRecords { get; set; }

        public string CompleteRandomizationStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0*numberRecords/TotalRecord)}%";

        public int numberRecordsFailTC1 { get; set; }

        public string CompleteFailTC1Style => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberRecordsFailTC1 / TotalRecord)}%";

        public int numberRecordsPassTC1 { get; set; }

        public string CompletePassTC1Style  => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0*numberRecordsPassTC1/ TotalRecord)}%";

        public int numberRecordsToTC1 => numberRecords - numberRecordsPassTC1 - numberRecordsFailTC1;

        public string ToTC1Style => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberRecordsToTC1 / TotalRecord)}%";

        public int numberAccept { get; set; }

        public string AcceptStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0*numberAccept/ TotalRecord)}%";

        public int numberNonAccept { get; set; }

        public string NonAcceptStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0*numberNonAccept/ TotalRecord)}%";

        public int numberToUpdate => numberRecordsPassTC1 - numberAccept - numberNonAccept;

        public string ToUpdateStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0*numberToUpdate/ TotalRecord)}%";

        public int numberUploaded { get; set; }

        public string CompletedUploadStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberUploaded / TotalRecord)}%";

        public int numberToUpload => numberAccept - numberUploaded;

        public string ToUploadStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberToUpload / TotalRecord)}%";

        public int numberReconciliationCompleted { get; set; }

        public string ReconciliationCompletedStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberReconciliationCompleted / TotalRecord)}%";

        public int numberReconciliationInProgress { get; set; }

        public string ReconciliationInProgressStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberReconciliationInProgress / TotalRecord)}%";

        public int numberDoubleReviewed { get; set; }

        public string DoubleReviewedStylev => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberDoubleReviewed / TotalRecord)}%";

        public int numberDoubleReviewInProgress { get; set; }

        public string DoubleReviewInProgressStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberDoubleReviewInProgress / TotalRecord)}%";

        public int numberSingleReiewed { get; set; }

        public string SingleReviewedStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberSingleReiewed / TotalRecord)}%";

        public int numberSingleReiewedInProgress { get; set; }

        public string SingleReiewedInProgressStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberSingleReiewed / TotalRecord)}%";

        public int numberToReiew
            => numberUploaded - numberReconciliationCompleted - numberReconciliationInProgress -  numberDoubleReviewed - numberDoubleReviewInProgress- numberSingleReiewed - numberSingleReiewedInProgress;

        public string ToReviewStyle => $"min-width: 1em; width: {(TotalRecord == 0 ? 0 : 100.0 * numberToReiew / TotalRecord)}%";

        public double[,] ProgressArray { get; set; }
    }
}