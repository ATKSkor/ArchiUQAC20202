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
    public class HorseController : ControllerBase
    {
        private readonly StableContext _context;

        public HorseController(StableContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "groom, secretary, admin")]
        public async Task<ActionResult<IEnumerable<HorseDto>>> GetHorses()
        {
            return await _context.Horses
                .Include(h => h.Owner)
                .Include(h => h.Boxes)
                .Include(h => h.MedicEntries)
                .Select(h => HorseToDto(h))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "groom, secretary, admin")]
        public async Task<ActionResult<SingleHorseDto>> GetHorse(int id)
        {
            var horse = await _context.Horses
                .Where(h => h.ID == id)
                .Include(h => h.Owner)
                .Include(h => h.Boxes)
                .Include(h => h.MedicEntries)
                .FirstOrDefaultAsync();

            if (horse == null)
            {
                return NotFound();
            }

            return HorseToSingleDto(horse);
        }

        [HttpPost]
        [Authorize(Roles = "groom, admin")]
        public async Task<IActionResult> CreateHorse(HorseDto horseDto)
        {
            if (horseDto.Name == null || horseDto.OwnerID <= 0)
            {
                return BadRequest();
            }

            var horse = new Horse
            {
                Name = horseDto.Name,
                OwnerID = horseDto.OwnerID
            };

            await _context.Horses.AddAsync(horse);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHorse), new {id = horse.ID}, horse);
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "groom, admin")]
        public async Task<IActionResult> UpdateHorse(int id, HorseDto horseDto)
        {
            if (horseDto.Name == null || horseDto.OwnerID <= 0)
            {
                return BadRequest();
            }

            var horse = await _context.Horses
                .FindAsync(id);

            if (horse == null)
            {
                return NotFound();
            }

            horse.Name = horseDto.Name;
            horse.OwnerID = horseDto.OwnerID;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "groom, admin")]
        public async Task<IActionResult> DeleteHorse(int id)
        {
            var horse = await _context.Horses
                .FindAsync(id);

            if (horse == null)
            {
                return NotFound();
            }

            _context.Horses.Remove(horse);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private static HorseDto HorseToDto(Horse horse)
        {
            var ret = new HorseDto
            {
                ID = horse.ID,
                OwnerID = horse.OwnerID,
                OwnerFullName = new string(horse.Owner.Name + " " + horse.Owner.Surname),
                Name = horse.Name,
                MedicEntryIDs = new List<int>()
            };

            if (horse.Boxes != null && horse.Boxes.Any())
            {
                ret.BoxID = horse.Boxes.First().ID;
            }

            if (horse.MedicEntries == null) return ret;
            foreach (var me in horse.MedicEntries)
            {
                ret.MedicEntryIDs.Add(me.ID);
            }

            return ret;
        }

        private static SingleHorseDto HorseToSingleDto(Horse horse)
        {
            var ret = new SingleHorseDto
            {
                ID = horse.ID,
                OwnerID = horse.OwnerID,
                OwnerFullName = new string(horse.Owner.Name + " " + horse.Owner.Surname),
                Name = horse.Name,
                MedicEntries = horse.MedicEntries
            };

            if (horse.Boxes != null && horse.Boxes.Any())
            {
                ret.BoxID = horse.Boxes.First().ID;
            }

            return ret;
        }
    }
}