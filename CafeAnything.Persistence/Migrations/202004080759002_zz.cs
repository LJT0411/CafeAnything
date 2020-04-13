namespace CafeAnything.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zz : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderCarts", "ID", "dbo.Users");
            DropIndex("dbo.OrderCarts", new[] { "ID" });
            CreateTable(
                "dbo.Tables",
                c => new
                    {
                        TablesID = c.Int(nullable: false, identity: true),
                        TableNo = c.String(),
                        TStatus = c.Int(nullable: false),
                        TotalQuantity = c.Int(nullable: false),
                        TotalAmount = c.Double(nullable: false),
                        ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TablesID)
                .ForeignKey("dbo.Users", t => t.ID, cascadeDelete: true)
                .Index(t => t.ID);
            
            AddColumn("dbo.OrderCarts", "TablesID", c => c.Int(nullable: false));
            CreateIndex("dbo.OrderCarts", "TablesID");
            AddForeignKey("dbo.OrderCarts", "TablesID", "dbo.Tables", "TablesID", cascadeDelete: true);
            DropColumn("dbo.OrderCarts", "ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderCarts", "ID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Tables", "ID", "dbo.Users");
            DropForeignKey("dbo.OrderCarts", "TablesID", "dbo.Tables");
            DropIndex("dbo.Tables", new[] { "ID" });
            DropIndex("dbo.OrderCarts", new[] { "TablesID" });
            DropColumn("dbo.OrderCarts", "TablesID");
            DropTable("dbo.Tables");
            CreateIndex("dbo.OrderCarts", "ID");
            AddForeignKey("dbo.OrderCarts", "ID", "dbo.Users", "ID", cascadeDelete: true);
        }
    }
}
