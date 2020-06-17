using System.Collections.Generic;

namespace StableAPI.Models
{
    public class Membership
    {
        public int StableID { get; set; }
        public int PersonID { get; set; }

        public Stable Stable { get; set; }
        public Person Person { get; set; }
        public ICollection<Bill> Bills { get; set; }
        public ICollection<Registration> Registrations { get; set; }
    }
}