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
                //.Include(e => e.RSVPs)
                .ToListAsync();

            var user = await GetUserAsync();
            //Set userid to a string to avoid null being passed into viewbag
            if (user == null)
            {
                ViewBag.UserId = "not_logged_in";
            }
            else
            {
                ViewBag.UserId = user.Id;
                var attendingList = await _context.RSVP.Where(e => e.attendeeId == user.Id).Select(e => e.eventId).ToListAsync();
                //var attendingList = await _context.RSVP.Where(e => e.attendeeId == user.Id).ToListAsync();
                ViewBag.AttendingList = attendingList;
            }


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
            var user = await GetUserAsync();
            //Set userid to a string to avoid null being passed into viewbag
            if (user == null)
            {
                ViewBag.UserId = "not_logged_in";
            }
            else
            {
                ViewBag.UserId = user.Id;
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
            //Remove hostId, get ID of logged in user, add it to the Event obj
            ModelState.Remove("hostId");
            var user = await GetUserAsync();
            @event.hostId = user.Id;

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //If the id isnt found, return NOT FOUND
            if (id == null)
            {
                return NotFound();
            }
            
            //Get the eent from the DB and store it in @event
            var @event = await _context.Event.FindAsync(id);
            //If the id exists but the event is null, return NOT FOUND
            if (@event == null)
            {
                return NotFound();
            }
            //Get the information about the currently logged in user
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user.Id == @event.hostId)
            {
                //Add a list of venues from the DB to ViewData for drop-down access
                var venues = await _context.Venue.ToListAsync();
                ViewData["Venues"] = new SelectList(_context.Venue, "id", "venueName");
                return View(@event);
            }
            //return View(@event);            
            return NotFound();
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
            //Remove hostId, get ID of logged in user, add it to the Event obj
            ModelState.Remove("hostId");
            var user = await GetUserAsync();
            @event.hostId = user.Id;

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
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user.Id == @event.hostId)
            {
                return View(@event);
            }

            return NotFound();

                
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
