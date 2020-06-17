using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StableAPI.Models
{
    public class Person
    {
        public int ID { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Surname { get; set; }
        
        public ICollection<Horse> Horses { get; set; }
        public ICollection<Membership> Memberships { get; set; }
    }
}