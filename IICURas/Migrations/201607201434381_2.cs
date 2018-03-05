namespace IICURas.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReviewCompletions", "Reconciliation", c => c.Boolean(nullable: false));
            DropColumn("dbo.PaperQualities", "Reconciliation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PaperQualities", "Reconciliation", c => c.Boolean(nullable: false));
            DropColumn("dbo.ReviewCompletions", "Reconciliation");
        }
    }
}
