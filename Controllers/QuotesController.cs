using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : Controller
    {
        private readonly LibraryContext _context;

        public QuotesController(LibraryContext context)
        {
            _context = context;

            _context.Database.EnsureCreated();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quote>>> GetAllQuotes()
        {
            var quotes = await _context.Quotes.ToArrayAsync();
            if (quotes == null || quotes.Length == 0)
            {
                return NotFound();
            }
            return Ok(quotes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return NotFound();
            }
            return Ok(quote);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddQuote(Quote quote)
        {
            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetQuote", new { id = quote.Id }, quote);
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return NotFound();
            }
            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();
            return Ok(quote);
        }
    }

}
