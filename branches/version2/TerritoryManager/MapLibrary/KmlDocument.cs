using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLibrary
{
    public class KmlDocument
    {
        private Document kmlDocument;
        private List<Placemark> placemarkList = new List<Placemark>();

        public KmlDocument(string xml)
        {
            kmlDocument = Parse(xml);
        }

        /// <summary>
        /// Add placemark that will be displayed on map.
        /// </summary>
        public void AddPlacemark(string name, string description, string longitude, string latitude)
        {
            double longitudeDouble; 
            double latitudeDouble;

            CultureInfo culture = CultureInfo.InvariantCulture;
            NumberStyles style = NumberStyles.Number;

            if (!double.TryParse(longitude, style, culture, out longitudeDouble) 
                || !double.TryParse(latitude, style, culture, out latitudeDouble))
            {
                return;
            }

            placemarkList.Add(
                new Placemark
                {
                    Name = name,
                    Description = new Description { Text = description },
                    Geometry = new Point { Coordinate = new Vector(latitudeDouble, longitudeDouble) }
                });
        }

        /// <summary>
        /// Generates kml with placemarks.
        /// </summary>
        /// <returns>Kml</returns>
        public string GetKmlWithPlacemarks()
        {
            Folder folderAddresses = new Folder();

            folderAddresses.Id = "SelectedAdresses";
            folderAddresses.Name = "Selected adresses";

            foreach (var placemark in placemarkList)
            {
                folderAddresses.AddFeature(placemark);
            }

            kmlDocument.AddFeature(folderAddresses);

            return ToString();
        }

        /// <summary>
        /// Changes colors of district boundary and fill.
        /// </summary>
        /// <param name="borderColor">Border color.</param>
        /// <param name="fillColor">Fill color.</param>
        public void ChangeBoundaryColor(string borderColor, string fillColor)
        {
            foreach (var element in kmlDocument.Flatten().OfType<LineStyle>())
            {
                element.Color = Color32.Parse(borderColor);
            }

            foreach (var element in kmlDocument.Flatten().OfType<PolygonStyle>())
            {
                element.Color = Color32.Parse(fillColor);
            }
        }


        /// <summary>
        /// Generates kml.
        /// </summary>
        /// <returns>Kml</returns>
        public override string ToString()
        {
            ChangeBoundaryColor("ff0000ff", "5950c24a");
            
            var serializer = new Serializer();
            serializer.Serialize(kmlDocument);
            return serializer.Xml;
        }


        /// <summary>
        /// Parse kml document.
        /// </summary>
        /// <param name="xml">Xml in string to parse</param>
        /// <returns>Kml document</returns>
        public static Document Parse(string xml)
        {
            var kmlParser = new Parser();
            kmlParser.ParseString(xml, false);

            var kml = kmlParser.Root as Kml;

            if (kml == null)
            {
                throw new ArgumentException("Invalid xml file");
            }

            var document = kml.Feature as Document;

            if (document == null)
            {
                throw new ArgumentException("Invalid xml file");
            }

            return document;  
        }
    }
}
