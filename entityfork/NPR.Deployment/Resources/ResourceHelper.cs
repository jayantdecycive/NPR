using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace PerformanceInsights.Deployment.Resources
{
	public class ResourceHelper
	{
		private static string _SqlScriptTracker = null;
		public static string SqlScriptTracker
		{
			get
			{
				if (ResourceHelper._SqlScriptTracker == null) ResourceHelper._SqlScriptTracker = new ResourceHelper().GetResourceTextfile("SqlScriptTracker.sql");

				return ResourceHelper._SqlScriptTracker;
			}
		}



		private Stream GetResourceStream(string FileName)
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("PerformanceInsights.Deployment.Resources.{0}", FileName));
		}
		private string GetResourceTextfile(string FileName)
		{
			string contents = "";
			using (StreamReader reader = new StreamReader(this.GetResourceStream(FileName)))
			{
				contents = reader.ReadToEnd();
			}

			return contents;
		}
	}
}
