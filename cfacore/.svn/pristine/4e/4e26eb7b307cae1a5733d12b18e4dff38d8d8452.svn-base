
#region Imports

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Web;
using System.Xml;
using cfacore.domain.store;
using cfacore.shared.domain.user;
using cfacore.shared.service.Helpers;

#endregion

namespace cfacore.shared.service.Locations
{
	public class LocationAPIService
	{
		#region Settings
		
		public const string W2GIUrl = "http://hosted.where2getit.com/lite?xml_request=";
        public const string W2GIUrlStaging = "http://api.where2stageit.com/rest?xml_request=";
        public const string LimitNode = "<limit>20</limit>";
		public const string LocalHttpIncapClientIP = "66.56.44.34";
		public const int SearchResultsDefaultLimitLocations = 50;
		public const int SearchResultsDefaultLimitAddresses = 20;
		public const int SearchResultsDefaultRadius = 20;

		#endregion

		#region GetBy API Searches

		public IEnumerable<LocationResult> GetByLatLong( string lat, string lon, int radiusInMiles )
        {
            string r = String.Format("<request><appkey>{0}</appkey><formdata id='locatorsearch'>" + 
				"<geolocs><geoloc><latitude>{1}</latitude><longitude>{2}</longitude></geoloc>" + 
				"</geolocs><searchradius>{3}</searchradius>" + LimitNode + "</formdata></request>",
                APIKey, lat, lon, CheckRadius( radiusInMiles ) );

            XmlDocument xmlDoc = EncodeLocationServiceRequest(r);
			if ( xmlDoc.SelectAttributeValue("response/collection", "name") != "poi")
				throw new ApplicationException( "Unexpected collection name [ 'poi' ]" );

            XmlNodeList nodes = xmlDoc.SelectNodes("response/collection/poi");
            int counter = 0;
            return new List<LocationResult>( from XmlNode n in nodes 
						   where counter++ < SearchResultsDefaultLimitLocations 
						   select n.ToLocation() );
        }

        public IEnumerable<LocationResult> GetByGeoIPAddress( string geoip, int radiusInMiles )
        {
            string r = String.Format("<request><appkey>{0}</appkey><formdata id='geoip'>" + 
				"<geolocs><geoloc><ipaddress>{1}</ipaddress></geoloc></geolocs><searchradius>" + 
				"{2}</searchradius>" + LimitNode + "</formdata></request>",
                APIKey, geoip, CheckRadius( radiusInMiles ) );

            XmlDocument xmlDoc = EncodeLocationServiceRequest( r );

	        XmlNodeList nodes = xmlDoc.SelectNodes("response/collection/geoip");
            string lat = string.Empty;
            string lon = string.Empty;
	        if( nodes == null ) throw new ApplicationException("Unable to locate geoip node within request");
	        foreach (XmlNode n in nodes)
            {
				if( n == null ) throw new ApplicationException("Invalid geoip node");
	            lat = n.SelectInnerText("latitude");
	            lon = n.SelectInnerText("longitude");
	            break;
            }

            return GetByLatLong( lat, lon, radiusInMiles );
        }

		public IEnumerable<LocationResult> GetByPostalCodeAndLocations(
			string postalCode, int radiusInMiles, IEnumerable<IStore> locationsFilter )
		{
			return GetByPostalCodeAndClientKeys( postalCode, radiusInMiles, 
				locationsFilter.Select( o => o.LocationNumber ).ToList() );
		}

        public IEnumerable<LocationResult> GetByPostalCodeAndClientKeys( string postalCode, 
			int radiusInMiles, ICollection<string> clientKeys )
        {
			if( ! clientKeys.Any() ) return new List<LocationResult>();

            List<LocationResult> locs = new List<LocationResult>();
            string r = String.Format("<request><appkey>{0}</appkey><formdata id='locatorsearch'>" + 
				"<geolocs><geoloc><addressline>{1}</addressline></geoloc></geolocs><searchradius>" + 
				"{2}</searchradius><where><clientkey><in>{3}</in></clientkey></where>" + LimitNode + "</formdata></request>",
                APIKey, postalCode, CheckRadius( radiusInMiles ), string.Join( ",", clientKeys ) );

            XmlDocument xmlDoc = EncodeLocationServiceRequest( r );
            if (xmlDoc.SelectAttributeValue("response/collection", "name" ) == "poi")
            {
                int counter = 0;
                foreach (XmlNode n in xmlDoc.SelectNodesSafe("response/collection/poi"))
					if (counter++ < SearchResultsDefaultLimitLocations)
						locs.Add( n.ToLocation() );
            }
            else
            {
				// Custom invalid data exception for 'no locations found' condition
				// For ex:  Unable to process location API search message [ Code = 5001, Message = 'Your search for Chick-fil-A restaurants near [ 90210 ] found no locations.  Please try another address to find a restaurant near you.', Request = '<request><appkey>B41D8DF8-9AE7-11E2-8B34-AC5F003085D0</appkey><formdata id='locatorsearch'><geolocs><geoloc><addressline>90210</addressline></geoloc></geolocs><searchradius>25</searchradius><where><clientkey><in>01386,70022,00001,00002,00250,00636,00011,00001,00005,00009,00001,00005,00009,00001,00005,00009,00001,02007,02007,01982</in></clientkey></where><limit>20</limit></formdata></request>' ]
				if( xmlDoc.SelectInnerText( "response/message/text" ).Contains( "no locations" ) )
					throw new InvalidDataException( xmlDoc.SelectInnerText( "response/message/text" ) );

				// Addresses: "Unable to process location API search message [ Code = 5000, Message = 'The address you entered (89273) is not found on the map.  Please try another address to find a restaurant near you."
				if( xmlDoc.SelectInnerText( "response/message/text" ).Contains( "not found on the map" ) )
					throw new InstanceNotFoundException( xmlDoc.SelectInnerText( "response/message/text" ) );

	            throw new ApplicationException( string.Format( "Unable to process location " +
					"API search message [ Code = {0}, Message = '{1}', Request = '{2}' ]",
					xmlDoc.SelectAttributeValue( "response", "code" ),
					xmlDoc.SelectInnerText( "response/message/text" ), r ) );
            }

            return locs;
        }

        public IEnumerable<LocationResult> GetByState( string stateCode )
        {
			// "Please enter an address" is currently returned from API
			throw new NotSupportedException();
			
			/* List<LocationResult> locs = new List<LocationResult>();
            string r = String.Format("<request><appkey>{0}</appkey><formdata id='locatorsearch'>" + 
				"<geolocs><geoloc><state>{1}</state></geoloc></geolocs>" + LimitNode + "</formdata></request>",
                APIKey, stateCode );

            XmlDocument xmlDoc = EncodeLocationServiceRequest( r );
            if (xmlDoc.SelectAttributeValue("response/collection", "name" ) == "poi")
            {
                int counter = 0;
                foreach (XmlNode n in xmlDoc.SelectNodesSafe("response/collection/poi"))
					if (counter++ < SearchResultsDefaultLimitLocations)
						locs.Add( n.ToLocation() );
            }

            return locs; */
        }

        public IEnumerable<LocationResult> GetByPostalCode( string postalCode, int radiusInMiles )
        {
            List<LocationResult> locs = new List<LocationResult>();
            string r = String.Format("<request><appkey>{0}</appkey><formdata id='locatorsearch'>" + 
				"<geolocs><geoloc><addressline>{1}</addressline></geoloc></geolocs><searchradius>" + 
				"{2}</searchradius>" + LimitNode + "</formdata></request>",
                APIKey, postalCode, CheckRadius( radiusInMiles ) );

            XmlDocument xmlDoc = EncodeLocationServiceRequest( r );
            if (xmlDoc.SelectAttributeValue("response/collection", "name" ) == "poi")
            {
                int counter = 0;
                foreach (XmlNode n in xmlDoc.SelectNodesSafe("response/collection/poi"))
					if (counter++ < SearchResultsDefaultLimitLocations)
						locs.Add( n.ToLocation() );
            }

            return locs;
        }

		public LocationSearchResult GetByAddress( string addressStreetLine1, int radiusInMiles )
        {
            string r = String.Format("<request><appkey>{0}</appkey><formdata id='locatorsearch'>" + 
				"<geolocs><geoloc><addressline>{1}</addressline></geoloc></geolocs><searchradius>{2}" + 
				"</searchradius>" + LimitNode + "</formdata></request>",
                APIKey, addressStreetLine1, CheckRadius( radiusInMiles ) );
            
            XmlDocument xmlDoc = EncodeLocationServiceRequest( r );
            LocationSearchResult results = new LocationSearchResult();
	        int counter;

	        switch (xmlDoc.SelectAttributeValue( "response/collection", "name" ))
	        {
		        case "poi":
				    List<LocationResult> locs = results.Locations;
				    counter = 0;
				    foreach (XmlNode n in xmlDoc.SelectNodesSafe("response/collection/poi"))
					    if (counter++ < SearchResultsDefaultLimitAddresses)
						    locs.Add(n.ToLocation());
			        break;

				case "multiple_address":
				    List<Address> addresses = results.MultipleAddresses;
				    counter = 0;
				    foreach (XmlNode n in xmlDoc.SelectNodesSafe("response/collection/address"))
					    if (counter++ < SearchResultsDefaultLimitAddresses)
						    addresses.Add(n.ToAddress());
			        break;
	        }

	        return results;
        }

        public IEnumerable<LocationResult> GetByGeoIPAddress( int radiusInMiles )
        {
	        string ip = HttpContext.Current.Request.IsLocal 
				? LocalHttpIncapClientIP 
		        : HttpContext.Current.Request.ServerVariables[ "HTTP_INCAP_CLIENT_IP" ];

            return GetByGeoIPAddress( ip, radiusInMiles );
		}

		#endregion

		#region Helpers [ EncodeLocationServiceRequest, CheckRadius, APIKey ]

		private static string APIKey { get 
		{
			return ConfigurationManager.AppSettings["LocationAPIWebServiceKey"] ?? string.Empty;
		} }

		private static string APIUrl { get 
		{
			string url = ConfigurationManager.AppSettings["LocationAPIWebServiceUrl"] ?? string.Empty;

			// Support Location API URL overriding via the QS for staging validation
			if( HttpContext.Current != null )
			{
				string qsu = HttpContext.Current.Request.QueryString[ "LocationAPIWebServiceUrl" ] ?? string.Empty;
				if( qsu != string.Empty ) url = qsu;
			}

			if( string.IsNullOrWhiteSpace( url ) )
				throw new ApplicationException( "Invalid Location API URL .. Please check AppSettings [ LocationAPIWebServiceUrl ]" );

			return url;

			//// ReSharper disable RedundantAssignment
			//string k = W2GIUrl;
			//#if DEBUG
			//	k = W2GIUrlStaging;
			//#endif
			//return k;
			//// ReSharper restore RedundantAssignment
		} }

		public XmlDocument EncodeLocationServiceRequest( string request )
		{
			string urlEncode = HttpUtility.UrlEncode(request);
			if (urlEncode == null) return null;

			string url = APIUrl + urlEncode.Replace("+", "%20").Replace("'", "%22");

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			HttpWebResponse res = (HttpWebResponse)req.GetResponse();
			Stream stream = res.GetResponseStream();
			if( stream == null ) throw new ApplicationException( "Invalid response stream / HTTP web context" );
			StreamReader sr = new StreamReader( stream, System.Text.Encoding.Default );
			string backstr = sr.ReadToEnd();
			sr.Close();
			res.Close();
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(new StringReader(backstr));
			return xmlDoc;
		}

		public static int CheckRadius( int radiusInMiles )
		{
			if( radiusInMiles <= 0 )
				radiusInMiles = SearchResultsDefaultRadius;
			
			return radiusInMiles;
		}

		#endregion
	}
}

#region Archive

//l.acceptscfacard = n.SelectSingleNode("acceptscfacard").InnerText;
//l.playground = n.SelectSingleNode("playground").InnerText;
//l.servesbreakfast = n.SelectSingleNode("servesbreakfast").InnerText;
//l.status = n.SelectSingleNode("status").InnerText;
//l.LocationNumber = n.SelectSingleNode("uid").InnerText;
//l.marketablename = n.SelectSingleNode("marketablename").InnerText;
//l.marketableurl = n.SelectSingleNode("marketableurl").InnerText;
//l.offersonlineordering = n.SelectSingleNode("offersonlineordering").InnerText;
//l.offerswireless = n.SelectSingleNode("offerswireless").InnerText;
//l.operatorname = n.SelectSingleNode("operatorname").InnerText;
//l.locationcode = n.SelectSingleNode("locationcode").InnerText;
//l.conceptcode = n.SelectSingleNode("conceptcode").InnerText;
//streetAddress.County = n.SelectSingleNode("country").InnerText;
//l.county = n.SelectSingleNode("county").InnerText;
// FaxNumber not currently supported:
// l.LocationContact.FaxNumber = FaxNumber.ParsePhoneNumber(n.SelectSingleNode("fax").InnerText);
//l.hasdiningroom = n.SelectSingleNode("hasdiningroom").InnerText;
//l.hasdrivethru = n.SelectSingleNode("hasdrivethru").InnerText;
//l.icon = n.SelectSingleNode("icon").InnerText;

/*
    * Do this in web application
private List<LocationResult> FindLocationsByIP(string radius)
{
    string ip = String.Empty;
    if (HttpContext.Current.Request.IsLocal)
        ip = "66.56.44.34";//Context.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
    else
        ip = Context.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
    return FindLocationsByIP(ip, radius);
}
*/

#endregion