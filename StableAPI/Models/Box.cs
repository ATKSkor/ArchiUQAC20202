using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StableAPI.Models
{
    public class Box
    {
        public int ID { get; set; }
        [Required] public bool Active { get; set; }
        [Required] public int StableID { get; set; }

        public Stable Stable { get; set; }
        public virtual Horse Occupant { get; set; }
    }
}