using System;

namespace StableAPI.Models
{
    public class Event
    {
        public int ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CalendarID { get; set; }
        public int EventTypeID { get; set; }

        public Calendar Calendar { get; set; }
        public EventType EventType { get; set; }
    }
}