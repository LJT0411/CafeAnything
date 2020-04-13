namespace CafeAnything.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dwds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderCarts", "TotalOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderCarts", "TotalOrder");
        }
    }
}
