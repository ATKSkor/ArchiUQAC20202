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

namespace StableAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MedicController : ControllerBase
    {
        private StableContext _context;

        public MedicController(StableContext context)
        {
            _context = context;
        }

        // GET: Medic/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, groom, secretary")]
        public async Task<ActionResult<MedicEntry>> GetMedicEntry(int id)
        {
            var report = await _context.MedicEntries
                .Where(me => me.ID == id)
                .Include(me => me.Report)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                return NotFound();
            }

            return report;
        }

        // POST: api/Medic
        [HttpPost]
        [Authorize(Roles = "admin, groom")]
        public async Task<ActionResult<MedicEntry>> CreateMedicEntry(MedicEntry medicEntry)
        {
            if (medicEntry.HorseID <= 0)
            {
                return BadRequest();
            }

            var horse = await _context.Horses
                .FindAsync(medicEntry.HorseID);

            if (horse == null)
            {
                return NotFound("No such horse: id " + medicEntry.HorseID);
            }

            _context.MedicEntries.Add(medicEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicEntry), 
                new {id = medicEntry.ID}, 
                medicEntry);
        }

        // POST: Medic/5
        [HttpPost("{id}")]
        [Authorize(Roles = "admin, groom")]
        public async Task<IActionResult> UpdateMedicEntry(int id, MedicEntry newEntry)
        {
            var medicEntry = await _context.MedicEntries
                .FindAsync(id);

            if (medicEntry == null)
            {
                return NotFound();
            }

            if (newEntry.Report != null)
            {
                _context.MedicReports.Remove(medicEntry.Report);
            }

            medicEntry.Title = newEntry.Title;
            medicEntry.HorseID = newEntry.HorseID;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: Medic/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, groom")]
        public async Task<IActionResult> DeleteMedicEntry(int id)
        {
            var medicEntry = await _context.MedicEntries
                .Where(me => me.ID == id)
                .Include(me => me.Report)
                .FirstOrDefaultAsync();

            if (medicEntry == null)
            {
                return NotFound("No such horse: id " + id);
            }

            _context.MedicEntries
                .Remove(medicEntry);
            await _context.SaveChangesAsync();

            var rep = medicEntry.Report;

            _context.MedicReports
                .Remove(rep);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
