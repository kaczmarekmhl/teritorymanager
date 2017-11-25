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
                var properties = new Dictionary<string, string> { { "districtName", district.Name }, { "searchPhrase", district.SearchPhrase }, { "postCode", district.PostCode } };
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
                    if (stopwatch.Elapsed.TotalSeconds < 15)
                    {
                        eventName = "SearchFailFast";
                    }
                    else
                    {
                        eventName = "SearchFail";
                    }
                                        
                    properties.Add("ExceptionMessage", e.Message + " --- " + e.GetType());
                    properties.Add("ExceptionStackTrace", e.StackTrace);

                    if (e.InnerException != null)
                    {
                        properties.Add("InnerExceptionMessage", e.InnerException.Message + " --- " + e.InnerException.GetType());

                        if (e.InnerException.InnerException != null)
                        {
                            properties.Add("InerInnerExceptionMessage", e.InnerException.InnerException.Message + " --- " + e.InnerException.InnerException.GetType());
                        }
                    }

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