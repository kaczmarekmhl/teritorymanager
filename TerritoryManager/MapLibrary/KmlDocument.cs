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
        public Document Document 
        {
            get
            {
                return kmlDocument;
            }

            private set {}
        }

        private Document kmlDocument;
        private List<Placemark> placemarkList = new List<Placemark>();

        private PointInsidePolygon pointInside;

        public KmlDocument()
        {
            kmlDocument = Parse("<?xml version=\"1.0\"?><kml xmlns=\"http://earth.google.com/kml/2.2\"><Document></Document></kml>");
        }

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

            if (!TryParseCoordinates(longitude, latitude, out longitudeDouble, out latitudeDouble))
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
        /// Merges two kml docuements.
        /// </summary>
        /// <param name="kmlDocumentToMerge">KmlDocument to merge</param>
        public void MergeDocuments(KmlDocument kmlDocumentToMerge)
        {
            kmlDocument.Merge<Document>(kmlDocumentToMerge.Document);
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

                var styleParent = element.GetParent<Style>();

                if(styleParent != null)
                {
                    var newStyleId = styleParent.Id + '_' + fillColor;
                    
                    // Fix pair style id
                    foreach (var pair in kmlDocument.Flatten().OfType<Pair>().Where(p => p.StyleUrl.OriginalString == '#' + styleParent.Id))
                    {
                        pair.StyleUrl = new Uri('#' + newStyleId, UriKind.Relative);
                    }

                    styleParent.Id = newStyleId;

                    // Fix style map style id
                    foreach (var styleMap in kmlDocument.Flatten().OfType<StyleMapCollection>())
                    {
                        var newStyleMapId = styleMap.Id + '_' + fillColor;

                        foreach (var placemark in kmlDocument.Flatten().OfType<Placemark>().Where(p => p.StyleUrl.OriginalString == '#' + styleMap.Id))
                        {
                            placemark.StyleUrl = new Uri('#' + newStyleMapId, UriKind.Relative);
                        }

                        styleMap.Id = newStyleMapId;
                    }

                }
            }
        }

        /// <summary>
        /// Sets boundary name.
        /// </summary>
        /// <param name="name">Boundary name</param>
        public void SetBoundaryName(string name)
        {
            var polygon = kmlDocument.Flatten().OfType<Polygon>().First();

            if (polygon != null)
            {
                polygon.GetParent<Placemark>().Name = name;
            }
        }

        /// <summary>
        /// Detects if point is inside district bounday.
        /// </summary>
        /// <returns>Is point inside polygon</returns>
        public bool IsPointInsideBounday(string longitude, string latitude)
        {
            double longitudeDouble;
            double latitudeDouble;

            if (!TryParseCoordinates(longitude, latitude, out longitudeDouble, out latitudeDouble))
            {
                return false;
            }

            if (pointInside == null)
            {
                pointInside = new PointInsidePolygon(GetBoundaryCoordinates());
            }

            return pointInside.IsPointInside(new Vector { Latitude = latitudeDouble, Longitude = longitudeDouble });
        }
        
        /// <summary>
        /// Generates kml.
        /// </summary>
        /// <returns>Kml</returns>
        public override string ToString()
        {         
            var serializer = new Serializer();
            serializer.Serialize(kmlDocument);
            return serializer.Xml;
        }

        /// <summary>
        /// Retreives coordinates of district boundary.
        /// </summary>
        /// <returns>The coorinate collection.</returns>
        public CoordinateCollection GetBoundaryCoordinates()
        {
            var polygon = kmlDocument.Flatten().OfType<Polygon>().First();

            if (polygon == null)
            {
                return null;
            }

            return polygon.Flatten().OfType<CoordinateCollection>().First();
        }

        /// <summary>
        /// Tries to parse latitude and longitude from string.
        /// </summary>
        /// <returns>Is parse successfull</returns>
        public static bool TryParseCoordinates(string longitudeTxt, string latitudeTxt, out double longitude, out  double latitude)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            NumberStyles style = NumberStyles.Number;

            longitude = 0;
            latitude = 0;

            if (!double.TryParse(longitudeTxt, style, culture, out longitude)
                || !double.TryParse(latitudeTxt, style, culture, out latitude))
            {
                return false;
            }

            return true;
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
