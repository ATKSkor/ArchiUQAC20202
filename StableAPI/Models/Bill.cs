using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StableAPI.Models
{
    public class Bill
    {
        public int ID { get; set; }
        [Required] [Column(TypeName = "decimal(6,2)")] public decimal Amount { get; set; }
        [Required] public int StableID { get; set; }
        [Required] public int PersonID { get; set; }
        public int MembershipID { get; set; }

        public Stable Stable { get; set; }
        public Person Person { get; set; }
        public Membership Membership { get; set; }
    }
}
