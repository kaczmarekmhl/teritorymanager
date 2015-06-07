using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MVCApp.Controllers;
using MVCApp.Helpers;
using MVCApp.Models;
using System.Web.Mvc;

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

                var search = new SearchAddress(db);
                
                search.SetProgressMessage = new SearchAddress.SetProgressMessageDelegate(SetProgressInClient);

                try
                {
                    var personList = search.SearchAndPersistNewPersonList(district);
                    Clients.Caller.searchComplete(personList.Count != 0);
                }
                catch (Exception e)
                {
                    Clients.Caller.setProgressMessage("Something wrong happened: " + e.Message);
                }                
            }
        }

        private void SetProgressInClient(string message)
        {
            Clients.Caller.setProgressMessage(message);
        } 
    }
}