using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using MySql.Data.MySqlClient;
using cfares.domain._event;
using cfacore.shared.domain.store;
using cfares.domain._event.occ;

using cfares.domain.store;

namespace cfacore.mysql.dao._event
{
    public class OccurrenceMySqlAccess : MySqlAccessObject<Occurrence>
    {
        public OccurrenceMySqlAccess(string connectionString)
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
        public Occurrence BindFromReaderShort(MySqlDataReader reader, string qualifier)
        {
            Occurrence occurrence = new Occurrence();

            occurrence.Id(Convert.ToString(reader["OccurrenceId"]));
            occurrence.ResEvent = new ResEvent(Convert.ToString(reader["ResEventId"]));
            
            return occurrence;

        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public override Occurrence BindFromReader(MySqlDataReader reader, string qualifier)
        {
            Occurrence occurrence = new Occurrence();
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            
            occurrence.Id (Convert.ToString(reader["OccurrenceId"]));
            occurrence.ResEvent = new ResEvent(Convert.ToString(reader["ResEventId"]));
            occurrence.Status = (OccurrenceStatus)Convert.ToInt32(reader["Status"]);
            occurrence.Store = new ResStore(Convert.ToString(reader["StoreId"]));
            occurrence.RegistrationAvailability.Start = reader["Start"] is DBNull? DateTime.Now:
                TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["Start"]),est);
            occurrence.RegistrationAvailability.End = reader["End"] is DBNull?DateTime.Now:
                TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["End"]),est);
            occurrence.BoundToPrototype = Convert.ToBoolean(reader["BoundToPrototype"]);
            occurrence.SlotRange.Start = reader["SlotRangeStart"] is DBNull?DateTime.Now:
                TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["SlotRangeStart"]),est);
            occurrence.SlotRange.End = reader["SlotRangeEnd"] is DBNull? DateTime.Now:
                TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["SlotRangeEnd"]),est);

            return occurrence;

        }

        #endregion fromreader

        #region to_parameter_set

        public override MySql.Data.MySqlClient.MySqlParameter[] BindToSqlVariableArray(Occurrence obj, bool identity)
        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (identity)
                parameters.Add(new MySqlParameter("_OccurrenceId", obj.Id()));

            parameters.Add(new MySqlParameter("_StoreId", obj.Store.Id().ToString())); 
            parameters.Add(new MySqlParameter("_Start", obj.Start.ToUniversalTime()));
            parameters.Add(new MySqlParameter("_End", obj.End.ToUniversalTime()));
            parameters.Add(new MySqlParameter("_ResEventId", obj.ResEvent.Id()));
            parameters.Add(new MySqlParameter("_Status", obj.Status));
            parameters.Add(new MySqlParameter("_BoundToPrototype", obj.BoundToPrototype));
            parameters.Add(new MySqlParameter("_SlotRangeStart", obj.SlotRangeStart.ToUniversalTime()));
            parameters.Add(new MySqlParameter("_SlotRangeEnd", obj.SlotRangeEnd.ToUniversalTime()));


            return (parameters.ToArray());

        }

        #endregion to_parameter_set

        #region accessor_and_assertion

        public override Occurrence Load(string ID)
        {
            return BindFromStoredProcedure("Occurrence_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        

        public OccurrenceCollection LoadByResEvent(string ID)
        {
            return new OccurrenceCollection(BindListFromStoredProcedure("Occurrences_GetByResId", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            }));
        }

        public override bool Save(Occurrence obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("Occurrence_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("Occurrence_Insert", obj, false);
                if (id > 0)
                    obj.Id(id.ToString());
                return id > 0;
            }
        }

        public override bool Delete(Occurrence obj)
        {
            return AssertToStoredProcedure("Occurrence_Delete", obj, true) > 0;
        }

        #endregion accessor_and_assertion
    }
}
