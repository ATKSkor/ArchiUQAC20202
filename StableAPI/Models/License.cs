using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StableAPI.Models
{
    public class License
    {
        public int ID { get; set; }
        public string Label { get; set; }
        [Column(TypeName = "decimal(6,2)")] public decimal Cost { get; set; }
        public bool CanCompete { get; set; }
        public bool CanPractice { get; set; }

        public ICollection<Registration> Registrations { get; set; }
    }
}