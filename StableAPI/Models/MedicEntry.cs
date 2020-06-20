namespace StableAPI.Models
{
    public class MedicEntry
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int HorseID { get; set; }
        public int ReportID { get; set; }

        public Horse Horse { get; set; }
        public MedicReport Report { get; set; }
    }
}