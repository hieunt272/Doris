namespace Doris.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerInfoBill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TypeBill", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "CustomerInfo_BusinessName", c => c.String());
            AddColumn("dbo.Orders", "CustomerInfo_TaxCode", c => c.String());
            AddColumn("dbo.Orders", "CustomerInfo_BillAddress", c => c.String());
            AddColumn("dbo.Orders", "CustomerInfo_BillEmail", c => c.String());
            AddColumn("dbo.Orders", "CustomerInfo_PersonEmail", c => c.String());
            AddColumn("dbo.Orders", "CustomerInfo_PersonName", c => c.String());
            AddColumn("dbo.Orders", "CustomerInfo_PersonAddress", c => c.String());
            AddColumn("dbo.Carts", "PriceUser", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "PriceUser");
            DropColumn("dbo.Orders", "CustomerInfo_PersonAddress");
            DropColumn("dbo.Orders", "CustomerInfo_PersonName");
            DropColumn("dbo.Orders", "CustomerInfo_PersonEmail");
            DropColumn("dbo.Orders", "CustomerInfo_BillEmail");
            DropColumn("dbo.Orders", "CustomerInfo_BillAddress");
            DropColumn("dbo.Orders", "CustomerInfo_TaxCode");
            DropColumn("dbo.Orders", "CustomerInfo_BusinessName");
            DropColumn("dbo.Orders", "TypeBill");
        }
    }
}
