namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.String(maxLength: 20),
                        Name = c.String(nullable: false, maxLength: 30),
                        PostCodeFirst = c.Int(nullable: false),
                        PostCodeLast = c.Int(),
                        AssignedToUserId = c.Int(),
                        DistrictBoundaryKml = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.AssignedToUserId)
                .Index(t => t.AssignedToUserId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 45),
                        PostCode = c.Int(nullable: false),
                        AddedByUserId = c.Int(),
                        Selected = c.Boolean(nullable: false),
                        cx = c.Binary(),
                        District_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Districts", t => t.District_Id)
                .Index(t => t.District_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.People", "District_Id", "dbo.Districts");
            DropForeignKey("dbo.Districts", "AssignedToUserId", "dbo.UserProfile");
            DropIndex("dbo.People", new[] { "District_Id" });
            DropIndex("dbo.Districts", new[] { "AssignedToUserId" });
            DropTable("dbo.People");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Districts");
        }
    }
}
