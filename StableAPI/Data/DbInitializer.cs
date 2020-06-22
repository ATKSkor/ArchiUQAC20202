using System;
using System.IO;
using System.Linq;
using StableAPI.Models;

namespace StableAPI.Data
{
    public class DbInitializer
    {
        public static void Initialize(StableContext context)
        {
            context.Database.EnsureCreated();

            if (context.Persons.Any())
            {
                return;   // DB has been seeded
            }

            var roles = new Role[]
            {
                new Role{Name = "default"},
                new Role{Name = "admin"},
                new Role{Name = "groom"},
                new Role{Name = "secretary"},
            };
            foreach (var role in roles)
            {
                context.Roles.Add(role);
            }

            context.SaveChanges();

            var persons = new Person[]
            {
                new Person {Name = "Doe", Surname = "John"},
                new Person {Name = "Martin", Surname = "Pierre"},
                new Person {Name = "Muller", Surname = "Hans"},
                new Person {Name = "West", Surname = "Malik"},
            };
            foreach (var person in persons)
            {
                context.Persons.Add(person);
            }

            context.SaveChanges();

            var users = new User[]
            {
                new User {Name = "Boss", Surname = "E", UserName = "admin", PassHash = "admin", RoleID = 2},
                new User {Name = "Secretary", Surname = "F", UserName = "secretary", PassHash = "secretary", RoleID = 4},
                new User {Name = "Groom", Surname = "G", UserName = "groom", PassHash = "groom", RoleID = 3},
                new User {Name = "Doe", Surname = "John", UserName = "jdoe", PassHash = "password", RoleID = 1}
            };
            foreach (var user in users)
            {
                context.Users.Add(user);
            }

            context.SaveChanges();

            var stables = new Stable[]
            {
                new Stable{BossID = 5},
            };
            foreach (var stable in stables)
            {
                context.Stables.Add(stable);
            }

            context.SaveChanges();

            var horses = new Horse[]
            {
                new Horse{OwnerID = 1, Name = "Pataclop"},
                new Horse{Owner = new Person{Name = "Owner", Surname = "StarPony"}, Name = "StarPony"}
            };
            foreach (var horse in horses)
            {
                context.Horses.Add(horse);
            }

            context.SaveChanges();

            AddMedicEntry(context);
        }

        private static void AddMedicEntry(StableContext context)
        {
            var doc = File.ReadAllBytes("./Resources/dummy.pdf");

            var medicEntry = new MedicEntry
            {
                Report = new MedicReport{Document = doc},
                HorseID = 1,
                IssuedDate = DateTime.Today,
                Title = "Very Important Report"
            };

            context.MedicEntries.Add(medicEntry);
            context.SaveChanges();
        }
    }
}