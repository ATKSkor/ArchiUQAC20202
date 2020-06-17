namespace StableAPI.Models
{
    public class Registration
    {
        public int ID { get; set; }
        public bool Validated { get; set; }
        public int StableID { get; set; }
        public int PersonID { get; set; }
        public int LicenseID { get; set; }

        public Stable Stable { get; set; }
        public Membership Membership { get; set; }
        public License License { get; set; }
    }
}