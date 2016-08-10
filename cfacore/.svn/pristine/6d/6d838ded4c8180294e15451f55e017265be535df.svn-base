using System;

namespace cfacore.domain._base
{
    public interface IDomainObject
    {
        /// <summary>
        /// Gets the URI.
        /// </summary>
        Uri Uri();
        void Uri(Uri uri);

        /// <summary>
        /// Gets the URI base.
        /// </summary>
        string UriBase();
        string UriBase(string append);

        string Id();
        void Id(string Id);

        bool Loaded();
        void Loaded(bool IsLoaded);
        
        void UnBind();
        void Bind();

        bool IsBound();
        string ToChecksum();
	    string ToHtmlString();
    }    
}
