using System.Linq;
using StableAPI.Models;

namespace StableAPI.Data
{
    public class DbInitializer
    {
        public static void Initialize(StableContext context)
        {
            context.Database.EnsureCreated();

            if (context.Stables.Any())
            {
                return;   // DB has been seeded
            }

            var stables = new Stable[]
            {
                new Stable()
            };
            foreach (var stable in stables)
            {
                context.Stables.Add(stable);
            }

            context.SaveChanges();
        }
    }
}