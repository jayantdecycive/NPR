using System;
using System.Linq;
using cfacore.shared.domain.common;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;

namespace cfares.repository.contact
{
    public class CommunicationRepo : GenericRepository<IResContext, Communication, int,Communication>
    {
        public override Communication FindBySlug(string slug)
        {
            return Find(x => x.EmailUriString == slug);
        }

        public bool MailAllowed(string email, Uri uri)
        {
	        string uriAsString = uri.ToString();
            return ! GetAll().Any( x => x.EmailUriString == uriAsString && x.Email == email );
        }

        public bool MailSent(string email, Uri uri)
        {
            Communication c = new Communication { Email = email, EmailUri = uri, CreationDate = DateTime.Now };
			Add( c );
	        Commit();
	        return true;
	        //return Save(c);
        }

        public CommunicationRepo(IResContext context) : base(context)
        {
        }
    }
}
