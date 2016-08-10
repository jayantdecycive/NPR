using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfacore.shared.domain.store
{
    public class GeographicCoordinate:IGeographicCoordinate
    {
        public GeographicCoordinate(double Latitude, double Longitude)
        {
            this._Latitude = Latitude;
            this._Longitude = Longitude;
        }
        public GeographicCoordinate() { 
        
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
        private double _Latitude = 0.0;

        public double Latitude
        {
            get{ return this._Latitude;}

            set{ this._Latitude = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
        private double _Longitude = 0.0;

        public double Longitude
        {
            get{ return this._Longitude;}

            set{ this._Longitude = value;}
        }

        public override string ToString()
        {
            return string.Format("({0},{1})",Latitude,Longitude);
        }
    }
}
