using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;

namespace cfacore.shared.modules.com.request
{
    [Serializable]
    [DataContract]
    public class BrowserInfo
    {
        [DataMember]
        public string browserClass = String.Empty;
        [DataMember]
        public string agentPlatform = String.Empty;
        [DataMember]
        public string agentKernal = String.Empty;
        [DataMember]
        public string agentStd = String.Empty;
        [DataMember]
        public string agentVersion = String.Empty;
        [DataMember]
        public string browserStr;
        [DataMember]
        public int majorVersion;
        [DataMember]
        public string clientOS;
        [DataMember]
        public bool legacy;
        [DataMember]
        public bool mobile = false;
        [DataMember]
        public bool tablet = false;
        [DataMember]
        public bool phone = false;
        [DataMember]
        public string agentStr;
        [DataMember]
        public bool print = true;
        [DataMember]
        public string flag = String.Empty;
        [DataMember]
        public string id = String.Empty;
        [DataMember]
        public string version = String.Empty;
        public List<string> browserCSS = new List<String>();
        
        public static BrowserInfo Read(HttpRequestBase httpRequest)
        {
            BrowserInfo browserInfo = new BrowserInfo();

            browserInfo.browserStr = httpRequest.Browser.Browser;
            browserInfo.majorVersion = httpRequest.Browser.MajorVersion;
            browserInfo.clientOS = httpRequest.Browser.Platform;
            browserInfo.mobile = httpRequest.Browser.IsMobileDevice;
            
            





            if (String.IsNullOrEmpty(httpRequest.QueryString["useragent"]))
                browserInfo.agentStr = httpRequest.UserAgent;
            else
                browserInfo.agentStr = httpRequest.QueryString["useragent"];


            if (!String.IsNullOrEmpty(httpRequest.QueryString["useragent"]) || browserInfo.clientOS == "Unknown" || httpRequest.UserAgent.IndexOf(browserInfo.browserStr) == -1)
            {

                browserInfo.browserClass = String.Empty;
                browserInfo.agentPlatform = String.Empty;
                browserInfo.agentKernal = String.Empty;
                browserInfo.agentStd = String.Empty;
                browserInfo.agentVersion = String.Empty;

                try
                {
                    Match agentMatch = Regex.Match(browserInfo.agentStr, @"([^\)]+)\s+\(([^\)]+)\)\s+([^\(\s]+)\s*\(?([^\)]*)\)?\s*([^\)]*)");
                    String[] a_browserClass = agentMatch.Groups[1].Value.Trim().Split(' ');
                    String[] a_agentPlatform = agentMatch.Groups[2].Value.Trim().Split(' ');
                    String[] a_agentKernal = agentMatch.Groups[3].Value.Trim().Split(' ');
                    String[] a_agentStd = agentMatch.Groups[4].Value.Trim().Split(' ');
                    String[] a_agentVersion;


                    if (agentMatch.Groups.Count >= 5 && !String.IsNullOrEmpty(agentMatch.Groups[5].Value.Trim()))
                        a_agentVersion = agentMatch.Groups[5].Value.Trim().Split(' ');
                    else
                        a_agentVersion = agentMatch.Groups[4].Value.Trim().Split(' ');

                    //account for blackberry!
                    if (String.IsNullOrEmpty(a_browserClass[0]) && String.IsNullOrEmpty(a_agentPlatform[0]) && browserInfo.agentStr.ToLower().Contains("berry"))
                    {
                        //BlackBerry9650/5.0.0.732 Profile/MIDP-2.1 Configuration/CLDC-1.1 VendorID/105
                        agentMatch = Regex.Match(browserInfo.agentStr, @"([^\s]+)\s+([^\s]+)\s+([^\s]+)\s+([^\s]+)");

                        a_agentPlatform[0] = "BlackBerry";
                        a_browserClass = agentMatch.Groups[2].Value.Trim().Split('/');

                        a_agentKernal = agentMatch.Groups[3].Value.Trim().Split('/');
                        a_agentStd = agentMatch.Groups[4].Value.Trim().Split('/');


                        //a_browserClass = 
                    }



                    switch (a_agentPlatform[0].ToLower().Replace(";", ""))
                    {
                        case "ipad":
                            browserInfo.clientOS = "iPad";
                            browserInfo.flag = "iOS";
                            browserInfo.tablet = true;
                            browserInfo.browserCSS.Add("iOS");
                            //browserInfo.browserCSS.Add("iPad");
                            break;
                        case "iphone":
                            browserInfo.clientOS = "iPhone";

                            browserInfo.flag = "iOS";
                            browserInfo.browserCSS.Add("iOS");
                            browserInfo.phone = true;
                            break;
                        case "blackberry":
                            browserInfo.clientOS = "BlackBerry";
                            browserInfo.browserStr = "BlackBerry";
                            browserInfo.flag = "BlackBerry";
                            
                            browserInfo.phone = true;
                            browserInfo.mobile = true;
                            if (browserInfo.majorVersion <= 5)
                                browserInfo.legacy = true;
                            break;
                        case "linux":
                            if (a_agentPlatform[2].ToLower().Contains("android"))
                            {
                                browserInfo.clientOS = "Android";
                                browserInfo.phone = browserInfo.agentStr.ToLower().Contains("mobile");
                                browserInfo.tablet = !browserInfo.phone;

                                browserInfo.flag = "Android";
                                double version = Double.Parse(Regex.Match(a_agentPlatform[2], @"([0-9\.]+);$").Groups[1].Value);
                                browserInfo.majorVersion = (int)Math.Round(version);
                                browserInfo.version = version.ToString();
                                
                            }
                            else
                            {
                                browserInfo.clientOS = "Linux";
                            }
                            break;
                        case "macintosh":
                            browserInfo.clientOS = "Macintosh";
                            break;
                        case "windows":

                            break;
                        default:
                            if (browserInfo.clientOS == "Unknown")
                                browserInfo.clientOS = httpRequest.Browser.MobileDeviceModel;
                            break;
                    }
                    string[] browserCore = a_agentVersion[0].Split('/');
                    browserInfo.browserStr = browserCore[0];
                    browserInfo.agentKernal = a_agentKernal[0].Split('/')[0];
                    int q = 0;
                    while (browserInfo.browserStr.ToLower() == "version" && q < a_agentVersion.Length)
                    {
                        browserInfo.browserStr = a_agentVersion[q].Split('/')[0];
                        q++;
                    }
                    if (browserInfo.browserStr.ToLower() == "version")
                        browserInfo.browserStr = browserInfo.agentKernal;
                    browserInfo.version = browserCore[1];
                    browserInfo.majorVersion = int.Parse(browserCore[1].Split('.')[0]);



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


            }
            else
            {
                browserInfo.phone = browserInfo.mobile;
            }

            switch (browserInfo.browserStr)
            {
                case "IE":

                    switch (browserInfo.majorVersion)
                    {
                        
                        case 7:
                            browserInfo.flag = "IE7";
                            browserInfo.browserCSS.Add("ie-Legacy");
                            break;
                        case 8:
                            browserInfo.flag = "IE8";
                            browserInfo.browserCSS.Add("ie-Legacy");
                            break;
                        case 9:
                            browserInfo.flag = "IE9";
                            break;
                        case 10:
                            browserInfo.flag = "IE10";
                            break;
                        default:
                        case 6:
                            browserInfo.flag = "IE6";
                            browserInfo.legacy = true;
                            break;
                    }
                    
                    break;
                case "Chrome":
                    browserInfo.flag = "Webkit";

                    break;
                case "Safari":
                    browserInfo.flag = "Webkit";
                    break;
                case "Firefox":
                    browserInfo.flag = "FireFox";
                    break;
                default:
                    break;
            }
            //if (!string.IsNullOrEmpty(browserInfo.flag))
              //  browserInfo.browserCSS.Add(browserInfo.flag);

            if(browserInfo.mobile)
                browserInfo.browserCSS.Add("Mobile");
            if (browserInfo.tablet)
                browserInfo.browserCSS.Add("Tablet");
            if (browserInfo.phone)
                browserInfo.browserCSS.Add("Phone");

            browserInfo.id = browserInfo.browserStr;

            return browserInfo;
        }
    }
}
