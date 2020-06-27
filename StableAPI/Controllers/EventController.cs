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
    [Authorize(Roles = "admin, secretary, groom")]
    public class EventController : ControllerBase
    {
        private StableContext _context;

        public EventController(StableContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            var e = await _context.Events
                .Where(e => e.ID == id)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync();

            if (e == null)
            {
                return BadRequest("No such Event");
            }

            return PlanningController.ToEventDto(e);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
        {
            return await _context.Events
                .Include(e => e.EventType)
                .Select(e => PlanningController.ToEventDto(e))
                .ToListAsync();
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<EventType>>> GetTypes()
        {
            return await _context.EventTypes
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "admin, secretary")]
        public async Task<IActionResult> CreateEvent(Event newEvent)
        {
            var calendar = await _context.Calendars.FindAsync(newEvent.CalendarID);

            if (calendar == null)
            {
                return BadRequest("No such Calendar");
            }

            if (newEvent.StartDate > newEvent.EndDate)
            {
                return BadRequest("Event cannot end before it started");
            }

            if (newEvent.EventTypeID == 0 && newEvent.EventType.ID != 0)
            {
                newEvent.EventTypeID = newEvent.EventType.ID;
            } else if (newEvent.EventTypeID == 0 && newEvent.EventType.ID == 0 && newEvent.EventType.Label != "")
            {
                await _context.EventTypes.AddAsync(newEvent.EventType);
                await _context.SaveChangesAsync();
            }

            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "admin, secretary")]
        public async Task<IActionResult> UpdateEvent(int id, Event e)
        {
            var old = await _context.Events
                .FindAsync(id);

            old.StartDate = e.StartDate;
            old.EndDate = e.EndDate;
            old.EventType = await _context.EventTypes.FindAsync(e.EventTypeID);
            old.EventTypeID = e.EventTypeID;
            old.Title = e.Title;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, secretary")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var e = await _context.Events
                .FindAsync(id);

            _context.Events.Remove(e);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("types/{id}")]
        [Authorize(Roles = "admin, secretary")]
        public async Task<IActionResult> DeleteEventType(int id)
        {
            var t = await _context.EventTypes
                .FindAsync(id);

            _context.EventTypes.Remove(t);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }


}