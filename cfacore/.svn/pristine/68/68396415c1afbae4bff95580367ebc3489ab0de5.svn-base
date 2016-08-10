using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cfacore.site.modules.log
{
    public class LDAPAuthLog
    {

        public void WriteEntry(string Source, string Message, EventLogEntryType EntryType)
        {

            System.Diagnostics.EventLog myEL = new System.Diagnostics.EventLog("Application", System.Environment.MachineName, Source);

            int eventID = 0;

            foreach (EventLogEntry _Entry in myEL.Entries)
            {

                if (_Entry.Source == Source)
                {

                    eventID++;

                }

            }

            myEL.WriteEntry(Message, EntryType, eventID);

            myEL.Close();

        }

    }

}