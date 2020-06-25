using System.Collections.Generic;


namespace StableAPI.Models.Dto
{
    public class PersonDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public virtual ICollection<int> HorsesIDs { get; set; }
        public ICollection<int> MembershipsIDs { get; set; }
    }
}
