namespace IICURas.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class coi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Records", "ConflicOfIntest", c => c.String());
            AddColumn("dbo.Records", "Funding", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Records", "Funding");
            DropColumn("dbo.Records", "ConflicOfIntest");
        }
    }
}
