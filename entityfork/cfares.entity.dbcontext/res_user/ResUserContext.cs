using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.entity.dbcontext.res_event;
using cfares.domain.user;
using System.Data;
using System.Data.Entity.Validation;
using cfacore.shared.domain.user;

namespace cfares.entity.dbcontext.res_user
{
    public class ResUserContext
    {
        public ResUserContext(){
            dbcontext = new CfaResContext();
        }
        public CfaResContext dbcontext;
        public int Save(ResUser User,bool isNew) {

            if (isNew)
            {
                
                dbcontext.ResUsers.Add(User);                
            }
            else {
                dbcontext.Entry(User).State = EntityState.Modified;                
            }
            int results = 0;
            try
            {
                results = dbcontext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            { 
                foreach(var eve in ex.EntityValidationErrors){
                    System.Diagnostics.Debug.Write(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.Write(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
                throw new Exception("Validation Error");
            }
            return results;

        }

       

        public ResUser Load(int id)
        {
            return dbcontext.ResUsers.Find(id);
        }

        public ResUser Load(string id)
        {
            return dbcontext.ResUsers.Find(int.Parse(id));
        }

        public ResUser LoadByUsername(string username)
        {
            return dbcontext.ResUsers.SingleOrDefault(x => x.Username == username);
        }

        public List<ResUser> LoadByUsernames(string[] usernames)
        {
            return dbcontext.ResUsers.Where(x => usernames.Contains(x.Username)).ToList();
        }

        public ResUser LoadByUsernameWithAddress(string username)
        {
            return dbcontext.ResUsers.Include("Addresses").SingleOrDefault(x => x.Username == username);
        }

        public ResUser LoadByDatasource(string authority, string ID)
        {

            ResUser obj = dbcontext.ResUsers.SingleOrDefault(user => user.Authority == authority && user.AuthorityUID == ID);
            
            //this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public int Delete(int id)
        {
            return Delete(dbcontext.ResUsers.Find(id));
        }

        public int Delete(string username)
        {
            return Delete(dbcontext.ResUsers.Single(user => user.Username == username));
        }

        public int Delete(ResUser user)
        {

            dbcontext.ResUsers.Remove(user);
            return dbcontext.SaveChanges();
        }

         

       
    }

}
