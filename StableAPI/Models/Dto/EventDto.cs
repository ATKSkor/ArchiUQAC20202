using System;

namespace StableAPI.Models.Dto
{
    public class EventDto
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EventType { get; set; }
    }
}