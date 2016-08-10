using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace mvc3_to_mvc4
{
    class Program
    {
        // if not args, it looks for first .csproj
        static void Main(string[] args)
        {
            // the main web.config
            string WebConfigFileContent = System.IO.File.ReadAllText(@"web.config");
            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "System.Web.Mvc, Version=3.0.0.0", "System.Web.Mvc, Version=4.0.0.0");
            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "System.Web.WebPages, Version=1.0.0.0", "System.Web.WebPages, Version=2.0.0.0");
            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "System.Web.Helpers, Version=1.0.0.0", "System.Web.Helpers, Version=2.0.0.0");
            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "System.Web.WebPages.Razor, Version=1.0.0.0", "System.Web.WebPages.Razor, Version=2.0.0.0");
            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "<add key=\"webpages:Version\" value=\"1.0.0.0\" />", "<add key=\"webpages:Version\" value=\"2.0.0.0\" />\r\n<add key=\"PreserveLoginUrl\" value=\"true\" />");

            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "bindingRedirect oldVersion=\"1.0.0.0-2.0.0.0\" newVersion=\"3.0.0.0\"", "bindingRedirect oldVersion=\"1.0.0.0-3.0.0.0\" newVersion=\"4.0.0.0\"");

            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "</assemblyBinding>", @"
                <dependentAssembly>
                <assemblyIdentity name=""System.Web.Helpers"" publicKeyToken=""31bf3856ad364e35"" />
                <bindingRedirect oldVersion=""1.0.0.0"" newVersion=""2.0.0.0""/>
                </dependentAssembly>
                <dependentAssembly>
                <assemblyIdentity name=""System.Web.WebPages"" 
                        publicKeyToken=""31bf3856ad364e35"" />
                <bindingRedirect oldVersion=""1.0.0.0"" newVersion=""2.0.0.0""/>
                </dependentAssembly>
            </assemblyBinding>");
            WriteFile("web.config", WebConfigFileContent);

            // views/web.config
            WebConfigFileContent = System.IO.File.ReadAllText(@"views\web.config");
            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "System.Web.Mvc, Version=3.0.0.0", "System.Web.Mvc, Version=4.0.0.0");
            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "System.Web.WebPages, Version=1.0.0.0", "System.Web.WebPages, Version=2.0.0.0");
            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "System.Web.Helpers, Version=1.0.0.0", "System.Web.Helpers, Version=2.0.0.0");
            WebConfigFileContent = FindAndReplace(WebConfigFileContent, "System.Web.WebPages.Razor, Version=1.0.0.0", "System.Web.WebPages.Razor, Version=2.0.0.0");
            WriteFile(@"views\web.config", WebConfigFileContent);

            // the project
            string projectname;
            if (args.Length == 0)
            {
                string[] files = System.IO.Directory.GetFiles(".", "*.csproj");
                projectname = files[0].ToString();
            }
            else
            {
                string projectarg = args[0].ToString().ToLower();
                projectname = (projectarg.EndsWith(".csproj") ? projectarg : projectarg + ".csproj");
            }
            string projectFileContent = System.IO.File.ReadAllText(projectname);
            projectFileContent = FindAndReplace(projectFileContent, "{E53F8FEA-EAE0-44A6-8774-FFD645390401}", "{E3E379DF-F4C6-4180-9B81-6769533ABE47}");
            projectFileContent = FindAndReplace(projectFileContent, "System.Web.Mvc, Version=3.0.0.0", "System.Web.Mvc, Version=4.0.0.0");
            projectFileContent = FindAndReplace(projectFileContent, "System.Web.Razor, Version=1.0.0.0", "System.Web.Razor, Version=2.0.0.0");
            WriteFile(projectname, projectFileContent);
        }

        static void WriteFile(string filename, string content)
        {
            TextWriter tw = new StreamWriter(filename);
            tw.WriteLine(content);
            tw.Close();
        }

        static string FindAndReplace(string content, string whattolook, string replacement)
        {
            return content.Replace(whattolook, replacement);
        }
    }
}