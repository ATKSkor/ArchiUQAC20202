using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StableAPI.Models
{
    public class Stable
    {
        public int ID { get; set; }
        public int BossID { get; set; }

        [ForeignKey("BossID")] public Person Boss { get; set; }
        public ICollection<Box> Boxes { get; set; }
        public ICollection<Calendar> Calendars { get; set; }
        public ICollection<Membership> Memberships { get; set; }
        public ICollection<Staff> Staff { get; set; }
        public ICollection<StockEntry> StockEntries { get; set; }
    }
}