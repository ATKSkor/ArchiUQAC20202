using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StableAPI.Data;
using StableAPI.Models;

namespace StableAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoxController : ControllerBase
    {
        private StableContext _context;

        public BoxController(StableContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Box>>> GetBoxes()
        {
            return await _context.Boxes
                .ToListAsync();
        }
    }
}