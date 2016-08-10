using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using MySql.Data.MySqlClient;
using cfacore.shared.domain.store;
using cfacore.shared.domain.user;
using cfares.domain._event;
using cfares.domain.store;

namespace cfacore.mysql.dao.shared
{
    public class StoreMySqlAccess : MySqlAccessObject<ResStore>
    {
        public StoreMySqlAccess(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public override ResStore BindFromReader(MySqlDataReader reader, string qualifier)
        {
            ResStore location = new ResStore();
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            location.LocationNumber = Convert.ToString(reader["LocationNumber"]);
            location.BusinessConsultant = new User(Convert.ToString(reader["BusinessConsultantId"]));
            location.ConceptCode = (cfacore.domain.store.ConceptCode)Convert.ToInt32(reader["ConceptCode"]);
            location.CorporateOwned = Convert.ToBoolean(reader["CorporateOwned"]);
            location.BillingAddress = new Address(Convert.ToInt32(reader["BillingAddressId"]));
            location.ShippingAddress = new Address(Convert.ToInt32(reader["ShippingAddressId"]));
            location.StreetAddress = new Address(Convert.ToInt32(reader["StreetAddressId"]));
            location.Phone = new Phone(Convert.ToString(reader["Phone"]));
            location.Fax = new Phone(Convert.ToString(reader["Fax"]));
            location.VoiceMail.Extension = (Convert.ToString(reader["VoiceMail"]));
            location.Email = Convert.ToString(reader["Email"]);
            location.Distributor = new Distributor(Convert.ToString(reader["DistributorId"]));
            
                location.Features.AcceptsCfaCard = Convert.ToBoolean(reader["AcceptsCfaCard"]);
                location.Features.HasDiningRoom = Convert.ToBoolean(reader["HasDiningRoom"]);
                location.Features.HasDriveThru = Convert.ToBoolean(reader["HasDriveThru"]);
                location.Features.OffersOnlineOrdering = Convert.ToBoolean(reader["OffersOnlineOrdering"]);
                location.Features.OffersWireless = Convert.ToBoolean(reader["OffersWireless"]);
                location.Features.Playground = Convert.ToString(reader["Playground"]);
                location.Features.ServesBreakfast = Convert.ToBoolean(reader["ServesBreakfast"]);
            
            location.FinancialConsultant = new User(Convert.ToString(reader["FinancialConsultantId"]));
            location.GMTOffset = Convert.ToString(reader["GMTOffset"]);

                location.Coordinates.Latitude = Convert.ToDouble(reader["Lat"]);
                location.Coordinates.Longitude = Convert.ToDouble(reader["Lon"]);

            location.LocationCode = (cfacore.domain.store.LocationCode)Convert.ToInt32(reader["LocationCode"]);
            location.MarketableName = Convert.ToString(reader["MarketableName"]);
            location.MarketableURL = new Uri(Convert.ToString(reader["MarketableURL"]));
            location.Message = Convert.ToString(reader["Message"]);
            location.LocationDescription = Convert.ToString(reader["LocationDescription"]);
            location.Name = Convert.ToString(reader["Name"]);
            location.OpenDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["OpenDate"]),est);
            location.Operator = new User(Convert.ToString(reader["OperatorId"]));
            location.OperatorTeamName = Convert.ToString(reader["OperatorTeamName"]);
            location.PriceGroupNumber = Convert.ToString(reader["PriceGroupNumber"]);
            location.ProjectedOpenDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["ProjectedOpenDate"]),est);
            location.RegionName = Convert.ToString(reader["RegionName"]);
            location.ServiceTeamName = Convert.ToString(reader["ServiceTeamName"]);
            location.Status = (cfacore.domain.store.StoreStatus)Convert.ToInt32(reader["Status"]);
            location.SiteStatus = Convert.ToString(reader["SiteStatus"]);
            location.UnitMarketingDirector = new User(Convert.ToString(reader["MarketingDirectorId"]));
            
            return location;

        }


        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [identity].</param>
        /// <returns></returns>
        public override MySqlParameter[] BindToSqlVariableArray(ResStore obj, bool identity)
        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();
                        

            parameters.Add(new MySqlParameter("_LocationNumber",obj.LocationNumber));
            parameters.Add(new MySqlParameter("_BusinessConsultantId",obj.BusinessConsultant.Id()));
            parameters.Add(new MySqlParameter("_ConceptCode",obj.ConceptCode));
            parameters.Add(new MySqlParameter("_CorporateOwned",obj.CorporateOwned));
            parameters.Add(new MySqlParameter("_BillingAddressId",obj.BillingAddress.Id()));
            parameters.Add(new MySqlParameter("_ShippingAddressId",obj.ShippingAddress.Id()));
            parameters.Add(new MySqlParameter("_StreetAddressId",obj.StreetAddress.Id()));
            parameters.Add(new MySqlParameter("_Phone",obj.Phone));
            parameters.Add(new MySqlParameter("_Fax",obj.Fax));
            parameters.Add(new MySqlParameter("_VoiceMail",obj.VoiceMail.Extension));
            parameters.Add(new MySqlParameter("_Email",obj.Email));
            parameters.Add(new MySqlParameter("_Distributor",obj.Distributor));
            parameters.Add(new MySqlParameter("_AcceptsCfaCard",obj.AcceptsCfaCard));
            parameters.Add(new MySqlParameter("_HasDiningRoom",obj.HasDiningRoom));
            parameters.Add(new MySqlParameter("_HasDriveThru",obj.HasDriveThru));
            parameters.Add(new MySqlParameter("_OffersOnlineOrdering",obj.OffersOnlineOrdering));
            parameters.Add(new MySqlParameter("_OffersWireless",obj.OffersWireless));
            parameters.Add(new MySqlParameter("_Playground",obj.Playground));
            parameters.Add(new MySqlParameter("_ServesBreakfast",obj.ServesBreakfast));
            parameters.Add(new MySqlParameter("_FinancialConsultantId",obj.FinancialConsultant.Id()));
            parameters.Add(new MySqlParameter("_GMTOffset",obj.GMTOffset));
            parameters.Add(new MySqlParameter("_Lat",obj.Lat));
            parameters.Add(new MySqlParameter("_Lon",obj.Lon));
            parameters.Add(new MySqlParameter("_LocationCode",obj.LocationCode));
            parameters.Add(new MySqlParameter("_MarketableName",obj.MarketableName));
            parameters.Add(new MySqlParameter("_MarketableURL",obj.MarketableURL));
            parameters.Add(new MySqlParameter("_Message",obj.Message));
            parameters.Add(new MySqlParameter("_LocationDescription",obj.LocationDescription));
            parameters.Add(new MySqlParameter("_Name",obj.Name));
            parameters.Add(new MySqlParameter("_OpenDate",obj.OpenDate));
            parameters.Add(new MySqlParameter("_OperatorId",obj.Operator.Id()));
            parameters.Add(new MySqlParameter("_OperatorTeamName",obj.OperatorTeamName));   
            parameters.Add(new MySqlParameter("_PriceGroupNumber",obj.PriceGroupNumber));
            parameters.Add(new MySqlParameter("_ProjectedOpenDate",obj.ProjectedOpenDate));
            parameters.Add(new MySqlParameter("_RegionName",obj.RegionName));
            parameters.Add(new MySqlParameter("_ServiceTeamName",obj.ServiceTeamName));
            parameters.Add(new MySqlParameter("_Status",obj.Status));
            parameters.Add(new MySqlParameter("_SiteStatus", obj.SiteStatus));    
            parameters.Add(new MySqlParameter("_MarketingDirectorId",obj.UnitMarketingDirector.Id()));

            return (parameters.ToArray());

        }

        public List<ResStore> LoadByResEvent(string ID)
        {
            return BindListFromStoredProcedure("Location_GetByResId", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public override ResStore Load(string ID)
        {
            return BindFromStoredProcedure("Location_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public ResStore LoadBySlot(string ID)
        {
            return BindFromStoredProcedure("Location_GetBySlotId", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public ResStore LoadBySlot(Slot slot)
        {
            return BindFromStoredProcedure("Location_GetBySlotId", new MySqlParameter[] { 
                new MySqlParameter("_Id",slot.Id())
            });
        }

        public ResStore LoadByMarketableName(string ID)
        {
            return BindFromStoredProcedure("Location_GetByMarketableName", new MySqlParameter[] { 
                new MySqlParameter("_MarketableName",ID)
            });
        }

        public override bool Save(ResStore obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("Location_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("Location_Insert", obj, false);
                if (id > 0)
                    obj.Id(id.ToString());
                return id > 0;
            }
        }

        public override bool Delete(ResStore obj)
        {
            return AssertToStoredProcedure("Location_Delete", obj, true) > 0;
        }
    }
}
