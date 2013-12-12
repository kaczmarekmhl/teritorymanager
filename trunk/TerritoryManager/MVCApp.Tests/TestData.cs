using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCApp.Tests
{
    class TestData
    {
        public static IQueryable<District> Districts
        {
            get
            {
                var admin = new UserProfile()
                {
                    UserId = 1,
                    UserName = "admin"
                };
                var user = new UserProfile()
                {
                    UserId = 2,
                    UserName = "testuser"
                };
                var testDataSet = new List<District>
                {
                    new District
                    {
                    Id="1",
                    Name="Ballerup",
                    PostCode="2750",
                    BelongsToUser=user, 
                    },
                    new District
                    {
                    Id="2",
                    Name="Hedehusene",
                    PostCode="2640",
                    BelongsToUser=user, 
                    },
                    new District
                    {
                    Id="3",
                    Name="Ishoj",
                    PostCode="2635",
                    BelongsToUser=null, 
                    }
                };
                return testDataSet.AsQueryable();
            }
        }
    }
}
