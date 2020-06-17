using System.Collections.ObjectModel;

namespace StableAPI.Models
{
    public class StockItem
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public Collection<StockEntry> StockEntries { get; set; }
    }
}