namespace CafeAnything.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zc : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tables", "ID", "dbo.Users");
            DropIndex("dbo.Tables", new[] { "ID" });
            AlterColumn("dbo.Tables", "ID", c => c.Int());
            CreateIndex("dbo.Tables", "ID");
            AddForeignKey("dbo.Tables", "ID", "dbo.Users", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tables", "ID", "dbo.Users");
            DropIndex("dbo.Tables", new[] { "ID" });
            AlterColumn("dbo.Tables", "ID", c => c.Int(nullable: false));
            CreateIndex("dbo.Tables", "ID");
            AddForeignKey("dbo.Tables", "ID", "dbo.Users", "ID", cascadeDelete: true);
        }
    }
}
