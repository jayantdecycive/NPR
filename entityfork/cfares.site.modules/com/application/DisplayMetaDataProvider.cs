
#region Imports

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

#endregion

namespace cfares.site.modules.com.application
{
	public class DisplayMetaDataProvider : DataAnnotationsModelMetadataProvider
	{
		protected override ModelMetadata CreateMetadata(
			IEnumerable<Attribute> attributes, 
			Type containerType,
			Func<object> modelAccessor, 
			Type modelType, 
			string propertyName
		)
		{
			IEnumerable<Attribute> enumerable = attributes as Attribute[] ?? attributes.ToArray();
			var metadata = base.CreateMetadata(enumerable, containerType, modelAccessor, modelType, propertyName);

			var displayAttribute = enumerable.OfType<DisplayAttribute>().FirstOrDefault();
			if (displayAttribute != null)
			{
				metadata.Description = displayAttribute.Description;
				metadata.DisplayName = displayAttribute.Name;
			}
			return metadata;
		}
	}
}
