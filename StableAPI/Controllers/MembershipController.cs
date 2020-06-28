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
    public class MembershipController : ControllerBase
    {
        private readonly StableContext _context;

        public MembershipController(StableContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "secretary, admin")]
        public async Task<ActionResult<IEnumerable<MembershipDto>>> GetMembers()
        {
            return await _context.Memberships
                .Include(h => h.Person)
                .Include(h => h.Bills)
                .Select(h => MemberToDo(h))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "secretary, admin")]
        public async Task<ActionResult<SingleMembershipDto>> GetMember(int id)
        {
            var person = await _context.Memberships
                .Where(h => h.PersonID == id)
                .Include(h => h.Person)
                .Include(h => h.Bills)
                .FirstOrDefaultAsync();

            if (person == null)
            {
                return NotFound();
            }

            return SingleMemberToDo(person);
        }

        [HttpPost]
        [Authorize(Roles = "secretary, admin")]
        public async Task<IActionResult> CreateMember(MembershipDto memberDto)
        {
            if (memberDto.PersonID == null || memberDto.StableID == null)
            {
                return BadRequest();
            }

            var person = new Membership
            {
                Person = memberDto.Person,
                Stable = memberDto.Stable,
                StableID = (int) memberDto.StableID,
            };

            await _context.Memberships.AddAsync(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMember), new { id = person.PersonID }, person);
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "secretary, admin")]
        public async Task<IActionResult> UpdateMember(int id, SingleMembershipDto memberDto)
        {
            if (memberDto.PersonID == null || memberDto.StableID == null)
            {
                return BadRequest();
            }

            var person = await _context.Memberships
                .FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            person.Person = memberDto.Person;
            person.Stable = memberDto.Stable;
            person.StableID = (int) memberDto.StableID;
            person.Bills = memberDto.Bills; 

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "secretary, admin")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var person = await _context.Memberships
                .FindAsync(1, id);

            if (person == null)
            {
                return NotFound();
            }

            _context.Memberships.Remove(person);
            await _context.SaveChangesAsync();

            return Ok();
        }


        private static MembershipDto MemberToDo(Membership person)
        {
            var ret = new MembershipDto
            {
                PersonID = person.PersonID,
                Person = person.Person,
                Stable = person.Stable,
                StableID = person.StableID,
                BillsID = new List<int>(),
                RegistrationsID = new List<int>()
            };

            if (person.Bills != null)
            {
                foreach (var bill in person.Bills)
                {
                    ret.BillsID.Add(bill.ID);
                }
            }
            if (person.Registrations != null)
            {
                foreach (var register in person.Registrations)
                {
                    ret.RegistrationsID.Add(register.PersonID);
                }
            }

            return ret;
        }

        private static SingleMembershipDto SingleMemberToDo(Membership person)
        {
            var ret = new SingleMembershipDto
            {
                PersonID = person.PersonID,
                Person = person.Person,
                Stable = person.Stable,
                Bills = person.Bills,
                StableID = person.StableID
            };

            return ret;
        }
    }
}
