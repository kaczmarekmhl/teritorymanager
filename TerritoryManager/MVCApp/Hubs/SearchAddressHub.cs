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

namespace MVCApp.Hubs
{
    public class SearchAddressHub : Hub
    {
        public void Search(int id)
        {
            using(var db = new DistictManagerDb())
            {
                var district = db.Districts.Find(id);
                if (district == null)
                {
                    throw new ArgumentException("id");
                }
                
                //Telemetry - start
                var telemetry = new TelemetryClient();
                var properties = new Dictionary<string, string> { { "districtName", district.Name } };
                var metrics = new Dictionary<string, double> ();
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                string eventName = string.Empty;
                //Telemetry - end

                try
                {
                    var search = new SearchAddress(db) { SetProgressMessage = SetProgressInClient };
                    var personList = search.SearchAndPersistNewPersonList(district);
                    Clients.Caller.searchComplete(personList.Count != 0);

                    district.LastSearchUpdate = DateTime.Now;
                    db.Entry(district).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //Telemetry - start
                    metrics.Add("PersonCount", personList.Count);
                    eventName = "SearchComplete";
                    //Telemetry - end
                }
                catch (Exception e)
                {
                    Clients.Caller.searchError(string.Format(Strings.SearchAdressesError, e.Message));

                    //Telemetry - start
                    eventName = "SearchFail";
                    //Telemetry - end
                }

                //Telemetry - start
                stopwatch.Stop();
                metrics.Add("ElapsedSeconds", Math.Ceiling(stopwatch.Elapsed.TotalSeconds));

                if (!string.IsNullOrEmpty(eventName))
                {
                    telemetry.TrackEvent(eventName, properties, metrics);
                }
                //Telemetry - end
            }
        }

        private void SetProgressInClient(string message)
        {
            Clients.Caller.setProgressMessage(message);
        } 
    }
}