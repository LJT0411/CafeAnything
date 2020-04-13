namespace CafeAnything.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OrderCarts", "TotalOrder");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderCarts", "TotalOrder", c => c.Int(nullable: false));
        }
    }
}
