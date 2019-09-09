using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LocalShowsOnly.Data;
using LocalShowsOnly.Models;

namespace LocalShowsOnly.Controllers
{
    public class RSVPsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RSVPsController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create([Bind("id,eventId,attendeeId")] RSVP rSVP)
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
        public async Task<IActionResult> Edit(int id, [Bind("id,eventId,attendeeId")] RSVP rSVP)
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
    }
}
