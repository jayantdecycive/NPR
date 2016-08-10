using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using MySql.Data.MySqlClient;
using cfares.domain._event;
using cfares.domain._event._ticket.tours;
using cfares.domain.user;
using cfares.domain._event._ticket;
using cfacore.mysql.dao.res;
using cfares.domain._event.slot.tours;

namespace cfacore.mysql.dao._event
{
    public class TicketMySqlAccess : MySqlAccessObject<Ticket>
    {
        public TicketMySqlAccess(string connectionString)
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
        public override Ticket BindFromReader(MySqlDataReader reader, string qualifier)
        {
            return BindFromReader<Ticket>(reader, qualifier);
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public Q BindFromReader<Q>(MySqlDataReader reader, string qualifier) where Q : Ticket, new()
        {
            Q ticket = new Q();

            ticket.Id ( Convert.ToString(reader["TicketId"]));
            ticket.CardNumber = Convert.ToString(reader["CardNumber"]);
            ticket.Slot = new Slot(Convert.ToString(reader["SlotId"]));
            ticket.Owner = new ResUser(Convert.ToString(reader["OwnerId"]));
            ticket.CreationSrc = Convert.ToString(reader["CreationSrc"]);
            ticket.Note = Convert.ToString(reader["Note"]);

            return ticket;
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public TourTicket BindTourTicketFromReader(MySqlDataReader reader, string qualifier)

        {

            TourTicket tourticket = BindFromReader<TourTicket>(reader,qualifier);
            
            //tourticket.TourTicketId = Convert.ToString(reader["Ticket:TourId"]);
            tourticket.GuestCount = Convert.ToInt32(reader["GuestCount"]);
            
            // Lunch Options
            tourticket.NumberOfAdultLunches = Convert.ToInt32(reader["NumberOfAdultLunches"]);
            tourticket.NumberOfKidLunches = Convert.ToInt32(reader["NumberOfKidLunches"]);
            tourticket.TotalCostOfLunches = Convert.ToDecimal(reader["TotalPriceOfLunch"]);
            tourticket.NumberOfSpecialNeedLunches = Convert.ToInt32(reader["NumberOfSpecialNeedLunches"]);
            tourticket.SpecialDietNeedsDescription = Convert.ToString(reader["SpecialDietNeedsDescription"]);
            tourticket.OptInForLunch = Convert.ToBoolean(reader["LunchOptIn"]);

            //Visitation Options
            tourticket.VisitTech = Convert.ToBoolean(reader["VisitTech"]);
            tourticket.VisitMarketing = Convert.ToBoolean(reader["VisitMarketing"]);
            tourticket.VisitInnovation = Convert.ToBoolean(reader["VisitInnovation"]);
            tourticket.VisitTraining = Convert.ToBoolean(reader["VisitTraining"]);
            tourticket.VisitWellness = Convert.ToBoolean(reader["VisitWellness"]);
            tourticket.VisitWareHouse = Convert.ToBoolean(reader["VisitWarehouse"]);
            tourticket.VisitOther = Convert.ToBoolean(reader["VisitOther"]);
            tourticket.VisitOtherDescription = Convert.ToString(reader["OtherVisitDescription"]);

            // Special Needs
            tourticket.HasSpecialNeeds = Convert.ToBoolean(reader["HasSpecialNeeds"]);
            tourticket.IsVisuallyImpaired = Convert.ToBoolean(reader["IsVisuallyImpaired"]);
            tourticket.IsHearingImpaired = Convert.ToBoolean(reader["IsHearingImpaired"]);
            tourticket.NeedsWheelChair = Convert.ToBoolean(reader["NeedsWheelChair"]);
            tourticket.OtherNeeds = Convert.ToBoolean(reader["OtherNeeds"]);
            tourticket.OtherNeedsDescription = Convert.ToString(reader["OtherNeedsDescription"]);

            // Group Type
            tourticket.IsSchoolGroup = Convert.ToBoolean(reader["IsSchoolGroup"]);
            tourticket.IsFamilyWithoutKids = Convert.ToBoolean(reader["IsFamilyWithoutKids"]);
            tourticket.IsFamilyWithKids = Convert.ToBoolean(reader["IsFamilyWithKids"]);
            tourticket.IsAdultGroup = Convert.ToBoolean(reader["IsAdultGroup"]);
            tourticket.IsReligiousGroup = Convert.ToBoolean(reader["IsReligiousGroup"]);
            tourticket.IsSeniorGroup = Convert.ToBoolean(reader["IsSeniorGroup"]);
            tourticket.IsBusinessGroup = Convert.ToBoolean(reader["IsBusinessGroup"]);
            tourticket.IsRavingFans = Convert.ToBoolean(reader["IsRavingFans"]);
            tourticket.IsTeamMemberGroup = Convert.ToBoolean(reader["IsTeamMemberGroup"]);
            tourticket.IsOtherTypeOfGroup = Convert.ToBoolean(reader["IsOtherTypeGroup"]);
            tourticket.OtherTypeDescription = Convert.ToString(reader["OtherTypeDescription"]);
            tourticket.GroupName = Convert.ToString(reader["GroupName"]);

            tourticket.GuestList = (Convert.ToString(reader["Guests"]));

            return tourticket;

        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public TourTicket BindTourTicketWithOwnerFromReader(MySqlDataReader reader, string qualifier)
        {

            TourTicket tourticket = BindTourTicketFromReader(reader, qualifier);

            ResUserMySqlAccess userSql = new ResUserMySqlAccess(ConnectionString);
            tourticket.Owner = userSql.BindFromReader(reader, qualifier);

            return tourticket;

        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public TourTicket BindTourTicketWithOwnerAndSlotFromReader(MySqlDataReader reader, string qualifier)
        {

            TourTicket tourticket = BindTourTicketWithOwnerFromReader(reader, qualifier);

            SlotMySqlAccess slotSql = new SlotMySqlAccess(ConnectionString);
            tourticket.Slot = slotSql.BindFromReader<TourSlot>(reader, qualifier);

            return tourticket;

        }

        #endregion fromreader

        #region to_parameter_set

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [[identity]].</param>
        /// <returns></returns>
        public override MySqlParameter[] BindToSqlVariableArray(Ticket obj, bool identity)

        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if(identity)
                parameters.Add(new MySqlParameter("_TicketId",obj.Id()));            
            parameters.Add(new MySqlParameter("_CardNumber",obj.CardNumber));

            parameters.Add(new MySqlParameter("_SlotId",obj.SlotId));
            parameters.Add(new MySqlParameter("_OwnerId",obj.Owner.Id()));
            parameters.Add(new MySqlParameter("_CreationSrc", obj.CreationSrc));
            parameters.Add(new MySqlParameter("_Note", obj.Note));

            return (parameters.ToArray());
        }

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [[identity]].</param>
        /// <returns></returns>
        public MySqlParameter[] BindTicketIdToArray(Ticket obj, bool identity)
        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            
            parameters.Add(new MySqlParameter("_Id", obj.TicketId));            

            return (parameters.ToArray());
        }

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [[identity]].</param>
        /// <returns></returns>
        public MySqlParameter[] BindToSqlVariableArray(TourTicket obj, bool identity)

        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            //if(identity)
            //parameters.Add(new MySqlParameter("_Id", obj.TourTicketId));                
            parameters.Add(new MySqlParameter("_GuestCount",obj.GuestCount));
            parameters.Add(new MySqlParameter("_TicketId", obj.Id()));
            parameters.Add(new MySqlParameter("_CreationSrc", obj.CreationSrc));

            // Lunch option parameters
            parameters.Add(new MySqlParameter("_NumberOfAdultLunches",obj.NumberOfAdultLunches));
            parameters.Add(new MySqlParameter("_NumberOfKidLunches",obj.NumberOfKidLunches));
            parameters.Add(new MySqlParameter("_TotalPriceOfLunch", obj.TotalCostOfLunches));
            parameters.Add(new MySqlParameter("_NumberOfSpecialNeedLunches", obj.NumberOfSpecialNeedLunches));
            parameters.Add(new MySqlParameter("_SpecialDietNeedsDescription", obj.SpecialDietNeedsDescription));
            parameters.Add(new MySqlParameter("_LunchOptIn", obj.OptInForLunch));
            

            // Visitation options parameters
            parameters.Add(new MySqlParameter("_VisitTech", obj.VisitTech));
            parameters.Add(new MySqlParameter("_VisitMarketing", obj.VisitMarketing));
            parameters.Add(new MySqlParameter("_VisitInnovation", obj.VisitInnovation));
            parameters.Add(new MySqlParameter("_VisitTraining", obj.VisitTraining));
            parameters.Add(new MySqlParameter("_VisitWellness", obj.VisitWellness));
            parameters.Add(new MySqlParameter("_VisitWareHouse", obj.VisitWareHouse));
            parameters.Add(new MySqlParameter("_VisitOther", obj.VisitOther));
            parameters.Add(new MySqlParameter("_OtherVisitDescription", obj.VisitOtherDescription));

            // Special needs parameters
            parameters.Add(new MySqlParameter("_HasSpecialNeeds", obj.HasSpecialNeeds));
            parameters.Add(new MySqlParameter("_IsVisuallyImpaired", obj.IsVisuallyImpaired));
            parameters.Add(new MySqlParameter("_IsHearingImpaired", obj.IsHearingImpaired));
            parameters.Add(new MySqlParameter("_NeedsWheelChair", obj.NeedsWheelChair));
            parameters.Add(new MySqlParameter("_OtherNeeds", obj.OtherNeeds));
            parameters.Add(new MySqlParameter("_OtherNeedsDescription", obj.OtherNeedsDescription));

            // Group type parameters
            parameters.Add(new MySqlParameter("_IsSchoolGroup", obj.IsSchoolGroup));
            parameters.Add(new MySqlParameter("_IsFamilyWithoutKids", obj.IsFamilyWithoutKids));
            parameters.Add(new MySqlParameter("_IsFamilyWithKids", obj.IsFamilyWithKids));
            parameters.Add(new MySqlParameter("_IsAdultGroup", obj.IsAdultGroup));
            parameters.Add(new MySqlParameter("_IsReligiousGroup", obj.IsReligiousGroup));
            parameters.Add(new MySqlParameter("_IsSeniorGroup", obj.IsSeniorGroup));
            parameters.Add(new MySqlParameter("_IsBusinessGroup", obj.IsBusinessGroup));
            parameters.Add(new MySqlParameter("_IsRavingFans", obj.IsRavingFans));
            parameters.Add(new MySqlParameter("_IsTeamMemberGroup", obj.IsTeamMemberGroup));
            parameters.Add(new MySqlParameter("_IsOtherTypeGroup", obj.IsOtherTypeOfGroup));
            parameters.Add(new MySqlParameter("_OtherTypeDescription", obj.OtherTypeDescription));
            parameters.Add(new MySqlParameter("_GroupName", obj.GroupName));


            parameters.Add(new MySqlParameter("_Guests", obj.GuestList));            


            return (parameters.ToArray());

        }

        #endregion to_parameter_set

        #region accessor_and_assertion

        public override Ticket Load(string ID)
        {
            return BindFromStoredProcedure("Ticket_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public TicketCollection LoadToursByGroupName(string GroupName)
        {
            return (BindListFromStoredProcedure("Tickets:Tour_GetByGroupName", new MySqlParameter[] { 
                new MySqlParameter("_GroupName",GroupName)
            })) as TicketCollection;
        }

        public TicketCollection LoadByResEvent(string ID)
        {
            return (BindListFromStoredProcedure("Tickets_GetByResId", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            })) as TicketCollection;
        }

        public TicketCollection LoadBySlot(string ID)
        {
            return BindListFromStoredProcedure("Tickets_GetBySlotId", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            }) as TicketCollection;
        }

        public TourTicketCollection LoadToursBySlot(string ID)
        {
            return new TourTicketCollection(BindListFromStoredProcedure<TourTicket>("Tickets:Tour_GetBySlotId", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            },new ObjectBinder<TourTicket>(BindTourTicketFromReader)));
        }

        public TourTicketCollection LoadToursByOwner(string ID)
        {
            return new TourTicketCollection(BindListFromStoredProcedure<TourTicket>("Tickets:Tour_GetByOwnerId", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            }, new ObjectBinder<TourTicket>(BindTourTicketFromReader)));
        }

        public TourTicketCollection LoadToursAndOwnersBySlot(string ID)
        {
            return new TourTicketCollection(BindListFromStoredProcedure<TourTicket>("Tickets:Tour~Owner_GetBySlotId", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            }, new ObjectBinder<TourTicket>(BindTourTicketWithOwnerFromReader)));
        }

        public TourTicket LoadTour(string ID)
        {
            return BindFromStoredProcedure<TourTicket>("Ticket:Tour_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            },new ObjectBinder<TourTicket>(BindTourTicketFromReader));
        }

        public TourTicket LoadTourWithOwner(string ID)
        {
            return BindFromStoredProcedure<TourTicket>("Ticket:Tour~Owner_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            }, new ObjectBinder<TourTicket>(BindTourTicketWithOwnerFromReader));
        }

        public TourTicket LoadTourWithOwnerAndSlot(string ID)
        {
            return BindFromStoredProcedure<TourTicket>("Ticket:Tour~Owner$Slot_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            }, new ObjectBinder<TourTicket>(BindTourTicketWithOwnerAndSlotFromReader));
        }

        

        /// <summary>
        /// Saves the specified obj. Very IMPORTANT! Keep this as reference on how to properly extend an object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public bool Save(TourTicket obj)
        {
            if (Save((Ticket)obj))
            {
                /*if (!string.IsNullOrEmpty(obj.TourTicketId))
                    return AssertToStoredProcedure("Ticket:Tour_Update", obj, true, new ParameterBinder<TourTicket>(BindToSqlVariableArray)) > 0;
                else
                {
                    int id = AssertToStoredProcedure("Ticket:Tour_Insert", obj, false, new ParameterBinder<TourTicket>(BindToSqlVariableArray));
                    if (id > 0)
                        obj.TourTicketId = (id.ToString());
                    return id > 0;
                }*/
                throw new NotImplementedException("This has been changed - we longer use this");
            }
            return false;
            
        }

        public override bool Save(Ticket obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("Ticket_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("Ticket_Insert", obj, false);
                if (id > 0)
                    obj.TicketId = (id);
                return id > 0;
            }
        }

        public override bool Delete(Ticket obj)
        {
            return AssertToStoredProcedure("Ticket_Delete", new MySqlParameter[] { 
                new MySqlParameter("_Id",obj.TicketId)
            }) > 0;
        }

    

        public bool Delete(TourTicket obj)
        {
            return AssertToStoredProcedure("Ticket_Delete", new MySqlParameter[] { 
                new MySqlParameter("_Id",obj.TicketId)
            }) > 0;
        }

        public TicketCollection LoadByUser(string UserID)
        {
            return (BindListFromStoredProcedure("Tickets_GetByOwner", new MySqlParameter[] { 
                new MySqlParameter("_UserId",UserID)
            })) as TicketCollection;
        }

        #endregion accessor_and_assertion

        //Tickets_GetByOwnerAndStatus

        
    }
}
