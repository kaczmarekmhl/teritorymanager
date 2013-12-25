using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Collections.Generic;
using MVCApp.Models;
using System.Linq;
using MVCApp.Tests.Controllers.Mocks;
using MVCApp.Controllers;
using MVCApp.Tests.Mocks;

namespace MVCApp.Tests.Controllers.Persons
{
    [TestClass]
    public class SearchPersonsValidTest
    {
        public static List<AddressSearch.AdressProvider.Entities.Person> personListFromAddressProvider;
        private static PersonsController personsController;
        public static MockTerritoryDb MockDb;

        [TestInitialize()]
        public void TestInit()
        {
            MockDb = new MockTerritoryDb();
            MockDb.AddSet<MVCApp.Models.District>(TestData.Districts);
            MockDb.AddSet<MVCApp.Models.Person>(TestData.Persons);

            int postCode = 2750;

            var personListFromMockDb = MockDb.Query<MVCApp.Models.District>().Single(d => d.Id == "1").PersonsFoundInDistrict;
            
            personListFromAddressProvider = new List<AddressSearch.AdressProvider.Entities.Person>();
            foreach (var person in personListFromMockDb)
            {
                personListFromAddressProvider.Add(
                    new AddressSearch.AdressProvider.Entities.Person()
                    {
                        Name = person.Name,
                        Lastname = person.Lastname,
                        StreetAddress = person.StreetAddress,
                        PostCode = Convert.ToString(postCode)
                    }
                    );
            }

            var personDictionary = new Dictionary<int, List<AddressSearch.AdressProvider.Entities.Person>>();
            personDictionary[postCode] = personListFromAddressProvider;

            MockAddressProvider mockAddressProvider = new MockAddressProvider(personDictionary);
            personsController = new PersonsController(MockDb, mockAddressProvider);
        }
        
        [TestMethod]
        public void SearchPersons_ListReturnedFromDbIsValid()
        {
            PartialViewResult result = personsController.Search("1") as PartialViewResult;

            var personList = result.Model as List<Person>;

            var expectedPersonList = MockDb.Query<MVCApp.Models.District>().Single(d => d.Id == "1").PersonsFoundInDistrict;

            var actualPerson = personList.ElementAt(0);
            var expectedPerson = expectedPersonList.ElementAt(0);
            Assert.IsTrue(actualPerson.Id == expectedPerson.Id);
            Assert.IsTrue(actualPerson.Name == expectedPerson.Name);
            Assert.IsTrue(actualPerson.Lastname == expectedPerson.Lastname);

            actualPerson = personList.ElementAt(1);
            expectedPerson = expectedPersonList.ElementAt(1);
            Assert.IsTrue(actualPerson.Id == expectedPerson.Id);
            Assert.IsTrue(actualPerson.Name == expectedPerson.Name);
            Assert.IsTrue(actualPerson.Lastname == expectedPerson.Lastname);
        }

        [TestMethod]
        public void SearchPersons_ListReturnedFromKrakIsValid()
        {
            PartialViewResult result = personsController.Search("1") as PartialViewResult;

            var personList = result.Model as List<Person>;

            var actualPerson = personList.ElementAt(0);
            var expectedPerson = personListFromAddressProvider.ElementAt(0);
            Assert.IsTrue(actualPerson.Name == expectedPerson.Name);
            Assert.IsTrue(actualPerson.Lastname == expectedPerson.Lastname);
            Assert.IsTrue(actualPerson.StreetAddress == expectedPerson.StreetAddress);

            actualPerson = personList.ElementAt(1);
            expectedPerson = personListFromAddressProvider.ElementAt(1);
            Assert.IsTrue(actualPerson.Name == expectedPerson.Name);
            Assert.IsTrue(actualPerson.Lastname == expectedPerson.Lastname);
            Assert.IsTrue(actualPerson.StreetAddress == expectedPerson.StreetAddress);
        }

        [TestMethod]
        public void SearchPersons_DeleteEntriesFromListReturnedFromKrak()
        {          
            int[] IdOfPersonsToDelete = new int[]{ 2 };
            personsController.Delete(IdOfPersonsToDelete);

            PartialViewResult result = personsController.Search("1") as PartialViewResult;
            var personList = result.Model as List<Person>;

            Assert.AreEqual(1, personList.Count());

            var actualPerson = personList.ElementAt(0);
            var expectedPerson = personListFromAddressProvider.ElementAt(0);
            Assert.IsTrue(actualPerson.Name == expectedPerson.Name);
            Assert.IsTrue(actualPerson.Lastname == expectedPerson.Lastname);
            Assert.IsTrue(actualPerson.StreetAddress == expectedPerson.StreetAddress);
        }

        [TestMethod]
        public void SearchPersons_RestoreRemovedEntries()
        {
            int[] IdOfPersonsToRestore = new int[] { 2 };
            personsController.Delete(IdOfPersonsToRestore);
            personsController.Restore(IdOfPersonsToRestore);

            PartialViewResult result = personsController.Search("1") as PartialViewResult;
            var personList = result.Model as List<Person>;

            Assert.AreEqual(2, personList.Count());

            var actualPerson = personList.ElementAt(0);
            var expectedPerson = personListFromAddressProvider.ElementAt(0);
            Assert.IsTrue(actualPerson.Name == expectedPerson.Name);
            Assert.IsTrue(actualPerson.Lastname == expectedPerson.Lastname);
            Assert.IsTrue(actualPerson.StreetAddress == expectedPerson.StreetAddress);

            actualPerson = personList.ElementAt(1);
            expectedPerson = personListFromAddressProvider.ElementAt(1);
            Assert.IsTrue(actualPerson.Name == expectedPerson.Name);
            Assert.IsTrue(actualPerson.Lastname == expectedPerson.Lastname);
            Assert.IsTrue(actualPerson.StreetAddress == expectedPerson.StreetAddress);
        }
    }
}
