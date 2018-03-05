using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace IICURas.Models
{
    public class Promotion
    {
        public Promotion()
        {
            status = "Current";
            CreatedOn = DateTime.Now;

        }

        [Key]
        public int PromotionID { get; set; }

        [ForeignKey("UserProfile")]
        public int UserID { get; set; }

        public string Name { get; set; }

        public IICURas.Enums.userRoles oldRole { get; set; }

        public IICURas.Enums.userRoles newRole { get; set; }

        public string url { get; set; }

        public string status { get; set; }

        public DateTime CreatedOn { get; private set; }


        private DateTime? lastUpdateTime;

        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        public virtual UserProfile UserProfile { get; set; }


    }
}
