using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfacore.shared.domain._base
{
    [Serializable]
    public class DateRange:IDateRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> class.
        /// </summary>
        /// <param name="Start">The start.</param>
        /// <param name="End">The end.</param>
        public DateRange(DateTime Start,DateTime End)
        {
            this.Start = Start;
            this.End = End;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> class.
        /// </summary>
        /// <param name="Start">The start.</param>
        /// <param name="End">The end.</param>
        public DateRange(string Start, string End)
        {
            this.Start = DateTime.Parse(Start);
            this.End = DateTime.Parse(End);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> class.
        /// </summary>
        /// <param name="Start">The start.</param>
        /// <param name="Span">The span.</param>
        public DateRange(DateTime Start, TimeSpan Span)
        {
            this.Start = Start;
            this.Span = Span;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> class.
        /// </summary>
        /// <param name="Start">The start.</param>
        public DateRange(DateTime Start)
        {
            this.Start = Start;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> class.
        /// </summary>
        /// <param name="Start">The start.</param>
        public DateRange(string Range)
        {
            string[] strDates = Range.Split(',');
            if(strDates.Length>0 && !string.IsNullOrEmpty(strDates[0]))
                this.Start = DateTime.Parse(strDates[0]);
            
            if (strDates.Length > 1 && !string.IsNullOrEmpty(strDates[1]))
                this.End = DateTime.Parse(strDates[1]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> class.
        /// </summary>        
        public DateRange()
        {

        }


        
        private DateTime _Start;
        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public DateTime Start
        {
            get
            {
                return this._Start;
            }
            set
            {
                this._Start = value;
            }
        }

        private DateTime _End;

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        public DateTime End
        {
            get
            {
                return this._End;
            }
            set
            {
                this._End = value;
            }
        }

        /// <summary>
        /// Gets or sets the span.
        /// </summary>
        /// <value>
        /// The span.
        /// </value>
        public TimeSpan Span
        {
            get
            {
                return this._End - this._Start;
            }
            set
            {
                this._End = this._Start + value;
            }
        }

        public DateRange Add(TimeSpan timeSpan)
        {
            DateRange dr = new DateRange();
            dr.Start = Start.Add(timeSpan);
            dr.End = End.Add(timeSpan);
            return dr;
        }
    }
}
