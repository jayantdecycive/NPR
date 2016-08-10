using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.domain._event.occ;
using cfares.repository._event;
using cfares.repository.store;
using cfares.domain._event.slot;

namespace cfares_entity_manager.model
{
    public class EntityManager
    {
        public static CfaResContext context = new CfaResContext("Data Source=sql.decycivefarm.com;Initial Catalog=npr_auth;User Id=res_admin;Password=Beard7Beard7;");
        public static string CFAIdentityConnectionString = "Data Source=sql.decycivefarm.com;Initial Catalog=npr_auth;User Id=res_admin;Password=Beard7Beard7;";
        public static void Command(string cmd) {

            List<string> args = new List<string>(cmd.Split(' '));

            string model = args[0].ToLower();
            args.RemoveAt(0);

            string action = args[0].ToLower();
            args.RemoveAt(0);
            
            switch(model){
                case "test":
                    Test();
                    break;
                case "template":
                    TemplateCommand(action,args);
                    break;
                case "type":
                    EventTypeCommand(action, args);
                    break;
                case "event":
                    EventCommand(action, args);
                    break;
                
                default:
                    break;
            }
            Console.WriteLine();
        }

        public static void TemplateCommand(string action, List<string> args) { 
            switch(action){
                case "new":
                    ResTemplate temp = new ResTemplate();

                    ConsoleQueryAdapter oargs = new ConsoleQueryAdapter();
                    temp.BrowserMedia = oargs["BrowserMedia"];
                    temp.DefaultReservationTypeId = oargs["DefaultReservationTypeId"];
                    temp.Description = oargs["Description"];
                    temp.Name = oargs["Name"];
                    temp.ResTemplateId = oargs["ResTemplateId"];
                    context.ResTemplate.Add(temp);
                    context.SaveChanges();
                    break;
                default: 
                    break;
            }
        }

        public static void EventCommand(string action, List<string> args)
        {
            switch (action)
            {
                case "new":
                    ResEvent temp = new ResEvent();

                    ConsoleQueryAdapter oargs = new ConsoleQueryAdapter();
                    temp.Description = oargs["Description"];
                    temp.TemplateId = oargs["TemplateName"];
                    
                    temp.Urls = oargs["Url"];
                    context.ResEvents.Add(temp);
                    context.SaveChanges();
                    break;
                default:
                    break;
            }
        }

        
        public static void EventTypeCommand(string action, List<string> args)
        {
            switch (action)
            {
                case "new":
                    ReservationType temp = new ReservationType();

                    ConsoleQueryAdapter oargs = new ConsoleQueryAdapter();
                    temp.Description = oargs["Description"];
                    temp.Name = oargs["Name"];
                    temp.ReservationTypeId = oargs["Slug"];
                    temp.Urls = temp.ReservationTypeId;
                    temp.Type(typeof(ResEvent));
                    context.ReservationTypes.Add(temp);
                    context.SaveChanges();
                    
                    break;
                default:
                    break;
            }
        }

        public static void Test(){
            ResEventRepository repo = new ResEventRepository(context);
            OccurrenceRepository oRepo = new OccurrenceRepository(context);
            LocationRepository lRepo = new LocationRepository(context);
            /*GiveawayOccurrence myOcc = new GiveawayOccurrence();
            myOcc.ResEvent = repo.Find(7);
            myOcc.Store = lRepo.GetAll().First(x=>x.LocationNumber=="01942");
            myOcc.SlotRange = myOcc.ResEvent.GetRegistrationAvailability();
            myOcc.SlotRangeEnd = DateTime.Now;
            myOcc.SlotRangeStart = DateTime.Now;
            myOcc.Start = DateTime.Now;
            myOcc.End = DateTime.Now;
            

            oRepo.Add(myOcc);
            oRepo.Commit(true);*/
            //GiveawayOccurrence myOcc = oRepo.GetAll().First(x=>x is GiveawayOccurrence) as GiveawayOccurrence;
            var slot = new GiveawaySlot();
            slot.Start = DateTime.Now;
            slot.End = DateTime.Now;
            slot.Cutoff = DateTime.Now;
            var myOcc = oRepo.Context.GiveawayOccurrences.First();
            foreach(var gslot in myOcc.GiveawaySlotsList){
                Console.Write(gslot.Start);
            }
            //myOcc.SlotsList.Add(slot);
            //oRepo.Edit(myOcc);
            oRepo.Commit(true);
        }

        
    }
    public class ConsoleQueryAdapter{

        public string this[string key] {
            get { return Read(key); }            
        }

        public string Read(string key, bool password=false)
        {
            
            Console.Write("\n" + key + ": ");
            if(!password)
                return Console.ReadLine();
            else
            {
                return ReadPassword();
            }

            
        }

        public static string ReadPassword() {
        Stack passbits = new Stack();
        //keep reading
        for (ConsoleKeyInfo cki = Console.ReadKey(true); cki.Key != ConsoleKey.Enter; cki = Console.ReadKey(true)) {
        if (cki.Key == ConsoleKey.Backspace) {
        //rollback the cursor and write a space so it looks backspaced to the user
        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        Console.Write(" ");
        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        passbits.Pop();
        }
        else {
        Console.Write("*");
        passbits.Push(cki.KeyChar.ToString());
        }
        }
        string[] pass = passbits.ToArray() as string[];
        Array.Reverse(pass);
        return string.Join(string.Empty, pass);
        }

    }
}

