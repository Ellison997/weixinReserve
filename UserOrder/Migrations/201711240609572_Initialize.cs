namespace UserOrder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserOrderInfoes", "UpdateDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserOrderInfoes", "UpdateDateTime");
        }
    }
}
