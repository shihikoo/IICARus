using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace IICURas.Models
{
    public class LeaderboardViewModel
    {

        public LeaderboardViewModel(IEnumerable<UserProfile> users )
        {
            AllTopScore = users.Where(u =>
              u.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null && rc.Record.AcceptanceStatus == "Accepted" && rc.Record.AcceptanceStatus_randomizer == "Pass TC1") > 0
            )
                .Select(u =>
                            new TopScoreViewModel(u.ForeName + " " + u.SurName, u.Promotions.Any(rc => rc.status == "Current"),
                                u.LastHeartbeat,
                                u.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null)))
                                .OrderByDescending(u => u.Points).ThenBy(u => u.CompleteTraining).ThenBy(u => u.LastActiveTime);

            ReviewTopScore = users.Where(u =>
                  u.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null && !rc.Reconciliation 
                  && rc.Record.AcceptanceStatus == "Accepted" && rc.Record.AcceptanceStatus_randomizer == "Pass TC1") > 0
                )
                    .Select(u =>
                new TopScoreViewModel(u.ForeName + " " + u.SurName, u.Promotions.Any(rc => rc.status == "Current"),
                    u.LastHeartbeat,
                    u.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null && !rc.Reconciliation)))
                    .OrderByDescending(u => u.Points).ThenBy(u => u.CompleteTraining).ThenBy(u => u.LastActiveTime);


            ReconciliationTopScore = users.Where(u =>
                  u.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null && rc.Reconciliation 
                  && rc.Record.AcceptanceStatus == "Accepted" && rc.Record.AcceptanceStatus_randomizer == "Pass TC1") > 0
                )
                    .Select(u =>
                new TopScoreViewModel(u.ForeName + " " + u.SurName, u.Promotions.Any(rc => rc.status == "Current"),
                    u.LastHeartbeat,
                    u.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null && rc.Reconciliation)))
                    .OrderByDescending(u => u.Points).ThenBy(u => u.CompleteTraining).ThenBy(u => u.LastActiveTime);

            FestivalTopScore = users.Where(u =>
             !Roles.IsUserInRole(u.UserName, "Administrator")
            && u.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()
            && rc.ComplesionDate != null
            && rc.LastUpdateTime >= new DateTime(2016,12,31,12,0,0) 
            && rc.LastUpdateTime < new DateTime(2017,3,1,12,0,0)) > 0)
                .Select(u =>
                new TopScoreViewModel(u.ForeName + " " + u.SurName, u.Promotions.Any(rc => rc.status == Enums.Status.Current.ToString()),
                    u.LastHeartbeat,
                    u.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()
                    && rc.ComplesionDate != null
                    && rc.LastUpdateTime >= new DateTime(2016, 12, 31, 12, 0, 0)
                    && rc.LastUpdateTime < new DateTime(2017, 3, 1, 12, 0, 0)
                    ))).OrderByDescending(u => u.Points).ThenBy(u => u.CompleteTraining).ThenBy(u => u.LastActiveTime);

            MonthExternalReviewerTopScore = users.Where(u => 
            !Roles.IsUserInRole(u.UserName,"Administrator")
            && u.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()
             && rc.ComplesionDate != null
            && rc.LastUpdateTime >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) 
            && rc.LastUpdateTime < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1)) > 0)
                .Select(u =>
                new TopScoreViewModel(u.ForeName + " " + u.SurName, u.Promotions.Any(rc => rc.status == "Current"),
                    u.LastHeartbeat,
                    u.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() 
                    && rc.ComplesionDate != null
                    && rc.LastUpdateTime >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                    && rc.LastUpdateTime < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1)
                    ))).OrderByDescending(u => u.Points);

            NewReviewerNames =
                users.Where(
                    u =>
                        u.Promotions.Count(
                            p => p.LastUpdateTime >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                                 &&
                                 p.LastUpdateTime < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1)) > 0).Select(u => u.ForeName + " " + u.SurName);
        }

        public IEnumerable<TopScoreViewModel> AllTopScore { get; set; }

        public IEnumerable<TopScoreViewModel> ReviewTopScore { get; set; }

        public IEnumerable<TopScoreViewModel> ReconciliationTopScore { get; set; }

        public IEnumerable<TopScoreViewModel> MonthExternalReviewerTopScore { get; set; }

        public IEnumerable<TopScoreViewModel> FestivalTopScore { get; set; }

        public IEnumerable<string> NewReviewerNames { get; set; }

    }
}
