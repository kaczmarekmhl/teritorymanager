namespace MapLibrary
{
    using SharpKml.Base;
    using SharpKml.Dom;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PointInsidePolygon
    {
        double maxLatitude;
        double minLatitude;

        double maxLongitude;
        double minLongitude;

        CoordinateCollection coordinates;

        public PointInsidePolygon(CoordinateCollection coordinateCollection)
        {
            coordinates = coordinateCollection;
            InitMinMaxCoordinates();
        }

        /// <summary>
        /// Detects if point is inside polygon.
        /// </summary>
        /// <param name="vector">Point coordinates</param>
        /// <returns>Is point inside polygon</returns>
        public bool IsPointInside(Vector vector)
        {
            if (vector.Latitude < minLatitude
                || vector.Latitude > maxLatitude
                || vector.Longitude < minLongitude
                || vector.Longitude > maxLongitude)
            {
                return false;
            }

            int i, j = 0;
            bool inside = false;
            for (i = 0, j = coordinates.Count - 1; i < coordinates.Count; j = i++)
            {
                if (((coordinates.ElementAt(i).Latitude > vector.Latitude) != (coordinates.ElementAt(j).Latitude > vector.Latitude))
                    && (vector.Longitude < (coordinates.ElementAt(j).Longitude - coordinates.ElementAt(i).Longitude) * (vector.Latitude - coordinates.ElementAt(i).Latitude)
                    / (coordinates.ElementAt(j).Latitude - coordinates.ElementAt(i).Latitude) + coordinates.ElementAt(i).Longitude))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        /// <summary>
        /// Initializes min and max coordinates.
        /// </summary>
        private void InitMinMaxCoordinates()
        {
            minLatitude = minLongitude = 1000;
            maxLatitude = maxLongitude = -1000;

            foreach (var vector in coordinates)
            {
                if (minLatitude > vector.Latitude)
                {
                    minLatitude = vector.Latitude;
                }
                if (maxLatitude < vector.Latitude)
                {
                    maxLatitude = vector.Latitude;
                }

                if (minLongitude > vector.Longitude)
                {
                    minLongitude = vector.Longitude;
                }
                if (maxLongitude < vector.Longitude)
                {
                    maxLongitude = vector.Longitude;
                }

            }
        }
    }
}