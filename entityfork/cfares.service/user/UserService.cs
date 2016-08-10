using System;
using System.Collections.Generic;
using cfacore.site.controllers._base;
using cfares.domain.user;
using cfacore.mysql.dao.res;
using cfacore.mysql.dao._event;
using cfacore.service;
using cfacore.shared.domain._base;
using cfacore.dao._base;

namespace cfacore.site.controllers.shared
{
    public class UserService : DomainCacheService<ResUser>
    {
        public ResUserMySqlAccess SqlDao = null;
        public ICacheAccess<ResUser> AfDao = null;

        public UserService(string connectionString, string appFabric):base()
        {
            SqlDao = new ResUserMySqlAccess(connectionString);
            AfDao = BestAvailableCache(appFabric);
        }

        public UserService():base()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString;
            SqlDao = new ResUserMySqlAccess(connectionString);
            connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            AfDao = BestAvailableCache(connectionString);
        }

        public override bool Save(ResUser obj)
        {
            bool success;
            bool isNew = !obj.IsBound();
            this.OnBeforeSave(new DomainServiceEventArgs { target = obj, isNew = isNew });
            success = SqlDao.Save(obj);
            AddressService addressManager = new AddressService(SqlDao.ConnectionString);
            if (!success || !obj.Address.IsBound())
            {
                if (obj.Address != null && obj.Address.HasContent() && success)
                {
                    if (obj.Address.Name == null || string.IsNullOrWhiteSpace(obj.Address.Name.ToString())) {
                        obj.Address.Name = obj.Name;
                    }

                    addressManager.Save(obj.Address);

                    this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew, success = success });

                    return SqlDao.Save(obj);
                }
                this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew, success = success });
                return success;
            }

            this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew,success=success });
            return addressManager.Save(obj.Address);
            

        }

        public override ResUser Load(string ID)
        {
            ResUser obj = SqlDao.Load(ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        

        public ResUser Load(ResUser User)
        {
            return Load(User.Id());
        }

        public ResUser LoadWithAddress(string ID)
        {            
            ResUser obj = SqlDao.LoadWithAddress(ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public ResUser LoadByUsernameWithAddress(string ID)
        {
            ResUser obj = SqlDao.LoadByUsernameWithAddress(ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public ResUser LoadWithTickets(string ID)
        {
            ResUser user = Load(ID);
            LoadTickets(user);
            return user;
        }

        public void LoadTickets(ResUser user)
        {
            TicketMySqlAccess ticketSqlDao = new TicketMySqlAccess(SqlDao.ConnectionString);
            user.Tickets = ticketSqlDao.LoadByUser(user.Id());            
        }

        public ResUser LoadByDatasource(string datasource, string ID)
        {
            ResUser obj = SqlDao.LoadByDatasource(datasource, ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public ResUser LoadByUsername(string username)
        {
            ResUser obj = SqlDao.LoadByUsername(username);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public ResUser LoadByEmail(string email)
        {
            ResUser obj = SqlDao.LoadByEmail(email);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public ResUserCollection LoadByUsernames(string[] usernames)
        {
            return SqlDao.LoadByUsernames(usernames);
        }

        public override ResUser Load(Uri uri)
        {
            ResUser obj = SqlDao.Load(uri);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }



        public override bool Cache(ResUser obj)
        {
            bool success = AfDao.Save(obj);
            this.OnCache(new DomainServiceEventArgs { target = obj });
            return success;    
        }

        public override bool CacheAndSave(ResUser obj)
        {
            return AfDao.Save(obj) && SqlDao.Save(obj);
        }

        public override ResUser DeCacheOrLoad(Uri uri)
        {
            ResUser m = AfDao.Load(uri);
            if (m == null)
            {
                m = SqlDao.Load(uri);
                AfDao.Save(m);
            }
            return m;
        }

        public override ResUser DeCacheOrLoad(string id)
        {
            ResUser m = DeCache(id);
            if (m == null)
            {
                m = SqlDao.Load(id);
                AfDao.Save(m);
            }
            return m;
        }

        public override ResUser DeCache(Uri uri)
        {
            ResUser m = new ResUser();
            m = AfDao.Load(uri);
            this.OnDeCache(new DomainServiceEventArgs { target = m });
            return m;
        }
        public override ResUser DeCache(string Id)
        {
            ResUser m = new ResUser();
            m = AfDao.Load(new Uri(m.UriBase() + Id));
            this.OnDeCache(new DomainServiceEventArgs { target = m });
            return m;
        }

        public override bool Delete(ResUser obj)
        {
            bool success = SqlDao.Delete(obj);
            this.OnDelete(new DomainServiceEventArgs { target = obj, success = success });
            return success;
        }

        public bool Delete(string Username)
        {
            ResUser obj = LoadByUsername(Username);
            bool success = SqlDao.DeleteByUsername(Username);
            this.OnDelete(new DomainServiceEventArgs { target = obj, success = success });
            return success;
        }

        public override List<ResUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public override ResUser[] Search(KeyValuePair<string, string>[] criteria)
        {

            throw new NotImplementedException();
        }

        public override bool Forget(ResUser obj)
        {
            bool s = AfDao.Delete(obj);
            this.OnForget(new DomainServiceEventArgs { target = obj });
            return s;
        }
    }
}
