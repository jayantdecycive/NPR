using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.site.controllers._base;
using cfacore.shared.domain.common;
using cfares.domain.user;
using cfacore.domain.user;
using cfacore.shared.domain._base;

namespace cfares.service.common
{
   public class TokenService// : DomainService<Token>
    {
        /*
        //TODO: connector
        //cfacore.entity.dao._event.ResApplicationEntities db = null;
        protected double DEFAULT_TOKEN_LIFE=5.0;

        public TokenService(string connectionString)
        {
            //db = new cfacore.entity.dao._event.ResApplicationEntities(connectionString);
        }
       
        public TokenService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString;
            //db = new cfacore.entity.dao._event.ResApplicationEntities();
        }

         public override bool Save(Token obj)
         {
             //cfacore.entity.dao._event.Token tk = toEntityToken(obj);
             this.OnBeforeSave(new DomainServiceEventArgs { target = obj });
             db.Tokens.AddObject(tk);
            
             if(obj.IsBound()){
                 db.ObjectStateManager.ChangeObjectState(tk, System.Data.EntityState.Modified);            
             }
             bool success = db.SaveChanges() > 0;
             obj.Id(tk.TokenId.ToString());


             this.OnSave(new DomainServiceEventArgs { target = obj, success = success });            
             return success;
         }

         public Token GenerateToken() {
             Token token = new Token();
             token.TokenUID = Guid.NewGuid();
             token.Expiration = DateTime.Now.AddDays(DEFAULT_TOKEN_LIFE);
            
             return token;
         }

         public Token GenerateToken(ResUser owner)
         {
             Token token = GenerateToken();
             token.User = owner;
             return token;
         }

         public Token GeneratePasswordToken(ResUser owner) {
             Token tk = GenerateToken(owner);
             tk.Action = "PasswordReset";
             return tk;
         }

        private cfacore.entity.dao._event.Token toEntityToken(Token obj)
        {
            cfacore.entity.dao._event.Token tk = new cfacore.entity.dao._event.Token();
            tk.Action = obj.Action;
            tk.Data = obj.Data;
            tk.Expiration = obj.Expiration;
            if(!string.IsNullOrEmpty(obj.Id()))
             tk.TokenId = long.Parse(obj.Id());
           
            tk.UID = obj.TokenUID.ToString();

            if (obj.User != null && obj.User.UserId!=0)
                 tk.UserId = (long)(obj.User.UserId);           

            return tk;
        }

        private Token toDomainObject(cfacore.entity.dao._event.Token obj)
        {
            Token tk = new Token();
            tk.Action = obj.Action;
            tk.Data = obj.Data;
            tk.Expiration = obj.Expiration;           
            tk.Id(obj.TokenId.ToString());

            tk.TokenUID = Guid.Parse(obj.UID);

            if (obj.UserId != null)
                tk.User = new User(obj.UserId.ToString());

            return tk;
        }

        private IList<Token> toDomainObjects(IQueryable<cfacore.entity.dao._event.Token> iQueryable)
        {
            List<Token> tokens = new List<Token>();
            foreach (cfacore.entity.dao._event.Token tk in iQueryable) {
                tokens.Add(toDomainObject(tk));
            }
            return tokens;
        }


        public override Token Load(string ID)
        {
            if (string.IsNullOrEmpty(ID))
                return null;
            long lID = long.Parse(ID);
            Token obj = toDomainObject(db.Tokens.FirstOrDefault(m => m.TokenId == lID));
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

       

        public Token LoadByUID(Guid ID)
        {
            string guidStr = ID.ToString();

            Token obj = toDomainObject(db.Tokens.FirstOrDefault(m => m.UID == guidStr));
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public Token LoadByUID(string ID)
        {
            return LoadByUID(Guid.Parse(ID));
        }

        public IList<Token> LoadByUserId(string id)
        {
            long lId = long.Parse(id);

            IList<Token> obj = toDomainObjects(db.Tokens.Where(m => m.UserId == lId));
            this.OnLoadCollection(obj);
            return obj;
           
        }

        public IList<Token> LoadAllExpired()
        {           
            IList < Token > obj = toDomainObjects(db.Tokens.Where(m => m.Expiration < DateTime.Today));
            this.OnLoadCollection(obj);
            return obj;
        }


        public override bool Delete(Token obj)
        {
            long lId = long.Parse(obj.Id());
            cfacore.entity.dao._event.Token tk = db.Tokens.FirstOrDefault(m => m.TokenId == lId);
            db.DeleteObject(tk);
            bool success = db.SaveChanges() > 0;
            this.OnDelete(new DomainServiceEventArgs { target = obj, success = success });
            return success;
        }

        public override Token Load(Uri uri)
        {
            return Load(uri.LocalPath);
        }

        public override List<Token> GetAll()
        {

            throw new NotImplementedException();
        }

        public override Token[] Search(KeyValuePair<string, string>[] criteria)
        {

            throw new NotImplementedException();
        }
         * */
    }
        
}
