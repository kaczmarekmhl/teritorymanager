using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public interface IDistrictManagerDb : IDisposable
    {
        IQueryable<T> Query<T>() where T : class;
        void Add<T>(T entity) where T : class;
        void AddRange<T>(IEnumerable<T> entities) where T : class;
        void Update<T>(T entity) where T : class;
        void Remove<T>(T entity) where T : class;
        void SaveChanges();
    }

    public class DistictManagerDb : DbContext, IDistrictManagerDb
    {
        public DistictManagerDb()
            : base("name=DefaultConnection")
        {
            //this.Configuration.LazyLoadingEnabled = true;
            //this.Configuration.ProxyCreationEnabled = false;
            //this.Configuration.ValidateOnSaveEnabled = false;
            //this.Configuration.AutoDetectChangesEnabled = false;          
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<DistrictReport> DistrictReports { get; set; }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            
            if (entityEntry.State == EntityState.Modified && entityEntry.Entity is District)
            {
                var district = entityEntry.Entity as District;

                int? oldUserId = entityEntry.GetDatabaseValues().GetValue<int?>("AssignedToUserId");
                int? newUserId = district.AssignedToUserId;

                if (oldUserId != newUserId)
                {
                    if (oldUserId.HasValue)
                    {
                        this.DistrictReports.Add(new DistrictReport(district, oldUserId.Value, DistrictReport.ReportTypes.Return));                     
                    }

                    if (newUserId.HasValue)
                    {
                        this.DistrictReports.Add(new DistrictReport(district, newUserId.Value, DistrictReport.ReportTypes.Request));
                    }                
                }                                                                                                                   
            }

            DbEntityValidationResult result = base.ValidateEntity(entityEntry, items);

            return result;
        }

        IQueryable<T> IDistrictManagerDb.Query<T>()
        {
            return Set<T>();
        }

        void IDistrictManagerDb.Add<T>(T entity)
        {
            Set<T>().Add(entity);
        }

        void IDistrictManagerDb.AddRange<T>(IEnumerable<T> entities)
        {
            Set<T>().AddRange(entities);
        }

        void IDistrictManagerDb.Update<T>(T entity)
        {
            Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }
        void IDistrictManagerDb.Remove<T>(T entity)
        {
            Set<T>().Remove(entity);
        }
        void IDistrictManagerDb.SaveChanges()
        {
            SaveChanges();
        }
    }
}