using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using cfacore.domain.user;

namespace cfacore.shared.domain.common
{
    public class Token : Token<User>
    {
        public Token(string argId):base(argId)
        {
            
        }

        public Token():base()
        {


        }
    }

    public class Token<TOwner>: DomainObject, IToken<TOwner>
        where TOwner:IUser
    {
        public Token(string argId)
        {
            this._Id = argId;

        }

        public Token()
        {


        }

        

        public override string UriBase()
        {
            return "http://data.chick-fil-a.com/common/token/";
        }

        
        public Guid TokenUID
        {
            get;
            set;
        }

        public int? UserId { get; set; }

        public TOwner User
        {
            get;
            set;
        }

        
        public string Action
        {
            get;
            set;
        }

        
        public DateTime Expiration
        {
            get;
            set;
        }

        public bool IsExpired() {
            return Expiration <= DateTime.Now;
        }

        public bool IsOwner(User user)
        {
            return User.UserId == user.UserId;
        }

        public bool IsOwner(string userId)
        {
            return User.Id() == userId;
        }

        public bool IsOwner(int userId)
        {
            return User.UserId == userId;
        }

        public override string ToChecksum()
        {
            return TokenUID.ToString();
        }

        public string ToUrl() {
            
            if(!string.IsNullOrEmpty(this.Data))
                return string.Format("/Token/{0}/{1}?t={2}",Action,Data,TokenUID.ToString())+(User!=null&&User.IsBound()?"&h="+User.HexedId():"");
            return string.Format("/Token/{0}/{1}", Action, TokenUID.ToString()) + (User != null && User.IsBound() ? "?h=" + User.HexedId() : "");
        }

        public string ToAbsoluteUrl(string domainAndProtocal)
        {
            return domainAndProtocal+ToUrl();
        }

        public string ToAbsoluteUrl(bool secure)
        {
            return ToAbsoluteUrl(string.Format("http{0}://tour.chick-fil-a.com",secure?"s":""));
        }

        public string Data { get; set; }
    }
}
