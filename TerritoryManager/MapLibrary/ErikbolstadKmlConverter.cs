using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using SharpKml.Dom;
using System.Globalization;

namespace MapLibrary
{
    public class ErikbolstadKmlConverter
    {
        private const string format = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
"<kml xmlns = \"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\" xmlns:kml=\"http://www.opengis.net/kml/2.2\" xmlns:atom=\"http://www.w3.org/2005/Atom\">\r\n" +
"<Document>\r\n" +
    "<name>{0}</name>\r\n" +
    "<Style id = \"style0\">\r\n" +
        "<LineStyle>\r\n" +
            "<color> ff0000ff </color>\r\n" +
        "</LineStyle>\r\n" +
        "<PolyStyle>\r\n" +
            "<color> 5900ffaa</color>\r\n" +
        "</PolyStyle>\r\n" +
    "</Style>\r\n" +
    "<StyleMap id = \"stylemap_id0\">\r\n" +
        "<Pair>\r\n" +
            "<key> normal </key>\r\n" +
            "<styleUrl>#style0</styleUrl>" +
        "</Pair>\r\n" +
        "<Pair>\r\n" +
            "<key> highlight </key>\r\n" +
            "<styleUrl>#style</styleUrl>\r\n" +
        "</Pair>\r\n" +
    "</StyleMap>\r\n" +
    "<Style id=\"style\">\r\n" +
        "<LineStyle>\r\n" +
            "<color>ff0000ff</color>\r\n" +
        "</LineStyle>\r\n" +
        "<PolyStyle>\r\n" +
            "<color>5900ffaa</color>\r\n" +
        "</PolyStyle>\r\n" +
    "</Style>\r\n" +
    "<Placemark>\r\n" +
        "<name>{1}</name>\r\n" +
        "<open>1</open>\r\n" +
        "<styleUrl>#stylemap_id0</styleUrl>\r\n" +
        "<MultiGeometry>\r\n" +
            "<Polygon>\r\n" +
                "<outerBoundaryIs>\r\n" +
                    "<LinearRing>\r\n" +
                        "<coordinates>\r\n" +
                            "{2}\r\n" +
                        "</coordinates>\r\n" +
                    "</LinearRing>\r\n" +
                "</outerBoundaryIs>\r\n" +
            "</Polygon>\r\n" +
        "</MultiGeometry>\r\n" +
    "</Placemark>\r\n" +
"</Document>\r\n" +
"</kml>";

        public static string Convert(string filePath)
        {
            string result = String.Empty;

            var kmlDoc = new KmlDocument(File.ReadAllText(filePath));

            Placemark placemark = kmlDoc.Document.Features.ElementAt(1) as Placemark;
            LineString lineString = placemark.Geometry as LineString;

            StringBuilder coordinates = new StringBuilder();

            foreach (var c in lineString.Coordinates)
            {
                coordinates.AppendFormat(CultureInfo.InvariantCulture, "{0},{1},0 ", c.Longitude, c.Latitude);
            }            

            result = String.Format(CultureInfo.InvariantCulture, format, 
                Path.GetFileName(filePath),
                kmlDoc.Document.Name, 
                coordinates.ToString());

            return result;
        }
    }
}
