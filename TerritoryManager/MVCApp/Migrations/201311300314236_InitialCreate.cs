namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonModels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Lastname = c.String(),
                        StreetAddress = c.String(),
                        TelephoneNumber = c.String(),
                        Longitude = c.String(),
                        Latitude = c.String(),
                        Notes = c.String(),
                        RemovedByUser = c.Boolean(nullable: false),
                        District_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DistrictModels", t => t.District_Id)
                .Index(t => t.District_Id);
            
            CreateTable(
                "dbo.DistrictModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        PostCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonModels", "District_Id", "dbo.DistrictModels");
            DropIndex("dbo.PersonModels", new[] { "District_Id" });
            DropTable("dbo.DistrictModels");
            DropTable("dbo.PersonModels");
        }
    }
}
