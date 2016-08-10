using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.shared.domain.user;
using cfacore.domain._base;
using core.synchronization.Automation;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;

namespace cfacore.domain.user
{
    [ITable]

    public interface IUser : IDomainObject
    {
       
        string Email
        {
            get;
            set;
        }

        string Username
        {
            get;
            set;
        }

        string UID
        {
            get;
            set;
        }

        string DN
        {
            get;
            set;
        }
        
        Name Name
        {
            get;
            set;
        }

        Address Address
        {
            get;
            set;
        }

        Phone HomePhone
        {
            get;
            set;
        }

        Phone MobilePhone
        {
            get;
            set;
        }

        DateTime CreatedDate
        {
            get;
            set;
        }

        DateTime LastActivity
        {
            get;
            set;
        }
        
        UserAccountStatus AccountStatus { 
            get; 
            set; 
        }

        string Authority
        {
            get;
            set;
        }

       
        string AuthorityUID
        {
            get;
            set;
        }

        int UserId { 
            get; 
            set; 
        }

        Gender Gender
        {
            get;
            set;
        }


        string HexedEmail();
        void HexedEmail(string Email);

        string HexedId();
        void HexedId(string Id);
    }

    public enum UserAccountStatus {
        Normal,
        Banned,
        UnderReview,
        Untrusted
    }

    public enum Gender
    {
        Unknown, 
        Male, 
        Female
    }

    
}