namespace IICURas.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class IICURasContext : DbContext
    {
        public IICURasContext()
            : base("name=IICURasConnection")
        {
        }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }

        public virtual DbSet<PaperDocument> PaperDocuments { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<ChecklistOptionLink> ChecklistOptionLinks { get; set; }
        public virtual DbSet<CheckList> CheckLists { get; set; }

        public virtual DbSet<Category> Categories { get; set; }       
        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Record> Records { get; set; }    
        public virtual DbSet<PaperQuality> PaperQualities { get; set; }
        public virtual DbSet<Specie> Species { get; set; }
        public virtual DbSet<LinkRecordUserSpecie> LinkRecordUserSpecies { get; set; }
        public virtual DbSet<ReviewCompletion> ReviewCompletions { get; set; }

        public virtual DbSet<GoldStandard> GoldStandards { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<TrainingPublication> TrainingPublications { get; set; }
        public virtual DbSet<TrainingReview> TrainingReviews { get; set; }
        public virtual DbSet<TrainingReviewItem> TrainingReviewItems { get; set; }
        public virtual DbSet<TrainingDocument> TrainingDocuments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Records)
                .WithRequired(e => e.Category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Country>()
                .HasMany(e => e.Records)
                .WithRequired(e => e.Country)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Record>()
                .HasMany(e => e.PaperDocuments)
                .WithOptional(e => e.Record)
                .WillCascadeOnDelete(true);

            //modelBuilder.Entity<TrainingPublication>()
            //    .HasMany(e => e.PaperDocuments)
            //    .WithOptional(e => e.TrainingPublication)
            //    .WillCascadeOnDelete(true);

            modelBuilder.Entity<Record>()
                .HasMany(e => e.PaperQualities)
                .WithRequired(e => e.Record)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.PaperQualities)
                .WithRequired(e => e.UserProfile)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CheckList>()
                .HasMany(e => e.PaperQualities)
                .WithRequired(e => e.CheckList)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CheckList>()
                .HasMany(e => e.ChecklistOptionLinks)
                .WithRequired(e => e.CheckLists)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Option>()
              .HasMany(e => e.ChecklistOptionLinks)
              .WithRequired(e => e.Option)
              .WillCascadeOnDelete(true);

            modelBuilder.Entity<Option>()
              .HasMany(e => e.Reviews)
              .WithRequired(e => e.Option)
              .WillCascadeOnDelete(true);

            modelBuilder.Entity<Record>()
                .HasMany(e => e.LinkRecordUserSpecies)
                .WithRequired(e => e.Records)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Specie>()
                .HasMany(e => e.LinkRecordUserSpecies)
                .WithRequired(e => e.Species)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.LinkRecordUserSpecies)
                .WithRequired(e => e.UserProfile)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ReviewCompletion>()
             .HasMany(e => e.PaperQualities)
             .WithRequired(e => e.ReviewCompletion)
             .WillCascadeOnDelete(true);

            modelBuilder.Entity<ReviewCompletion>()
             .HasMany(e => e.LinkRecordUserSpecies)
             .WithRequired(e => e.ReviewCompletion)
             .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserProfile>()
             .HasMany(e => e.ReviewCompletions)
             .WithRequired(e => e.UserProfile)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<Record>()
             .HasMany(e => e.ReviewCompletions)
             .WithRequired(e => e.Record)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<Record>()
             .HasMany(e => e.PaperQualities)
             .WithRequired(e => e.Record)
             .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserProfile>()
               .HasMany(e => e.Promotions)
               .WithRequired(e => e.UserProfile)
               .WillCascadeOnDelete(true);

            //modelBuilder.Entity<TrainingPublication>()
            // .HasMany(e => e.PaperDocuments)
            // .WithOptional(e => e.TrainingPublication)
            // .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingPublication>()
             .HasMany(e => e.TrainingReviews)
             .WithRequired(e => e.TrainingPublication)
             .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserProfile>()
             .HasMany(e => e.TrainingReviews)
             .WithRequired(e => e.UserProfile)
             .WillCascadeOnDelete(true);

            modelBuilder.Entity<TrainingPublication>()
             .HasMany(e => e.TrainingReviews)
             .WithRequired(e => e.TrainingPublication)
             .WillCascadeOnDelete(true);

            modelBuilder.Entity<TrainingReview>()
                 .HasMany(e => e.TrainingReviewItems)
                 .WithRequired(e => e.TrainingReview)
                 .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserProfile>()
             .HasMany(e => e.TrainingReviewItems)
             .WithRequired(e => e.UserProfile)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<CheckList>()
                 .HasMany(e => e.TrainingReviewItems)
                 .WithRequired(e => e.CheckList)
                 .WillCascadeOnDelete(true);

            modelBuilder.Entity<Option>()
                 .HasMany(e => e.TrainingReviewItems)
                 .WithOptional(e => e.Option)
                 .WillCascadeOnDelete(false);


            modelBuilder.Entity<TrainingPublication>()
                 .HasMany(e => e.TrainingReviewItems)
                 .WithRequired(e => e.TrainingPublication)
                 .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingPublication>()
                 .HasMany(e => e.TrainingDocuments)
                 .WithRequired(e => e.TrainingPublication)
                 .WillCascadeOnDelete(false);
        }
    }
}
