using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StableAPI.Models.Dto
{
    public class MembershipDto
    {
        public int? StableID { get; set; }
        public int? PersonID { get; set; }

        public Stable Stable { get; set; }
        public Person Person { get; set; }
        public ICollection<int> BillsID { get; set; }
        public ICollection<int> RegistrationsID { get; set; }
    }
}
