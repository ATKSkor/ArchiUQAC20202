using System.ComponentModel.DataAnnotations.Schema;

namespace StableAPI.Models
{
    public class Secret
    {
        public int ID { get; set; }
        public int OwnerID { get; set; }
        public byte[] Data { get; set; }

        public Person Owner { get; set; }
    }
}