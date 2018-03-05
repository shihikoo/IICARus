using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity;
//using System.Globalization;
//using System.Web.Security;


namespace IICURas.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        public UserProfile()
        {
            CreateDate = DateTime.Now;

            PaperQualities = new HashSet<PaperQuality>();

            LinkRecordUserSpecies = new HashSet<LinkRecordUserSpecie>();

            TrainingReviews = new HashSet<TrainingReview>();

            TrainingReviewItems = new HashSet<TrainingReviewItem>();

            Promotions = new HashSet<Promotion>();

            ReviewCompletions = new HashSet<ReviewCompletion>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [StringLength(50)]
        public string ForeName { get; set; }

        [StringLength(50)]
        public string SurName { get; set; }

        [Required]
        public string Email { get; set; }

        public DateTimeOffset CreateDate { get; set; }

        public string Details { get; set; }

        public string Institution { get; set; }

        public DateTime LastHeartbeat { get; set; }

        public bool CurrentlyLogged { get; set; }

        public virtual ICollection<PaperQuality> PaperQualities { get; set; }

        public virtual ICollection<LinkRecordUserSpecie> LinkRecordUserSpecies { get; set; }

        public virtual ICollection<TrainingReview> TrainingReviews { get; set; }

        public virtual ICollection<TrainingReviewItem> TrainingReviewItems { get; set; }

        public virtual ICollection<Promotion> Promotions { get; set; }

        public virtual ICollection<ReviewCompletion> ReviewCompletions { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username *")]
        public string UserName { get; set; }

         [DisplayName("Forename")]
        [StringLength(50)]
        public string ForeName { get; set; }

         [DisplayName("Surname")]
        [StringLength(50)]
        public string SurName { get; set; }

        [DataType(DataType.MultilineText)]
        public string Details { get; set; }

        public string Institution { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password *")]
        public string Password { get; set; }

         [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password *")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage = "Valid Message is required")]
        [EmailAddress]
        [Display(Name = "Email *")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm Email *")]
        [EmailAddress]
        [System.ComponentModel.DataAnnotations.Compare("Email", ErrorMessage = "The emails you've entered do not match.")]
        public string ConfirmEmail { get; set; }
    }

    public class UserViewModel
    {
        public int UserId { get; set; }
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Forename")]
        public string ForeName { get; set; }

        [Display(Name = "Surname")]
        public string SurName { get; set; }

        [Display(Name = "Note")]
        public string Details { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Institution")]
        public string Institution { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; }

        [Display(Name = "Registration Date")]
        public DateTimeOffset RegistrationDate { get; set; }
    }

    public class UserListViewModel
    {
        public int UserId { get; set; }
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Note")]
        public string Details { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Institution")]
        public string Institution { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "# Training")]
        public int TrainingDone { get; set; }

        [Display(Name = "# Review")]
        public int ReviewDone { get; set; }

        [Display(Name = "# Training")]
        public int TrainingStart { get; set; }

        [Display(Name = "# Review")]
        public int ReviewStart { get; set; }

        public bool suspended { get; set; }
    }

    public class LostPasswordModel
    {
        [Required(ErrorMessage = "We need your email to send you a reset link!")]
        [Display(Name = "Your account email")]
        [EmailAddress(ErrorMessage = "Not a valid email--what are you trying to do here?")]
        [DataType(DataType.Password)]
        public string Email { get; set; }
    }

    public class LostUsernameModel
    {
        [Required(ErrorMessage = "We need your email to send you a reset link!")]
        [Display(Name = "Your account email")]
        [EmailAddress(ErrorMessage = "Not a valid email--what are you trying to do here?")]
        [DataType(DataType.Password)]
        public string Email { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "New password and confirmation does not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ReturnToken { get; set; }
    }
}
