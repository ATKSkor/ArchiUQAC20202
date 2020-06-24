using System.Collections.Generic;

namespace StableAPI.Models
{
    public class StockItem
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public virtual ICollection<StockEntry> StockEntries { get; set; }
    }
}