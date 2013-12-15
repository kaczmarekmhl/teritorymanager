using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public DistictManagerDb() : base("name=DefaultConnection")
        {

        }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<District> Districts { get; set; }

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