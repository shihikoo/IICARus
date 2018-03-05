using System;
using System.Collections.Generic;
using System.Linq;


namespace IICURas.Models.ViewModels
{
    public class ReportViewModel
    {
        public ReportViewModel(ICollection<UserProfile> users, ICollection<Record> records)
        {
            RegistrationsProgressArray = GetProgressArray(users.Select(u => u.CreateDate.DateTime).ToList());

            InternalReviewProgressArray = GetProgressArray(records.Select(p => 
            p.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null).Min(rc => rc.ComplesionDate??new DateTime()))
            .ToList());

            ExternalReviewProgressArray = GetProgressArray(records.Where(r => r.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null) >= 2).Select(p => 
            p.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null).Skip(1).Min(rc => rc.ComplesionDate ?? new DateTime())).ToList());

            RegistrationsAverageArray = GetAverageArray(users.Select(u => u.CreateDate.DateTime).ToList());

            InternalReviewAverageArray = GetAverageArray(records.Select(p =>
            p.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null).Min(rc => rc.ComplesionDate ?? new DateTime()))
            .ToList());

            ExternalReviewAverageArray = GetAverageArray(records.Where(r => r.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null) >= 2).Select(p =>
           p.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null).Skip(1).Min(rc => rc.ComplesionDate ?? new DateTime())).ToList());

            var agreementRatios = records.Where(
                p => p.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null) >= 2)
                .Select(p => CalculateAgreement(p.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null).Take(2))).ToList();

            ReviewQualityArray = GetReviewQualityArray(agreementRatios);
        }

        public double[,] RegistrationsProgressArray { get; set; }

        public double[,] ReviewQualityArray { get; set; }

        public double[,] InternalReviewProgressArray { get; set; }

        public double[,] ExternalReviewProgressArray { get; set; }

        public double[,] RegistrationsAverageArray { get; set; }

        public double[,] InternalReviewAverageArray { get; set; }

        public double[,] ExternalReviewAverageArray { get; set; }

        private static double CalculateAgreement(IEnumerable<ReviewCompletion> reviewCompletions)
        {
            var SeniorReviews = reviewCompletions
          .OrderBy(rc => rc.ComplesionDate)
          .FirstOrDefault()
          .PaperQualities;

            var JuniorReviews = reviewCompletions
                     .OrderBy(rc => rc.ComplesionDate)
                     .Skip(1).FirstOrDefault()
                     .PaperQualities;

            var totalpoints = SeniorReviews.Count(r => r.Status == "Current" && r.OptionOptionID != null);

            var point = SeniorReviews.Select(correctRev => JuniorReviews
            .FirstOrDefault(r => r.CheckListCheckListID == correctRev.CheckListCheckListID && r.RecordRecordID == correctRev.RecordRecordID && r.OptionOptionID == correctRev.OptionOptionID))
            .Count(rev => rev != null);

            return (double)point * 100.0 / totalpoints;
        }

        private static double[,] GetProgressArray(IList<DateTime> RCDateTime)
        {
            if (RCDateTime == null) return null;

            //const double timeGrid = 30 * 24 * 3600;

            const double monthGrid = 1;

            var startDate = new DateTime(2016, 01, 01); // users.Min(rc => rc.CreateDate)??default(DateTime);

            var plotStartDate = new DateTime(2016, 07, 31,23,59,59);

            var plotEndDate = DateTime.Now;

            var nCut = (int)Math.Ceiling(((plotEndDate.Year - plotStartDate.Year) * 12 + plotEndDate.Month - plotStartDate.Month + 1) / monthGrid);  //(int)Math.Ceiling((endDate - startDate).TotalSeconds / timeGrid);

            var outputArray = new double[nCut, 2];

            for (var ii = 0; ii < nCut; ii++)
            {
                outputArray[ii, 0] = plotStartDate.AddMonths(ii).ToOADate(); // .AddSeconds(timeGrid * (ii + 1)).ToOADate();

                outputArray[ii, 1] = RCDateTime.Count(d => d >= startDate && d <= plotStartDate.AddMonths(ii));
            }

            return outputArray;
        }

        private static double[,] GetAverageArray(IList<DateTime> RCDateTime)
        {
            if (RCDateTime == null) return null;

            //const double timeGrid = 30 * 24 * 3600;

            const double monthGrid = 1;

            var startDate = new DateTime(2016, 01, 01); // users.Min(rc => rc.CreateDate)??default(DateTime);

            var plotStartDate = new DateTime(2016, 07, 31,23,59,59);

            var plotEndDate = DateTime.Now;

            var endDate = DateTime.Now;

            var nCut = (int)Math.Ceiling(((plotEndDate.Year - plotStartDate.Year) * 12 + plotEndDate.Month - plotStartDate.Month + 1) / monthGrid);  //(int)Math.Ceiling((endDate - startDate).TotalSeconds / timeGrid);

            //var nCut = (int)Math.Ceiling((endDate - startDate).TotalSeconds / timeGrid);

            var outputArray = new double[nCut, 2];

            for (var ii = 0; ii < nCut; ii++)
            {
                outputArray[ii, 0] = plotStartDate.AddMonths(ii).ToOADate(); // .AddSeconds(timeGrid * (ii + 1)).ToOADate();

                outputArray[ii, 1] = RCDateTime.Count(d => d >= plotStartDate.AddMonths(ii - 1) && d <= plotStartDate.AddMonths(ii));

                //outputArray[ii, 0] = startDate.AddSeconds(timeGrid * (ii + 1)).ToOADate();

                //outputArray[ii, 1] = RCDateTime.Count(d => d >= startDate.AddSeconds(timeGrid * (ii)) && d <= startDate.AddSeconds(timeGrid * (ii + 1)));
            }

            return outputArray;
        }

        private static double[,] GetReviewQualityArray(IList<double> Ratio)
        {
            if (Ratio == null) return null;

            const double grid = 10;

            var lowLimit = 0;

            var highLimit = 100;

            var nCut = (int)Math.Ceiling((highLimit - lowLimit) / grid);

            var outputArray = new double[nCut, 2];

            for (var ii = 0; ii < nCut; ii++)
            {
                outputArray[ii, 0] = lowLimit + grid * (ii);

                outputArray[ii, 1] = Ratio.Count(d => d >= lowLimit + grid * (ii) && d <= lowLimit + grid * (ii + 1));
            }

            return outputArray;
        }
    }
}
