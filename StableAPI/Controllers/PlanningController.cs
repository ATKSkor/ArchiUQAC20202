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
    public class PlanningController : ControllerBase
    {
        private StableContext _context;

        public PlanningController(StableContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin, groom, secretary")]
        public async Task<ActionResult<ICollection<Calendar>>> GetCalendars()
        {
            return await _context.Calendars
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin, groom, secretary")]
        public async Task<ActionResult<CalendarDto>> GetCalendar(int id)
        {
            var calendar = await _context.Calendars
                .Include(c => c.Events)
                    .ThenInclude(e => e.EventType)
                .Where(c => c.ID == id)
                .FirstOrDefaultAsync();

            if (calendar == null)
            {
                return BadRequest("No such Calendar");
            }

            return ToCalendarDto(calendar);
        }

        [HttpPost]
        [Authorize(Roles = "admin, secretary")]
        public async Task<IActionResult> CreateCalendar(Calendar calendar)
        {
            var stable = await _context.Stables
                .FindAsync(calendar.StableID);

            if (stable == null)
            {
                return BadRequest("No such Stable");
            }

            await _context.Calendars.AddAsync(calendar);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, secretary")]
        public async Task<IActionResult> DeleteCalendar(int id)
        {
            var calendar = await _context.Calendars
                .FindAsync(id);

            if (calendar == null)
            {
                return BadRequest("No such Calendar");
            }

            _context.Calendars.Remove(calendar);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public static CalendarDto ToCalendarDto(Calendar calendar)
        {
            return new CalendarDto
            {
                ID = calendar.ID,
                Name = calendar.Name,
                StableID = calendar.StableID,
                Events = calendar.Events.Select(ToEventDto).ToList()
            };
        }

        public static EventDto ToEventDto(Event e)
        {
            return new EventDto
            {
                ID = e.ID,
                Title = e.Title,
                EventType = new EventType
                {
                    ID = e.EventType.ID,
                    Label = e.EventType.Label
                },
                StartDate = e.StartDate,
                EndDate = e.EndDate
            };
        }
    }
}