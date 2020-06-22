using System;

namespace StableAPI.Models
{
    public class MedicEntry
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime IssuedDate { get; set; }
        public int HorseID { get; set; }
        public int ReportID { get; set; }

        public MedicReport Report { get; set; }
    }
}