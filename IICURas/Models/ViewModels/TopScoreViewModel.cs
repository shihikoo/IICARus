using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Data.OData;

namespace IICURas.Models
{
    public class TopScoreViewModel
    {

        public TopScoreViewModel(string username, bool completeTraining, DateTime lastActiveTime, int points)
        {
            UserName = username;
            CompleteTraining = completeTraining;
            LastActiveTime = lastActiveTime;
            Points = points;
        }

        public TopScoreViewModel()
        {

        }

        public int Rank { get; set; }

        [DisplayName("UserName")]
        public string  UserName { get; set; }

        [DisplayName("Complete Training")]
        public bool CompleteTraining { get; set; }

        [DisplayName("Last Active Time")]
        public DateTime LastActiveTime { get; set; }

        [DisplayName("Number of Review Completed")]
        public int Points { get; set; }

     
    }
}