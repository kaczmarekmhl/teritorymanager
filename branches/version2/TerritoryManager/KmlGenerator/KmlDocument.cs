using KmlGenerator.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KmlGenerator
{
    public class KmlDocument
    {
        List<Placemark> placemarkList;

        public KmlDocument()
        {
            placemarkList = new List<Placemark>();
        }

        public void addPlacemark(string name, string description, string longitude, string latitude)
        {
            placemarkList.Add(
                new Placemark { 
                    Name = name, 
                    Description = description, 
                    Longitude = longitude, 
                    Latitude = latitude});        
        }

        public XDocument GenerateKml(string baseKmlPath)
        {
            XDocument xml = XDocument.Load(baseKmlPath);
            XNamespace ns = xml.Root.Name.Namespace;

            XElement documentElement = xml.Descendants(ns + "Document").First();

            if (documentElement == null)
            {
                throw new Exception("Document node should exist in proper kml file");
            }

            documentElement.Add(GetFolderElementWithPlacemarks());

            return xml;
        }

        private XElement GetFolderElementWithPlacemarks()
        {
            XElement folderElement = new XElement("Folder");

            folderElement.Add(new XElement("name", "Selected adresses"));

            foreach (var placemark in placemarkList)
            {
                folderElement.Add(GetPlacemarkElement(placemark));
            }

            return folderElement;
        }

        private XElement GetPlacemarkElement(Placemark placemark)
        {
            XElement placemarkElement = new XElement("Placemark");

            placemarkElement.Add(new XElement("name", placemark.Name));
            placemarkElement.Add(new XElement("description", new XCData(placemark.Description)));
            
            var point = new XElement("Point");
            point.Add(new XElement("coordinates", string.Format("{0},{1},0.0", placemark.Longitude, placemark.Latitude)));

            placemarkElement.Add(point);

            return placemarkElement;
        }
    }
}
