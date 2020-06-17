namespace StableAPI.Models
{
    public class MedicEntry
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public byte[] Report { get; set; }
        public int HorseID { get; set; }

        public Horse Horse { get; set; }
    }
}