using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sync.locations;
using sync.media;
using cfares.domain._event.slot;
using cfacore.site.controllers.shared;
using cfares.domain._event;

namespace sync
{
    class Program
    {
        static string Query(string message) {
            Console.WriteLine(message);
            return Console.ReadLine();
        }

        static bool QueryBool(string message)
        {
            Console.WriteLine(string.Format("{0} (y/n)",message));
            string response = Console.ReadLine().ToLower();;
            return response == "y" || response == "t" || response == "true" || response == "yes";
        }

        static void Main(string[] args)
        {
            if (QueryBool("Iterate Slots?"))
            {
                SlotService serv = new SlotService();
                List<Slot> allSlots = serv.GetAll();
                foreach (Slot slot in allSlots)
                {
                    Console.Write("This method is sensative and is disabled in the code.");
                    break;
                    
                        /*try
                        {
                            serv.Save(slot);                            
                        }catch(Exception ex){
                            Console.WriteLine(ex.Message);
                        }*/
                    
                }

            }
            
            
            if (QueryBool("Parse Media?"))
            {
                MediaParser mparser = new MediaParser();
                mparser.Init();
            }

            
            
            
            if (QueryBool("Parse Locations?"))
            {
                LocationsParser lparse = new LocationsParser();
                //lparse.ClearTables();
                lparse.Init();

                FutureLocationsParser flparse = new FutureLocationsParser();
                flparse.Init();
            }

           
            
            
            
        }
    }
}
