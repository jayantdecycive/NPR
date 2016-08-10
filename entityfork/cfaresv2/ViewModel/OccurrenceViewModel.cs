using cfacore.shared.domain.attributes;
using cfares.domain._event;
using cfares.domain.store;
using cfaresv2.ViewModel._base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using cfacore.shared.domain._base;

namespace cfaresv2.ViewModel
{
    public class OccurrenceViewModel:GenericViewModel<Occurrence>
    {
        /*[DataType("AutoComplete/_Store")]
        [UIHint("Badge/_Store")]
        public ResStore Store
        {
            get;
            set;
        }*/

        /*[DataType("Picker/_ResEvent")]
        [UIHint("Badge/_ResEvent")]
        [ClientDefault(1, type = typeof(string))]
        public ResEvent ResEvent
        {
            get;
            set;
        }*/

        public string StoreId
        {
            get;
            set;
        }

        public int? ResEventId
        {
            get;
            set;
        }

        public int OccurrenceId
        {
            get;
            set;
        }

        private DateRange _RegistrationAvailability = new DateRange();        
        [DataType("jqui/_DateRange")]
        [UIHint("type/_DateRange")]
        public DateRange RegistrationAvailability
        {
            get
            {
                if (this._RegistrationAvailability == null)
                    this._RegistrationAvailability = new DateRange();
                return this._RegistrationAvailability;
            }

            set
            {
                if (this._RegistrationAvailability == null)
                    this._RegistrationAvailability = new DateRange();
                this._RegistrationAvailability = value;
            }
        }

        public DateTime RegistrationAvailability_Start { get { return RegistrationAvailability.Start; } set { RegistrationAvailability.Start = value; } }
        public DateTime RegistrationAvailability_End { get { return RegistrationAvailability.End; } set { RegistrationAvailability.End = value; } }

    }
}