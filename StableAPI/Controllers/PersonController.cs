using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StableAPI.Data;
using StableAPI.Models;
using StableAPI.Models.Dto;

namespace StableAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly StableContext _context;

        public PersonController(StableContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "secretary, admin")]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetPersons()
        {
            return await _context.Persons
                .Include(h => h.Horses)
                .Select(h => PersonToDo(h))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles ="secretary, admin")]
        public async Task<ActionResult<SinglePersonDto>> GetPerson(int id)
        {
            var person = await _context.Persons
                .Where(h => h.ID == id)
                .Include(h => h.Horses)
                .FirstOrDefaultAsync();

            if (person == null)
            {
                return NotFound();
            }

            return SinglePersonToDo(person);
        }

        [HttpPost]
        [Authorize(Roles = "secretary, admin")]
        public async Task<IActionResult> CreatePerson(PersonDto personDto)
        {
            if (personDto.Name == null || personDto.Surname == null)
            {
                return BadRequest();
            }

            var person = new Person
            {
                Name = personDto.Name,
                Surname = personDto.Surname
            };

            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerson), new { id = person.ID }, person);
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "secretary, admin")]
        public async Task<IActionResult> UpdatePerson(int id, SinglePersonDto personDto)
        {
            if (personDto.Name == null || personDto.Surname == null)
            {
                return BadRequest();
            }

            var person = await _context.Persons
                .FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            person.Name = personDto.Name;
            person.Surname = personDto.Surname;
            if (personDto.Horses != null)
            {
                var listHorse = new List<Horse>();
                foreach (var horse in personDto.Horses)
                {
                    var tmpHorse = new Horse
                    {
                        ID = horse.ID,
                        BoxID = horse.BoxID,
                        OwnerID = horse.OwnerID,
                        Owner = person,
                        Name = horse.Name,
                        MedicEntries = horse.MedicEntries
                    };

                    listHorse.Add(tmpHorse);
                }
                person.Horses = listHorse;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "secretary, admin")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.Persons
                .FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            if (person.Horses != null)
            {
                Console.WriteLine(person.Horses.Count);
                foreach (var horse in person.Horses)
                {
                    var tmpHorse = await _context.Horses
                    .FindAsync(horse.ID);

                    if (tmpHorse == null)
                    {
                        return NotFound();
                    }

                    _context.Horses.Remove(tmpHorse);
                }               
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return Ok();
        }


        private static PersonDto PersonToDo(Person person)
        {
            var ret = new PersonDto
            {
                ID = person.ID,
                Name = person.Name,
                Surname = person.Surname,
                HorsesIDs = new List<int>(),
                MembershipsIDs = new List<int>()
            };

            if (person.Horses != null) {
                foreach (var horse in person.Horses)
                {
                    ret.HorsesIDs.Add(horse.ID);
                }
            }
            if (person.Memberships != null)
            {
                foreach (var member in person.Memberships)
                {
                    ret.MembershipsIDs.Add(member.PersonID);
                }
            }

            return ret;
        }

        private static SinglePersonDto SinglePersonToDo(Person person)
        {
            var ret = new SinglePersonDto
            {
                ID = person.ID,
                Name = person.Name,
                Surname = person.Surname,
                Horses = new List<SingleHorseDto>()
            };

            foreach (var horse in person.Horses)
            {
                var tmpHorse = new SingleHorseDto
                {
                    ID = horse.ID,
                    BoxID = horse.BoxID,
                    OwnerID = horse.OwnerID,
                    OwnerFullName = new string(horse.Owner.Name + " " + horse.Owner.Surname),
                    Name = horse.Name,
                    MedicEntries = horse.MedicEntries
                };
                ret.Horses.Add(tmpHorse);
            }
            return ret;
        }
    }
}
