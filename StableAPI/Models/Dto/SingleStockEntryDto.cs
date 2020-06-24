using System;
using System.Collections.Generic;

namespace StableAPI.Models.Dto
{
    public class SingleStockEntryDto
    {
        public int StockItemId { get; set; }
        public int StableId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }

        public class ReservationDto
        {
            public int ID { get; set; }
            public int Quantity { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public List<ReservationDto> Reservations { get; set; }
    }
}