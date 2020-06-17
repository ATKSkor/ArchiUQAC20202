namespace StableAPI.Models
{
    public class Staff
    {
        public int StableID { get; set; }
        public int PersonID { get; set; }
        public int RoleID { get; set; }

        public Stable Stable { get; set; }
        public Person Person { get; set; }
        public Role Role { get; set; }
    }
}