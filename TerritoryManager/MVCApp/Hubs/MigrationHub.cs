using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MVCApp.Controllers;
using MVCApp.Helpers;
using MVCApp.Models;
using System.Web.Mvc;
using MVCApp.Translate;
using Microsoft.ApplicationInsights;
using System.Globalization;
using System.Threading;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Transactions;
using IsolationLevel = System.Data.IsolationLevel;

namespace MVCApp.Hubs
{
    public class MigrationHub : Hub
    {
        //public const int CurrentMigrationVersion = 1;

        public void Migration()
        {
            //This migration is already completed in production
            return;

            /*try
            {
                using (var db = new DistictManagerDb())
                {

                    District district = db.Districts.FirstOrDefault(d => d.MigrationVersion < CurrentMigrationVersion);

                    if (district == null)
                    {
                        Clients.Caller.migrationComplete(true);
                        return;
                    }

                    string progressMessage = String.Format(
                        CultureInfo.InvariantCulture,
                        "{0} discricts left... Migrating {1}",
                        GetNotMigratedDistrictCount(db, CurrentMigrationVersion),
                        district.Name);

                    SetProgressInClient(progressMessage + " Loading person list...");

                    int progressPerson = 1;
                    List<int> personIdList = db.Persons.Where(p => p.District.Id == district.Id && p.MigrationVersion < CurrentMigrationVersion)
                        .Select(p => p.Id).ToList();
                    int personCount = personIdList.Count();

                    Parallel.ForEach(personIdList,
                        new ParallelOptions { MaxDegreeOfParallelism = 2 },
                        (personId) =>
                    {
                        using (var db2 = new DistictManagerDb())
                        {
                            using (var scope = new TransactionScope(TransactionScopeOption.Required,
                                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead }))
                            {
                                Person person = db2.Persons.FirstOrDefault(p => p.Id == personId);

                                if (person == null)
                                {
                                    return;
                                }

                                PersonEncrypt(person, db2, CurrentMigrationVersion);
                                scope.Complete();

                                Interlocked.Increment(ref progressPerson);
                                SetProgressInClient(progressMessage
                                    + String.Format(CultureInfo.InvariantCulture, " Processing person {0} from {1}", progressPerson, personCount));
                            }
                        }
                    });

                    district.MigrationVersion = CurrentMigrationVersion;
                    db.Entry(district).State = EntityState.Modified;
                    db.SaveChanges();

                    Clients.Caller.migrationComplete(GetNotMigratedDistrictCount(db, CurrentMigrationVersion) == 0);
                }
            }
            catch (Exception e)
            {
                Clients.Caller.migrationError(String.Concat(e.Message, e.InnerException?.Message));
            }
            */
        }
        /*
        private int GetNotMigratedDistrictCount(DistictManagerDb db, int currentMigrationVersion)
        {
            return db.Districts.Count(d => d.MigrationVersion < currentMigrationVersion);
        }

        private void PersonEncrypt(Person person, DistictManagerDb db, int currentMigrationVersion)
        {
            person.Migrate(currentMigrationVersion);

            db.Entry(person).State = EntityState.Modified;
            db.SaveChanges();
        }*/

        private void SetProgressInClient(string message)
        {
            Clients.Caller.setProgressMessage(message);
        }
    }
}