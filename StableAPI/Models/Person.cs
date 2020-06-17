using System.Collections.Generic;

namespace StableAPI.Models
{
    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        
        public ICollection<Horse> Horses { get; set; }
        public ICollection<Membership> Memberships { get; set; }
        public User User { get; set; }
    }
}