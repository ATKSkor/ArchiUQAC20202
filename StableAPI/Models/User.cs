namespace StableAPI.Models
{
    public class User : Person
    {
        public string UserName { get; set; }
        public string PassHash { get; set; }
    }
}