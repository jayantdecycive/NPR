using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using MySql.Data.MySqlClient;
using cfares.domain._event;
using cfares.domain.user;
using cfacore.mysql.dao.res;
using cfares.domain._event.slot.tours;

namespace cfacore.mysql.dao._event
{
    public class ScheduleMySqlAccess : MySqlAccessObject<Schedule>
    {
        public ScheduleMySqlAccess(string connectionString)
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
        public override Schedule BindFromReader(MySqlDataReader reader, string qualifier)
        {
            return BindFromReader<Schedule>(reader, qualifier);
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public Q BindFromReader<Q>(MySqlDataReader reader, string qualifier) where Q : Schedule, new()
        {
            Q schedule = new Q();

            schedule.Id ( Convert.ToString(reader["ScheduleId"]));
            schedule.Capacity = Convert.ToInt32(reader["Capacity"]);
            schedule.End = Convert.ToDateTime(reader["End"]);
            schedule.Start = Convert.ToDateTime(reader["Start"]);
            schedule.Name = Convert.ToString(reader["Name"]);
            schedule.UrlName = Convert.ToString(reader["UrlName"]);
            schedule.Region = Convert.ToInt32(reader["Region"]);

            return schedule;
        }


        #endregion fromreader

        #region to_parameter_set

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [[identity]].</param>
        /// <returns></returns>
        public override MySqlParameter[] BindToSqlVariableArray(Schedule obj, bool identity)

        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            
            if(identity)
                parameters.Add(new MySqlParameter("_ScheduleId",obj.Id()));
            parameters.Add(new MySqlParameter("_Capacity", obj.Capacity));

            parameters.Add(new MySqlParameter("_End",obj.End));
            parameters.Add(new MySqlParameter("_Start",obj.Start));
            parameters.Add(new MySqlParameter("_Name", obj.Name));
            parameters.Add(new MySqlParameter("_UrlName", obj.UrlName));
            parameters.Add(new MySqlParameter("_Region", obj.Region));

            return (parameters.ToArray());
        }

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [[identity]].</param>
        /// <returns></returns>
        public MySqlParameter[] BindScheduleIdToArray(Schedule obj, bool identity)
        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            
            parameters.Add(new MySqlParameter("_Id", obj.ScheduleId));            

            return (parameters.ToArray());
        }


        #endregion to_parameter_set

        #region accessor_and_assertion

        public override Schedule Load(string ID)
        {
            return BindFromStoredProcedure("Schedule_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

                

        /// <summary>
        /// Saves the specified obj. Very IMPORTANT! Keep this as reference on how to properly extend an object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override bool Save(Schedule obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("Schedule_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("Schedule_Insert", obj, false);
                if (id > 0)
                    obj.Id  (id.ToString());
                return id > 0;
            }
        }

        public override bool Delete(Schedule obj)
        {
            return AssertToStoredProcedure("Schedule_Delete", new MySqlParameter[] { 
                new MySqlParameter("_Id",obj.ScheduleId)
            }) > 0;
        }


        public bool ApplyToSlot(Schedule obj,Slot slot)
        {
            return AssertToStoredProcedure("$Util~ApplyScheduleToSlot", new MySqlParameter[] { 
                new MySqlParameter("_ScheduleId",obj.ScheduleId),
                new MySqlParameter("_SlotId",slot.SlotId)
            }) > 0;
        }

    


        #endregion accessor_and_assertion

        //Schedules_GetByOwnerAndStatus



        public Schedule GetScheduleWithinTimeRangeAndRegion(DateTime start, DateTime end,int region)
        {
            return BindFromStoredProcedure("Schedule_GetWithinTimeRangeAndRegion", new MySqlParameter[] { 
                new MySqlParameter("_Start",start),
                new MySqlParameter("_End",end),
                new MySqlParameter("_Region",region)
            });
        }
    }
}
