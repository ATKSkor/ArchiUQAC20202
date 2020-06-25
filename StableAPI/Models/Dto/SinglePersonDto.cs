using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StableAPI.Models.Dto
{
    public class SinglePersonDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public virtual ICollection<Horse> Horses { get; set; }
        public ICollection<Membership> Memberships { get; set; }
    }
}
