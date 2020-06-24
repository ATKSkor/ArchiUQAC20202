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
        public int StockEntryStableID { get; set; }
        public int StockEntryItemID { get; set; }

        public StockEntry StockEntry { get; set; }
    }
}