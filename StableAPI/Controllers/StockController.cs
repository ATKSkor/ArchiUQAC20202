using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    [Authorize(Roles = "admin, secretary")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private StableContext _context;

        public StockController(StableContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<StockEntryDto>>> GetAllEntries()
        {
            return await _context.StockEntries
                .Include(se => se.Item)
                .Select(se => ToStockEntryDto(se))
                .ToListAsync();
        }

        [HttpGet("{stableId}")]
        public async Task<ActionResult<ICollection<StockEntryDto>>> GetStableItems(int stableId)
        {
            return await _context.StockEntries
                .Include(se => se.Item)
                .Where(se => se.StableID == stableId)
                .Select(se => ToStockEntryDto(se))
                .ToListAsync();
        }

        [HttpGet("{stableId}/{itemId}")]
        public async Task<ActionResult<SingleStockEntryDto>> GetStockEntry(int stableId, int itemId)
        {
            var stockEntry = await _context.StockEntries
                .Include(se => se.Item)
                .Include(se => se.Reservations)
                .Where(se => se.StableID == stableId
                             && se.ItemID == itemId)
                .Select(se => ToSingleStockEntryDto(se))
                .FirstOrDefaultAsync();

            if (stockEntry == null)
            {
                return BadRequest("No such entry");
            }

            return stockEntry;
        }

        [HttpGet("items")]
        public async Task<ActionResult<IEnumerable<StockItemDto>>> GetItemTypes()
        {
            var stockItem = await _context.StockItems
                .Select(si => ToStockItemDto(si))
                .ToListAsync();

            return stockItem;
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteItemType(int id)
        {
            var stockItem = await _context.StockItems.FindAsync(id);

            if (stockItem == null)
            {
                return NotFound();
            }

            _context.StockItems.Remove(stockItem);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{stableId}/{itemId}")]
        public async Task<IActionResult> UpdateStockEntry(int stableId, int itemId, StockEntryDto stockEntryDto)
        {
            if (stockEntryDto.Quantity < 0)
            {
                return BadRequest();
            }

            var stockEntry = await _context.StockEntries.FindAsync(stableId, itemId);

            if (stockEntry == null)
            {
                return NotFound();
            }

            stockEntry.Quantity = stockEntryDto.Quantity;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStockEntry(StockEntry stockEntry)
        {
            if (stockEntry.Stable != null)
            {
                return BadRequest("Won't add a new stable here");
            }

            if (stockEntry.ItemID > 0)
            {
                var item = await _context.StockItems
                    .FindAsync(stockEntry.ItemID);
                if (item == null)
                {
                    return BadRequest("No such item");
                }

                var existing = await _context.StockEntries
                    .FindAsync(stockEntry.StableID, stockEntry.ItemID);

                if (existing != null)
                {
                    return BadRequest("There already exists such an entry");
                }
            }
            else if(stockEntry.Item.ItemName == null || stockEntry.Item.ItemName == "" )
            {
                 return BadRequest("Empty item name not allowed");
            }

            await _context.StockEntries.AddAsync(stockEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStockEntry), 
                new {stableId = stockEntry.StableID, itemId = stockEntry.ItemID},
                ToStockEntryDto(stockEntry));
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveItem(Reservation reservation)
        {
            var stockEntry = await _context.StockEntries
                .Include(se => se.Item)
                .Where(se => se.StableID == reservation.StockEntryStableID
                             && se.ItemID == reservation.StockEntryItemID)
                .FirstOrDefaultAsync();

            if (stockEntry == null)
            {
                return BadRequest("No such item in this stable");
            }

            if (reservation.Quantity <= 0)
            {
                return BadRequest("Quantity must be a positive number");
            }

            if (reservation.EndDate < DateTime.Now)
            {
                return BadRequest("Cannot end reservation before now");
            }

            if (!CheckReservationValidity(reservation))
            {
                return BadRequest("Item is not available in the desired quantity for the given period");
            }

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("reserve/{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations
                .FindAsync(id);

            if (reservation == null)
            {
                return BadRequest("No such reservation");
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{stableId}/{itemId}")]
        public async Task<IActionResult> DeleteStockEntry(int stableId, int itemId)
        {
            var stockEntry = await _context.StockEntries
                .FindAsync(stableId, itemId);

            if (stockEntry == null)
            {
                return BadRequest("No such entry");
            }

            _context.StockEntries
                .Remove(stockEntry);

            await _context.SaveChangesAsync();

            return Ok();
        }

        private static StockEntryDto ToStockEntryDto(StockEntry stockEntry)
        {
            return new StockEntryDto
            {
                StableId = stockEntry.StableID,
                StockItemId = stockEntry.ItemID,
                ItemName = stockEntry.Item.ItemName,
                Quantity = stockEntry.Quantity
            };
        }

        private static StockItemDto ToStockItemDto(StockItem stockItem)
        {
            return new StockItemDto
            {
                ID = stockItem.ID,
                ItemName = stockItem.ItemName
            };
        }

        private static SingleStockEntryDto ToSingleStockEntryDto(StockEntry stockEntry)
        {
            var dto = new SingleStockEntryDto
            {
                StockItemId = stockEntry.ItemID,
                StableId = stockEntry.StableID,
                ItemName = stockEntry.Item.ItemName,
                Quantity = stockEntry.Quantity,
                Reservations = new List<SingleStockEntryDto.ReservationDto>()
            };

            if (stockEntry.Reservations == null) return dto;

            foreach (var reservation in stockEntry.Reservations)
            {
                dto.Reservations.Add(new SingleStockEntryDto.ReservationDto
                {
                    ID = reservation.ID,
                    Quantity = reservation.Quantity,
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate
                });
            }

            return dto;
        }

        private class StockEventTimeStamp
        {
            public DateTime Time { get; set; }
            public int QuantityChange { get; set; }
        }

        private bool CheckReservationValidity(Reservation reservation)
        {
            var existingReservations = _context.Reservations
                .Where(r => r.EndDate > reservation.StartDate
                            && r.StartDate < reservation.EndDate)
                .ToList();

            var timeline = new List<StockEventTimeStamp>();
            var count = _context.StockEntries.Find(reservation.StockEntryStableID, reservation.StockEntryItemID).Quantity;

            foreach (var existing in existingReservations)
            {
                if (existing.StartDate > reservation.StartDate)
                {
                    timeline.Add(new StockEventTimeStamp {QuantityChange = -existing.Quantity, Time = existing.StartDate});
                }
                else
                {
                    count -= existing.Quantity;
                }

                if (existing.EndDate < reservation.EndDate)
                {
                    timeline.Add(new StockEventTimeStamp {QuantityChange = existing.Quantity, Time = existing.EndDate});
                }
            }

            if (count < reservation.Quantity)
            {
                return false;
            }

            foreach (var timeStamp in timeline.OrderBy(ts => ts.Time))
            {
                count += timeStamp.QuantityChange;
                if (count < reservation.Quantity)
                {
                    return false;
                }
            }

            return true;
        }
    }
}