namespace UserOrder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialize1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserOrderInfoes", "PeopleNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserOrderInfoes", "PeopleNumber");
        }
    }
}
