using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using MySql.Data.MySqlClient;
using cfares.domain._event;
using cfacore.shared.domain._base;
using cfares.domain._event.resevent;

namespace cfacore.mysql.dao._event
{
    public class ResEventMySqlAccess : MySqlAccessObject<ResEvent>
    {
        public ResEventMySqlAccess(string connectionString)
            : base(connectionString)
        {
        }

        #region fromreader

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public override ResEvent BindFromReader(MySqlDataReader reader, string qualifier)
        {

            ResEvent resevent = new ResEvent();
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");


            resevent.Id( Convert.ToString(reader["ResEventId"]));

            resevent.SetRegistrationAvailability( new DateRange(
                TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["RegistrationStart"]),est), 
                TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["RegistrationEnd"]),est)
                ));
            resevent.SetSiteAvailability(new DateRange(
                TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["SiteStart"]),est), 
                TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["SiteEnd"]),est)
                ));
            resevent.Name = Convert.ToString(reader["Name"]);
            resevent.Urls = Convert.ToString(reader["Url"]);
            resevent.Description = Convert.ToString(reader["Description"]);
            resevent.Status = (ResEventStatus)Convert.ToInt32(reader["Status"]);
            resevent.ReservationType = new ReservationType(Convert.ToString(reader["ReservationType"]));
            resevent.Template = new ResTemplate(Convert.ToString(reader["Template"]));
            
            //resevent.ProtoOccurrence = new Occurrence(Convert.ToString(reader["ProtoOccurrenceId"]));

            return resevent;

        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public ResEvent BindFromReaderShort(MySqlDataReader reader, string qualifier)
        {

            ResEvent resevent = new ResEvent();



            resevent.Id ( Convert.ToString(reader["ResEventId"]));

            resevent.Urls = Convert.ToString(reader["ResEventUrl"]);
            
            resevent.Status = (ResEventStatus)Convert.ToInt32(reader["ReservationStatus"]);
            resevent.ReservationType = new ReservationType(Convert.ToString(reader["ReservationType"]));
            
            return resevent;

        }

        #endregion fromreader

        #region to_parameter_set

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [[identity]].</param>
        /// <returns></returns>
        public override MySqlParameter[] BindToSqlVariableArray(ResEvent obj, bool identity)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            if(identity)
                parameters.Add(new MySqlParameter("_ResEventId",obj.Id()));
            parameters.Add(new MySqlParameter("_RegistrationStart", obj.RegistrationStart.ToUniversalTime()));
            parameters.Add(new MySqlParameter("_RegistrationEnd", obj.RegistrationEnd.ToUniversalTime()));
            parameters.Add(new MySqlParameter("_SiteStart", obj.SiteStart.ToUniversalTime()));
            parameters.Add(new MySqlParameter("_SiteEnd", obj.SiteEnd.ToUniversalTime()));
            parameters.Add(new MySqlParameter("_Name",obj.Name));
            parameters.Add(new MySqlParameter("_Url",obj.Urls));
            parameters.Add(new MySqlParameter("_Description",obj.Description));
            parameters.Add(new MySqlParameter("_Status",obj.Status));
            parameters.Add(new MySqlParameter("_ReservationType",obj.ReservationType.Id()));
            parameters.Add(new MySqlParameter("_Template", obj.Template.Id()));
            
            parameters.Add(new MySqlParameter("_ProtoOccurrenceId", obj.ProtoOccurrence.Id()));

            return (parameters.ToArray());
        }

        #endregion to_parameter_set

        #region accessor_and_assertion

        public override ResEvent Load(string ID)
        {
            return BindFromStoredProcedure("ResEvent_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public override bool Save(ResEvent obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("ResEvent_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("ResEvent_Insert", obj, false);
                if (id > 0)
                    obj.Id(id.ToString());
                return id > 0;
            }
        }

        public override bool Delete(ResEvent obj)
        {
            return AssertToStoredProcedure("ResEvent_Delete", new MySqlParameter[] { 
                    new MySqlParameter("_Id",obj.ResEventId)
                    
                }) > 0;
        }

        public ResEvent LoadByUrlName(string name)
        {
            return BindFromStoredProcedure("ResEvent_GetByUrlName", new MySqlParameter[] { 
                new MySqlParameter("_UrlName",name)
            });
        }

        public string GetLatestYearMonthForEventType(string id) {
            return ScalarFromStoredProcedure<string>("ResEvent_GetLatestYearMonthForEventType", new MySqlParameter[] { 
                new MySqlParameter("_EventType",id)
            }, new ScalarBinder<string>(ScalarStringFromReader));
        }

        #endregion accessor_and_assertion
    }
}
