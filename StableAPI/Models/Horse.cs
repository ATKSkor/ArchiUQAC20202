using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace StableAPI.Models
{
    public class Horse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int OwnerID { get; set; }
        public int? BoxID { get; set; }

        [ForeignKey("OwnerID")] public virtual Person Owner { get; set; }
        [ForeignKey("BoxID")] public virtual Box Box { get; set; }
        public ICollection<MedicEntry> MedicEntries { get; set; }
    }
}