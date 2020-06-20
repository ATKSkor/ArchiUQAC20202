using System.Collections.Generic;

namespace StableAPI.Models.Dto
{
    public class SingleHorseDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int OwnerID { get; set; }
        public int BoxID { get; set; }

        public string OwnerFullName { get; set; }
        public ICollection<MedicEntry> MedicEntries { get; set; }
    }
}