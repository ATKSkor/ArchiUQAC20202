using System.ComponentModel.DataAnnotations.Schema;

namespace StableAPI.Models
{
    public class User : Person
    {
        public string UserName { get; set; }
        public string PassHash { get; set; }
        public int RoleID { get; set; }

        [ForeignKey("RoleID")] public Role Role { get; set; }
    }
}