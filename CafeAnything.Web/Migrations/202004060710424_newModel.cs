namespace CafeAnything.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderCarts",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        TotalAmount = c.Double(nullable: false),
                        ID = c.Int(nullable: false),
                        CategoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.ID, cascadeDelete: true)
                .Index(t => t.ID)
                .Index(t => t.CategoryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderCarts", "ID", "dbo.Users");
            DropForeignKey("dbo.OrderCarts", "CategoryID", "dbo.Categories");
            DropIndex("dbo.OrderCarts", new[] { "CategoryID" });
            DropIndex("dbo.OrderCarts", new[] { "ID" });
            DropTable("dbo.OrderCarts");
        }
    }
}
