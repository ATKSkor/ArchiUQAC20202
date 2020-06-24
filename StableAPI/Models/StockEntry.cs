using System.Collections.Generic;

namespace StableAPI.Models
{
    public class StockEntry
    {
        public int StableID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }

        public virtual Stable Stable { get; set; }
        public StockItem Item { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}