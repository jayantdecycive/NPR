using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using MySql.Data.MySqlClient;
using cfares.domain._event;
using cfares.domain._event.slot.tours;
using cfares.domain.user;
using cfares.domain._event.slot;
using cfacore.mysql.dao.res;

namespace cfacore.mysql.dao._event
{
    public class SlotMySqlAccess : MySqlAccessObject<Slot>
    {
        public SlotMySqlAccess(string connectionString)
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
        public override Slot BindFromReader(MySqlDataReader reader, string qualifier)
        {
            return BindFromReader<Slot>(reader,qualifier);
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public Q BindFromReader<Q>(MySqlDataReader reader, string qualifier) where Q:Slot,new ()
        {
            Q slot = new Q();
            
            slot.Id ( Convert.ToString(reader["SlotId"]));
            slot.Status = (SlotStatus)Convert.ToInt32(reader["Status"]);
            slot.Capacity = Convert.ToInt32(reader["Capacity"]);
            slot.Occurrence = new Occurrence(Convert.ToString(reader["OccurrenceId"]));
            slot.IsScheduled = Convert.ToBoolean(reader["IsScheduled"]);
            
            slot.Schedule = new Schedule(Convert.ToString(reader["ScheduleId"]));
            
            slot.Start = (Convert.ToDateTime(reader["Start"]));
            slot.End = (Convert.ToDateTime(reader["End"]));
            if (reader["Cutoff"]!=Convert.DBNull)
                slot.Cutoff = (Convert.ToDateTime(reader["Cutoff"]));
            
            return slot;
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public Q BindFromReaderWithCount<Q>(MySqlDataReader reader, string qualifier) where Q : Slot, new()
        {
            Q slot = BindFromReader<Q>(reader, qualifier);

            if (reader["TotalTickets"] != Convert.DBNull)
            {
                slot.TotalTickets = Convert.ToInt32(reader["TotalTickets"]);
            }
            else {
                slot.TotalTickets = 0;
            }

            return slot;
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public Q BindFromTourReaderWithCount<Q>(MySqlDataReader reader, string qualifier) where Q : TourSlot, new()
        {
            Q slot = BindFromReader<Q>(reader, qualifier);

            if (reader["TotalPersons"] != Convert.DBNull)
            {
                slot.TotalTickets = Convert.ToInt32(reader["TotalPersons"]);
            }else if (reader["TotalTickets"] != Convert.DBNull)
            {
                slot.TotalTickets = Convert.ToInt32(reader["TotalTickets"]);
            }
            else
            {
                slot.TotalTickets = 0;
            }

            return slot;
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public TourSlot BindTourFromReader(MySqlDataReader reader, string qualifier)
        {
            TourSlot slot = BindFromReader<TourSlot>(reader,qualifier);

            slot.Id (Convert.ToString(reader["Slot:TourId"]));
            slot.Guide = new ResAdmin(Convert.ToString(reader["GuideId"]));
            slot.KidFriendly = Convert.ToBoolean(reader["KidFriendly"]);
            slot.SpecialNeeds = Convert.ToString(reader["SpecialNeeds"]);            
            

            return slot;
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public TourSlot BindTourFromReaderWithOccurrenceAndEvent(MySqlDataReader reader, string qualifier)
        {
            TourSlot slot = BindTourFromReader(reader, qualifier);

            ResUserMySqlAccess userAccess = new ResUserMySqlAccess(this.ConnectionString);
            ResEventMySqlAccess eventAccess = new ResEventMySqlAccess(this.ConnectionString);
            OccurrenceMySqlAccess occurrenceAccess = new OccurrenceMySqlAccess(this.ConnectionString);

            slot.Guide = userAccess.BindFromReaderShort<ResAdmin>(reader, qualifier);
            slot.Occurrence = occurrenceAccess.BindFromReaderShort(reader, qualifier);
            slot.Occurrence.ResEvent = eventAccess.BindFromReaderShort(reader, qualifier);


            return slot;
        }

        #endregion fromreader

        #region to_parameter_set

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [[identity]].</param>
        /// <returns></returns>
        public override MySqlParameter[] BindToSqlVariableArray(Slot obj, bool identity)
        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if(identity)
                parameters.Add(new MySqlParameter("_SlotId",obj.Id()));            

            parameters.Add(new MySqlParameter("_OccurrenceId",obj.Occurrence.Id()));
            parameters.Add(new MySqlParameter("_Start", obj.Start.ToUniversalTime()));
            parameters.Add(new MySqlParameter("_Capacity", obj.Capacity));
            parameters.Add(new MySqlParameter("_Status", obj.Status));
            parameters.Add(new MySqlParameter("_ScheduleId", obj.ScheduleId));
            parameters.Add(new MySqlParameter("_IsScheduled", obj.IsScheduled));
            parameters.Add(new MySqlParameter("_End", obj.End.ToUniversalTime()));
            parameters.Add(new MySqlParameter("_Cutoff", obj.Cutoff.ToUniversalTime()));


            return (parameters.ToArray());

        }


        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [[identity]].</param>
        /// <returns></returns>
        public MySqlParameter[] BindToSqlVariableArray(TourSlot obj, bool identity)
        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            //if(identity)
              //  parameters.Add(new MySqlParameter("_Slot_TourId", obj.TourSlotId));
            if(obj.Guide!=null)
                parameters.Add(new MySqlParameter("_GuideId",obj.Guide.Id()));
            else
                parameters.Add(new MySqlParameter("_GuideId", null));
            parameters.Add(new MySqlParameter("_SlotId", obj.Id()));                    
            parameters.Add(new MySqlParameter("_KidFriendly",obj.KidFriendly));
            parameters.Add(new MySqlParameter("_SpecialNeeds",obj.SpecialNeeds));
            
            
            return (parameters.ToArray());

        }

        #endregion to_parameter_set

        #region accessors

        public SlotCollection LoadByResEvent(string ID)
        {
            return new SlotCollection(BindListFromStoredProcedure("Slots_GetByResId", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            }));
        }

        public SlotCollection LoadByResEvent(ResEvent resEvent)
        {
            return new SlotCollection(BindListFromStoredProcedure("Slots_GetByResId", new MySqlParameter[] { 
                new MySqlParameter("_Id",resEvent.Id())
            }));
        }

        public SlotCollection LoadByOccurrence(string ID)
        {
            return new SlotCollection(BindListFromStoredProcedure("Slots_GetByOccurrenceId", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            }));
        }

        public SlotCollection LoadByApproximateDateRange(DateTime hour, string eventType)
        {
            return new SlotCollection(BindListFromStoredProcedure("Slot:Tour_GetByApproxStart", new MySqlParameter[] { 
                new MySqlParameter("_Hour",hour.ToUniversalTime()),                                
                new MySqlParameter("_EventType",eventType)
            }));
        }
        
        public List<Slot> LoadByOccurrence(Occurrence occurrence)
        {
            return BindListFromStoredProcedure("Slots_GetByOccurrenceId", 
                new MySqlParameter[] { 
                new MySqlParameter("_Id",occurrence.Id())
            });
        }

        public override Slot Load(string ID)
        {
            return BindFromStoredProcedure("Slot_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public List<Slot> LoadAll()
        {
            return BindListFromStoredProcedure("Slot_GetAll", new MySqlParameter[] { 
                
            });
        }

        public TourSlot LoadTour(string ID)
        {
            return BindFromStoredProcedure<TourSlot>("Slot:Tour_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            },new ObjectBinder<TourSlot>(BindTourFromReader));
        }

        public TourSlot LoadTourSlotWithCapacity(string ID)
        {
            return BindFromStoredProcedure<TourSlot>("Slot:Tour_GetById_WithCapacity", new MySqlParameter[] {
                 new MySqlParameter("_Id",ID)
            }, new ObjectBinder<TourSlot>(BindFromTourReaderWithCount<TourSlot>));
        }

        public TourSlot LoadTourWithGuideOccurrenceAndEvent(string ID)
        {
            return BindFromStoredProcedure<TourSlot>("Slot:Tour$Guide$Occurrence$Event_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            }, new ObjectBinder<TourSlot>(BindTourFromReaderWithOccurrenceAndEvent));
        }

        public SlotCollection GetCameoSlots(string UserId)
        {
            return new SlotCollection(BindListFromStoredProcedure(
                "Slot:Tour$Cameos_GetSlotsByUserId",
                new MySqlParameter[] { 
                    new MySqlParameter("_Id",UserId)
                }
            ));
        }

        public SlotCollection GetSlotsByEventTypeWithDateRange(string EventType,DateTime Start, DateTime End)
        {
            return new SlotCollection(BindListFromStoredProcedure(
                "Slots_GetByEventTypeWithinDateRange",
                new MySqlParameter[] { 
                    new MySqlParameter("_Type",EventType),
                    new MySqlParameter("_Start",Start.ToUniversalTime()),
                    new MySqlParameter("_End",End.ToUniversalTime())
                },
                    new ObjectBinder(BindFromReaderWithCount<Slot>)
            ));
        }

        public TourSlotCollection GetTourSlotsByEventTypeWithDateRange(string EventType, DateTime Start, DateTime End)
        {

            List<TourSlot> slots = BindListFromStoredProcedure<TourSlot>(
                "Slot:Tours_GetByEventTypeWithinDateRange",
                new MySqlParameter[] { 
                    new MySqlParameter("_Type",EventType),
                    new MySqlParameter("_Start",Start.ToUniversalTime()),
                    new MySqlParameter("_End",End.ToUniversalTime())
                },
                new ObjectBinder<TourSlot>(BindFromTourReaderWithCount<TourSlot>)
            );

            return new TourSlotCollection(slots);
        }

        public SlotCollection LoadBySchedule(Schedule obj)
        {
            List<Slot> slots = BindListFromStoredProcedure(
                "Slot_GetByScheduleId",
                new MySqlParameter[] {                     
                    new MySqlParameter("_ScheduleId",obj.ScheduleId)
                }
                
            );

            return new SlotCollection(slots);
        }

        #endregion accessors

        #region assertion

        public override bool Save(Slot obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("Slot_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("Slot_Insert", obj, false);
                if (id > 0)
                    obj.Id(id.ToString());
                return id > 0;
            }
        }

        public bool Save(TourSlot obj)
        {
            if (Save((Slot)obj))
            {
                if (!string.IsNullOrEmpty(obj.Id()))
                    return AssertToStoredProcedure("Slot:Tour_Update", obj, true, new ParameterBinder<TourSlot>(BindToSqlVariableArray)) > 0;
                else
                {
                    int id = AssertToStoredProcedure("Slot:Tour_Insert", obj, false, new ParameterBinder<TourSlot>(BindToSqlVariableArray));
                    if (id > 0)
                        obj.Id( id.ToString());
                    return id > 0;
                }
            }
            return false;
        }

        public override bool Delete(Slot obj)
        {
            return AssertToStoredProcedure("Slot_Delete", new MySqlParameter[] { 
                    new MySqlParameter("_Id",obj.SlotId)
                    
                }) > 0;
        }

        #endregion assertion

        
    }
}
