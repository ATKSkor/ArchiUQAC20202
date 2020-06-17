using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StableAPI.Models
{
    public class Reservation
    {
        public int ID { get; set; }
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StableID { get; set; }
        public int ItemID { get; set; }

        [ForeignKey("ItemID")] public StockItem Item { get; set; }
        public StockEntry StockEntry { get; set; }
    }
}