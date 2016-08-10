
#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#endregion

namespace cfacore.shared.service.Helpers
{
	public static class Xml
	{
		public static string SelectInnerText( this XmlNode n, string key )
		{
			return SelectInnerText<string>( n, key ) ?? string.Empty;
		}

		public static T SelectInnerText<T>( this XmlNode n, string key )
		{
			if( n == null ) return default( T );
			XmlNode c = n.SelectSingleNode( key );
			return c == null ? default( T ) : c.InnerText.ToType<T>();
		}

		public static string SelectAttributeValue( this XmlDocument doc, string key, string attribute )
		{
			return SelectAttributeValue<string>( doc, key, attribute ) ?? string.Empty;
		}

		public static T SelectAttributeValue<T>( this XmlDocument doc, string key, string attribute )
		{
			if( doc == null ) return default( T );
			XmlNode c = doc.SelectSingleNode( key );
			if( c == null ) return default( T );
			if( c.Attributes == null ) return default( T );
			XmlAttribute a = c.Attributes[ attribute ];
			return a == null ? default( T ) : a.Value.ToType<T>();
		}

	    public static IEnumerable<XmlNode> SelectNodesSafe( this XmlDocument doc, string xpath )
	    {
		    XmlNodeList l = null;
			try { l = doc.SelectNodes(xpath); } catch (Exception) {} 
			return l == null ? new List<XmlNode>() 
				: new List<XmlNode>( l.Cast<XmlNode>() );
	    }
	}
}
