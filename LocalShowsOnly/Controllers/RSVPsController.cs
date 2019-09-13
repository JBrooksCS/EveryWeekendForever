using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LocalShowsOnly.Data;
using LocalShowsOnly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace LocalShowsOnly.Controllers
{
    public class RSVPsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public RSVPsController(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }

        // GET: RSVPs
        public async Task<IActionResult> Index()
        {
            return View(await _context.RSVP.ToListAsync());
        }

        // GET: RSVPs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rSVP = await _context.RSVP
                .FirstOrDefaultAsync(m => m.id == id);
            if (rSVP == null)
            {
                return NotFound();
            }

            return View(rSVP);
        }

        // GET: RSVPs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RSVPs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,eventId,attendeeId,reviewText")] RSVP rSVP)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rSVP);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rSVP);
        }

        // GET: RSVPs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rSVP = await _context.RSVP.FindAsync(id);
            if (rSVP == null)
            {
                return NotFound();
            }
            return View(rSVP);
        }

        // POST: RSVPs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,eventId,attendeeId,reviewText")] RSVP rSVP)
        {
            if (id != rSVP.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rSVP);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RSVPExists(rSVP.id))
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
            return View(rSVP);
        }
        //POST RSVPs/AddAttendee/5
        [Authorize]
        [HttpPost, ActionName("AddAttendee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAttendee(int showId)
        {
            var user = await GetUserAsync();
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            //This is just a check that the user isn't already attending - shouldnt be possible
            else if (UserIsAttending(showId, user.Id))
            {
                return RedirectToAction("Index", "Events");
            }
            else
            {
                var newRSVP = new RSVP() {
                    eventId = showId,
                    attendeeId = user.Id,
                    reviewText = null
                };
                _context.Add(newRSVP);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Events");
        }
        //GET RSVPs/AddAttendee/5
        [HttpGet]
        public IActionResult AddAttendee()
        {
            return RedirectToAction("Index", "Events");
        }
        //POST RSVPs/RemoveAttendee/5
        [HttpPost, ActionName("RemoveAttendee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAttendee(int showId)
        {
            var user = await GetUserAsync();
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            //This is just a check that the user is in fact attending 
            else if (!UserIsAttending(showId, user.Id))
            {
                return RedirectToAction("Index", "Events");
            }
            else
            {
                var rSVP = await _context.RSVP.FirstOrDefaultAsync(m => m.attendeeId == user.Id && m.eventId == showId);

                if (rSVP == null)
                {
                    return NotFound();
                }
                _context.Remove(rSVP);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Events");
        }


        // GET: RSVPs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rSVP = await _context.RSVP
                .FirstOrDefaultAsync(m => m.id == id);
            if (rSVP == null)
            {
                return NotFound();
            }

            return View(rSVP);
        }

        // POST: RSVPs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rSVP = await _context.RSVP.FindAsync(id);
            _context.RSVP.Remove(rSVP);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RSVPExists(int id)
        {
            return _context.RSVP.Any(e => e.id == id);
        }
        private bool UserIsAttending(int show_id, string attendee_id)
        {
            return _context.RSVP.Any(e => e.eventId == show_id && e.attendeeId == attendee_id);
        }
        private Task<ApplicationUser> GetUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
