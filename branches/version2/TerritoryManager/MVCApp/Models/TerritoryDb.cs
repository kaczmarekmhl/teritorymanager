using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public interface ITerritoryDb : IDisposable
    {
        IQueryable<T> Query<T>() where T : class;
        void Add<T>(T entity) where T : class;
        void AddRange<T>(IEnumerable<T> entities) where T : class;
        void Update<T>(T entity) where T : class;
        void Remove<T>(T entity) where T : class;
        void SaveChanges();
    }

    public class TerritoryDb : DbContext, ITerritoryDb
    {   
        public TerritoryDb() : base("name=DefaultConnection")
        {

        }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Territory> Territories { get; set; }

        IQueryable<T> ITerritoryDb.Query<T>()
        {
            return Set<T>();
        }

        void ITerritoryDb.Add<T>(T entity)
        {
            Set<T>().Add(entity);
        }

        void ITerritoryDb.AddRange<T>(IEnumerable<T> entities)
        {
            Set<T>().AddRange(entities);
        }

        void ITerritoryDb.Update<T>(T entity)
        {
            Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }
        void ITerritoryDb.Remove<T>(T entity)
        {
            Set<T>().Remove(entity);
        }
        void ITerritoryDb.SaveChanges()
        {
            SaveChanges();        
        }
    }
}