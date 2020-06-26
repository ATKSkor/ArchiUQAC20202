using System.Collections.Generic;

namespace StableAPI.Models.Dto
{
    public class CalendarDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int StableID { get; set; }

        public List<EventDto> Events { get; set; }
    }
}