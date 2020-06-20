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
        public async Task<ActionResult<SingleHorseDto>> GetHorseById(int id)
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