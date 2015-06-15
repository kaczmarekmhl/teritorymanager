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
                
                try
                {
                    var search = new SearchAddress(db) { SetProgressMessage = SetProgressInClient };
                    var personList = search.SearchAndPersistNewPersonList(district);
                    Clients.Caller.searchComplete(personList.Count != 0);
                }
                catch (Exception e)
                {
                    Clients.Caller.searchError(string.Format(Strings.SearchAdressesError, e.Message));
                }                
            }
        }

        private void SetProgressInClient(string message)
        {
            Clients.Caller.setProgressMessage(message);
        } 
    }
}