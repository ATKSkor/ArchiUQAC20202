using System.Collections.Generic;

namespace StableAPI.Models
{
    public class EventType
    {
        public int ID { get; set; }
        public string Label { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}