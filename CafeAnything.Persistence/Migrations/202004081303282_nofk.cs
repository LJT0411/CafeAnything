namespace CafeAnything.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nofk : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderCarts", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.OrderCarts", "TablesID", "dbo.Tables");
            DropIndex("dbo.OrderCarts", new[] { "TablesID" });
            DropIndex("dbo.OrderCarts", new[] { "CategoryID" });
            AlterColumn("dbo.OrderCarts", "TablesID", c => c.Int());
            AlterColumn("dbo.OrderCarts", "CategoryID", c => c.Int());
            CreateIndex("dbo.OrderCarts", "TablesID");
            CreateIndex("dbo.OrderCarts", "CategoryID");
            AddForeignKey("dbo.OrderCarts", "CategoryID", "dbo.Categories", "CategoryID");
            AddForeignKey("dbo.OrderCarts", "TablesID", "dbo.Tables", "TablesID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderCarts", "TablesID", "dbo.Tables");
            DropForeignKey("dbo.OrderCarts", "CategoryID", "dbo.Categories");
            DropIndex("dbo.OrderCarts", new[] { "CategoryID" });
            DropIndex("dbo.OrderCarts", new[] { "TablesID" });
            AlterColumn("dbo.OrderCarts", "CategoryID", c => c.Int(nullable: false));
            AlterColumn("dbo.OrderCarts", "TablesID", c => c.Int(nullable: false));
            CreateIndex("dbo.OrderCarts", "CategoryID");
            CreateIndex("dbo.OrderCarts", "TablesID");
            AddForeignKey("dbo.OrderCarts", "TablesID", "dbo.Tables", "TablesID", cascadeDelete: true);
            AddForeignKey("dbo.OrderCarts", "CategoryID", "dbo.Categories", "CategoryID", cascadeDelete: true);
        }
    }
}
