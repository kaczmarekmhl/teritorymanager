using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLibrary
{
    public class KmlValidator
    {
        /// <summary>
        /// Validates kml file.
        /// </summary>
        /// <param name="xml">Xml in string.</param>
        /// <returns>Validation result</returns>
        public static bool isValid(string xml)
        {
            try
            {
                KmlDocument.Parse(xml);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
