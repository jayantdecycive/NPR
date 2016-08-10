
using System.IO;

namespace cfacore.shared.service.email
{
	public class EmailContextModel
	{
		public string BaseUrl { get; set; }
		public string ThemeImagesUrl { get; set; }
		public string ThemeName { get {
			
			if( string.IsNullOrWhiteSpace( ThemeImagesUrl ) ) return string.Empty;
			return Path.GetFileName( ThemeImagesUrl.Replace("/images", string.Empty ) );

		} }
		public object Entity { get; set; }
	}
}
