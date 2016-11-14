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

namespace MVCApp.Hubs
{
    public class MigrationHub : Hub
    {
        public void Migration()
        {
            const int currentMigrationVersion = 1;

            try
            {
                using (var db = new DistictManagerDb())
                {
                    int districtCount = db.Districts.Count(d => d.MigrationVersion < currentMigrationVersion);
                    int progress = 1;

                    foreach (var district in db.Districts.Where(d => d.MigrationVersion < currentMigrationVersion))
                    {
                        string progressMessage = String.Format(
                            CultureInfo.InvariantCulture, 
                            "Progress: {0}\\{1} ({2}))",
                            progress,
                            districtCount,
                            district.Name);

                        SetProgressInClient(progressMessage);

                        int personCount = db.Persons.Count(p => p.District.Id == district.Id && p.MigrationVersion < currentMigrationVersion);
                        int progressPerson = 1;
                        foreach (var person in db.Persons.Where(p => p.District.Id == district.Id && p.MigrationVersion < currentMigrationVersion))
                        {
                            PersonEncrypt(person, db, currentMigrationVersion);

                            SetProgressInClient(progressMessage
                                + String.Format(
                                    CultureInfo.InvariantCulture,
                                    " Person {0} from {1}",
                                    progressPerson++,
                                    personCount));
                        }

                        district.MigrationVersion = currentMigrationVersion;
                        db.Entry(district).State = EntityState.Modified;
                        db.SaveChanges();

                        progress++;
                    }

                    Clients.Caller.migrationComplete(true);
                }
            }
            catch (Exception e)
            {
                Clients.Caller.migrationError(String.Concat(e.Message, e.InnerException?.Message));
            }
            
        }

        private void PersonEncrypt(Person person, DistictManagerDb db, int currentMigrationVersion)
        {
            //person.StreetAddress = person.StreetAddress;
            //person.Longitude = person.Longitude;
            //person.Latitude = person.Latitude;

            person.Lastname = person.Lastname;
            person.TelephoneNumber = person.TelephoneNumber;
            person.MigrationVersion = currentMigrationVersion;

            db.Entry(person).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void SetProgressInClient(string message)
        {
            Clients.Caller.setProgressMessage(message);
        } 
    }
}