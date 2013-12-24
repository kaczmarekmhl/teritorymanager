using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KmlGenerator
{
    public class KmlHelper
    {
        /// <summary>
        /// Validates kml file.
        /// </summary>
        /// <param name="kml">Kml in string.</param>
        /// <returns>Validation result</returns>
        public static bool isValidKml(string kml)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(kml);          
            }
            catch (XmlException) 
            {
                return false;
            }

            return true;
        }
    }
}
