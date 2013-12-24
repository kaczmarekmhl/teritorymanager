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
        List<Placemark> placemarkList = new List<Placemark>();
        XDocument xml;
        XNamespace ns;

        public KmlDocument(string kml)
        {
            xml = XDocument.Parse(kml);
            ns = xml.Root.Name.Namespace;
        }

        /// <summary>
        /// Add placemark that will be displayed on map.
        /// </summary>
        public void AddPlacemark(string name, string description, string longitude, string latitude)
        {
            placemarkList.Add(
                new Placemark { 
                    Name = name, 
                    Description = description, 
                    Longitude = longitude, 
                    Latitude = latitude});        
        }

        /// <summary>
        /// Changes colors of district boundary and fill.
        /// </summary>
        /// <param name="borderColor">Border color.</param>
        /// <param name="fillColor">Fill color.</param>
        public void ChangeBoundaryColor(string borderColor, string fillColor)
        {
            foreach (XElement element in xml.Descendants(ns + "LineStyle"))
            {
                element.Element(ns + "color").SetValue(borderColor);
            }

            foreach (XElement element in xml.Descendants(ns + "PolyStyle"))
            {
                element.Element(ns + "color").SetValue(fillColor);
            }
        }

        /// <summary>
        /// Generates kml with placemarks.
        /// </summary>
        /// <returns>Kml</returns>
        public string GetKmlWithPlacemarks()
        {
            XElement documentElement = xml.Descendants(ns + "Document").First();

            if (documentElement == null)
            {
                throw new Exception("Document node should exist in proper kml file");
            }

            documentElement.Add(GetFolderElementWithPlacemarks());

            return xml.ToString();
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
