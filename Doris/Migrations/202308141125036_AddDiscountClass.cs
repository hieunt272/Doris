namespace Doris.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDiscountClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DiscountUsers",
                c => new
                    {
                        DiscountId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.DiscountId, t.UserId })
                .ForeignKey("dbo.Discounts", t => t.DiscountId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.DiscountId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Discounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        ShowName = c.String(maxLength: 200),
                        Description = c.String(maxLength: 500),
                        PriceOff = c.Decimal(precision: 18, scale: 2),
                        TotalOrder = c.Decimal(precision: 18, scale: 2),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderDiscounts",
                c => new
                    {
                        Order_Id = c.Int(nullable: false),
                        Discount_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Order_Id, t.Discount_Id })
                .ForeignKey("dbo.Orders", t => t.Order_Id, cascadeDelete: true)
                .ForeignKey("dbo.Discounts", t => t.Discount_Id, cascadeDelete: true)
                .Index(t => t.Order_Id)
                .Index(t => t.Discount_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DiscountUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.OrderDiscounts", "Discount_Id", "dbo.Discounts");
            DropForeignKey("dbo.OrderDiscounts", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.DiscountUsers", "DiscountId", "dbo.Discounts");
            DropIndex("dbo.OrderDiscounts", new[] { "Discount_Id" });
            DropIndex("dbo.OrderDiscounts", new[] { "Order_Id" });
            DropIndex("dbo.DiscountUsers", new[] { "UserId" });
            DropIndex("dbo.DiscountUsers", new[] { "DiscountId" });
            DropTable("dbo.OrderDiscounts");
            DropTable("dbo.Discounts");
            DropTable("dbo.DiscountUsers");
        }
    }
}
