namespace Doris.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSome : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductUsers",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        UserPrice = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.ProductId, t.UserId })
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Articles", "LastUpdate", c => c.DateTime());
            AddColumn("dbo.ConfigSites", "MemberShip", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.ProductUsers", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductUsers", new[] { "UserId" });
            DropIndex("dbo.ProductUsers", new[] { "ProductId" });
            DropColumn("dbo.ConfigSites", "MemberShip");
            DropColumn("dbo.Articles", "LastUpdate");
            DropTable("dbo.ProductUsers");
        }
    }
}
