using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LocalShowsOnly.Data;
using LocalShowsOnly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using LocalShowsOnly.Models.ViewModels;

namespace LocalShowsOnly.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public EventsController(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var list = await _context.Event
                .Include(e => e.venue)
                .ToListAsync();

            return View(list);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create()
        {
            var venues = await _context.Venue.ToListAsync();
            ViewData["Venues"] = new SelectList(_context.Venue, "id", "venueName");

            //List<SelectListItem> venueList = _context.Venue.ToList();
            //ViewData["Venue"] = venueList;

            //var viewModel = new EventVenueViewModel();
            //viewModel.VenuesSelectListItem = new SelectList(_context.Venue, "id", "venueName");
            //return View(viewModel);
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,hostId,title,venueId,showtime,externalLink,photoURL")] Event @event)
        {
            //ModelState.Remove("UserId");
            ModelState.Remove("hostId");
            var user = await GetUserAsync();
            @event.hostId = user.Id;

            if (ModelState.IsValid)
            {
                
                //@event.hostId = Int32.Parse(user.Id);
                
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,hostId,title,venueId,showtime,externalLink,photoURL")] Event @event)
        {
            if (id != @event.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.id == id);
        }
        private Task<ApplicationUser> GetUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        //public async List<Venue> GetAllVenues()
        //{
        //    List<Venue> venues = await _context.Venue.ToListAsync();
        //    return View(venues);
        //}
    }
    
}
