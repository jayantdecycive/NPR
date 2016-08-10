using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace MvcDomainRouting.Code
{
    public class DomainRoute : Route,IRouteWithArea
    {
        private Regex domainRegex;
        private Regex pathRegex;
        private string _Area;
        private string[] namespaces;
        public string Name { get; set; }

        public string Domain { get; set; }

        public DomainRoute(string domain, string url, RouteValueDictionary defaults)
            : base(url, defaults, new MvcRouteHandler())
        {
            Domain = domain;
        }


        public DomainRoute(string domain, string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
            Domain = domain;
        }

        public DomainRoute(string domain, string url, object defaults)
            : base(url, new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            Domain = domain;
        }

        public DomainRoute(string domain, string url, object defaults, IRouteHandler routeHandler)
            : base(url, new RouteValueDictionary(defaults), routeHandler)
        {
            Domain = domain;
        }

        public DomainRoute(string domain, string area, string url, object defaults)
            : base(url, new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            this._Area = area;
            Domain = domain;
        }

        public DomainRoute(string domain, string area, string url, object defaults, string[] namespaces)
            : base(url, new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            this._Area = area;
            Domain = domain;
            this.namespaces = namespaces;
        }

		public DomainRoute(string domain, string url, string area, object defaults, object constraints, string[] namespaces)
            : this( domain, url, defaults, constraints, namespaces )
        {
            this._Area = area;
        }

		public DomainRoute(string domain, string url, object defaults, object constraints, string[] namespaces)
            : base(url, new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            if (url == null) {
                throw new ArgumentNullException("url");
            }

            Route route = new Route(url, new MvcRouteHandler()) {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints)
            };

            if ((namespaces != null) && (namespaces.Length > 0)) {
                route.DataTokens = new RouteValueDictionary();
                route.DataTokens["Namespaces"] = namespaces;
            }

            Domain = domain;
        }

        public DomainRoute(string domain, string area, string url, object defaults, IRouteHandler routeHandler)
            : base(url, new RouteValueDictionary(defaults), routeHandler)
        {
            this._Area = area;
            Domain = domain;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // Build regex
            domainRegex = CreateRegex(Domain);
            pathRegex = CreateRegex(Url);

            // Request information
            string requestDomain = httpContext.Request.Headers["host"];
            if (!string.IsNullOrEmpty(requestDomain))
            {
                if (requestDomain.IndexOf(":") > 0)
                {
                    requestDomain = requestDomain.Substring(0, requestDomain.IndexOf(":"));
                }
            }
            else
            {
                requestDomain = httpContext.Request.Url.Host;
            }
            string requestPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;

            // Match domain and route
            Match domainMatch = domainRegex.Match(requestDomain);
            Match pathMatch = pathRegex.Match(requestPath);

            // Route data
            RouteData data = null;
            if (domainMatch.Success && pathMatch.Success)
            {
                data = new RouteData(this, RouteHandler);

                // Add defaults first
                if (Defaults != null)
                {
                    foreach (KeyValuePair<string, object> item in Defaults)
                    {
                        data.Values[item.Key] = item.Value;
                    }
                }

                // Iterate matching domain groups
                for (int i = 1; i < domainMatch.Groups.Count; i++)
                {
                    Group group = domainMatch.Groups[i];
                    if (group.Success)
                    {
                        string key = domainRegex.GroupNameFromNumber(i);

                        if (!string.IsNullOrEmpty(key) && !char.IsNumber(key, 0))
                        {
                            if (!string.IsNullOrEmpty(group.Value))
                            {
                                data.Values[key] = group.Value;
                            }
                        }
                    }
                }

                // Iterate matching path groups
                for (int i = 1; i < pathMatch.Groups.Count; i++)
                {
                    Group group = pathMatch.Groups[i];
                    if (group.Success)
                    {
                        string key = pathRegex.GroupNameFromNumber(i);

                        if (!string.IsNullOrEmpty(key) && !char.IsNumber(key, 0))
                        {
                            if (!string.IsNullOrEmpty(group.Value))
                            {
                                data.Values[key] = group.Value;
                            }
                        }
                    }
                }
            }

			if( DataTokens != null )
				foreach (var item in DataTokens)
					 data.DataTokens.Add(item.Key, item.Value);

			if( data != null && data.DataTokens != null && namespaces != null )
				data.DataTokens.Add("namespaces", namespaces);

            return data;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return base.GetVirtualPath(requestContext, RemoveDomainTokens(values));
        }

        public DomainData GetDomainData(RequestContext requestContext, RouteValueDictionary values)
        {
            // Build hostname
            string hostname = Domain;
            foreach (KeyValuePair<string, object> pair in values)
            {
                hostname = hostname.Replace("{" + pair.Key + "}", pair.Value.ToString());
            }

            // Return domain data
            return new DomainData
            {
                Protocol = "http",
                HostName = hostname,
                Fragment = ""
            };
        }

        private Regex CreateRegex(string source)
        {
            // Perform replacements
            source = source.Replace("/", @"\/?");
            source = source.Replace(".", @"\.?");
            source = source.Replace("-", @"\-?");
            source = source.Replace("{", @"(?<");
            source = source.Replace("}", @">([a-zA-Z0-9_]*))");

            return new Regex("^" + source + "$");
        }

        private RouteValueDictionary RemoveDomainTokens(RouteValueDictionary values)
        {
            Regex tokenRegex = new Regex(@"({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?");
            Match tokenMatch = tokenRegex.Match(Domain);
            for (int i = 0; i < tokenMatch.Groups.Count; i++)
            {
                Group group = tokenMatch.Groups[i];
                if (group.Success)
                {
                    string key = group.Value.Replace("{", "").Replace("}", "");
                    if (values.ContainsKey(key))
                        values.Remove(key);
                }
            }

            return values;
        }

        public string Area
        {
            get { return this._Area; }
        }
    }

    public class SubdomainRoute : Route, IRouteWithArea
    {
        private string[] namespaces;
        public string Subdomain { get; private set; }

        public SubdomainRoute(string subdomain, string url, object defaults, string[] namespaces)
            : this(subdomain, url, defaults, new { }, namespaces)
        {
            this.Subdomain = subdomain;
            this.namespaces = namespaces;
        }

        public SubdomainRoute(string subdomain, string url, object defaults, object constraints, string[] namespaces)
            : base(url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new MvcRouteHandler())
        {
            this.Subdomain = subdomain;
            this.namespaces = namespaces;
        }

        public string Area
        {
            get { return Convert.ToString(this.Defaults["area"]); }
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            string requestPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;
            string requestDomain = GetSubdomain(httpContext.Request.Headers["Host"]);

            RouteData data = null;
            Regex domainRegex = CreateRegex(this.Subdomain);
            Regex pathRegex = CreateRegex(this.Url);
            Match domainMatch = domainRegex.Match(requestDomain);
            Match pathMatch = pathRegex.Match(requestPath);

            if (domainMatch.Success && pathMatch.Success)
            {
                data = base.GetRouteData(httpContext);
                if (data == null)
                    return null;

                data.DataTokens.Add("Area", data.Values["area"]);
                data.DataTokens.Add("namespaces", this.namespaces);
            }

            return data;
        }

        public static string GetSubdomain(string host)
        {
            if (host.IndexOf(":") >= 0)
                host = host.Substring(0, host.IndexOf(":"));

            Regex tldRegex = new Regex(@"\.[a-z]{2,3}\.[a-z]{2}$");
            host = tldRegex.Replace(host, "");
            tldRegex = new Regex(@"\.[a-z]{2,4}$");
            host = tldRegex.Replace(host, "");

            if (host.Split('.').Length > 1)
                return host.Substring(0, host.LastIndexOf("."));
            else
                return string.Empty;
        }

        private Regex CreateRegex(string source)
        {
            source = source.Replace("/", @"\/?");
            source = source.Replace(".", @"\.?");
            source = source.Replace("-", @"\-?");
            source = source.Replace("{", @"(?<");
            source = source.Replace("}", @">([a-zA-Z0-9_-]*))");
            return new Regex("^" + source + "$");
        }
    }
}
