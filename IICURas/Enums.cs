//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

namespace IICURas
{
    public static class Enums
    {
        public enum userRoles
        {
          Administrator,
          Randomizer,
          SeniorReviewer,
          Reviewer,
          Trainee,
          Uploader,
            Suspended,
            Reconciler
        }

        public enum PassingCriteria
        {
            PassingPercentage = 80,
            PassingTimes = 3
        }

        public enum ReviewComplesionThreshold
        {
            MinimumReviewedNum = 2  //before reconsilation
        }

        public enum RecordLimit
        {
            Total = 1689
        }

        public enum OngoingPublicationLimit
        {
            ExternalReviewer = 2,
            InternalReviewer = 5,
            Reconciler = 3
        }

        public enum CheatingCriteria
        {
            percentage = 50,
            time = 900
        }

        public enum Training
        {
            OpeningTrainingLimit = 1
        }

        public enum Status
        {
            Current,
            Archived,
            Deleted
        }

        public enum ReviewStatus
        {
            InternalInProgress,
            InternalComplete,
            ExternalInProgress,
            ExternalComplete,
            ReconciliationInProgress,
            ReconciliationComplete
        }
    }
}