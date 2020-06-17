using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StableAPI.Models
{
    public class Calendar
    {
        public int ID { get; set; }
        [Required] public int StableID { get; set; }

        public Stable Stable { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}