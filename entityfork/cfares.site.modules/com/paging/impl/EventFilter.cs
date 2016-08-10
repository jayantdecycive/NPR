using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cfares.Models.Paging;

namespace cfares.Models
{
    public class EventFilter: IFilter
    {
        private string startTimeAsString;
        public string startTimeValue
        {
            get
            {
                return startTimeAsString;

            }
            set
            {
                string startTimeAsString = value;

                DateTime valueAsTime;
                try
                {
                    valueAsTime = DateTime.Parse(startTimeAsString);
                }
                catch (FormatException e)
                {
                    throw new Exception("Start Time Format Exception", e);
                }

                FilterCriteria startTimeCriteria = new FilterCriteria
                {
                    filterName = "startTime",
                    filterCondition = "greater than or equal to",
                    filterValue = valueAsTime
                };
                filterCriteriaList.Add(startTimeCriteria);
            }

        }

        private string endTimeAsString;
        public string endTimeValue
        {
            get
            {
                return endTimeAsString;
            }
            set
            {
                endTimeAsString = value;

                DateTime valueAsTime;
                try
                {
                    valueAsTime = DateTime.Parse(endTimeAsString);
                }
                catch (FormatException e)
                {
                    throw new Exception("End Time Format Exception", e);
                }

                FilterCriteria endTimeCriteria = new FilterCriteria
                {
                    filterName = "endTime",
                    filterCondition = "lesser than or equal to",
                    filterValue = valueAsTime
                };
                filterCriteriaList.Add(endTimeCriteria);
            }
        }

    }
}