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
using System.IO;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Hosting;
using PagedList;

namespace LocalShowsOnly.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;
        public EventsController(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            //This next line was added to prevent tracking issues when no image is selected for Event Edit Post
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _userManager = userManager;
            _context = ctx;
            _env = env;
        }

        // GET: Events
        public async Task<IActionResult> Index(string searchString)
        {
            

            var list = await _context.Event
                .Include(e => e.venue)
                .Include(e => e.RSVPs)
                .OrderBy(e => e.showtime)
                .ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(s => s.title.ToLower().Contains(searchString.ToLower())).ToList();
            }

            var TopAttended = list.OrderByDescending(e => e.RSVPs.Count).Take(1);
            ViewBag.top = TopAttended.ElementAt(0).id;

            var user = await GetUserAsync();
            //Set userid to a string to avoid null being passed into viewbag
            if (user == null)
            {
                ViewBag.UserId = "not_logged_in";
                
                var attendingList = new List<RSVP>();
            }
            else
            {
                ViewBag.UserId = user.Id; 
                var attendingList = await _context.RSVP.Where(e => e.attendeeId == user.Id).Select(e => e.eventId).ToListAsync();
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
        [Authorize]
        // GET: Events/Create
        public async Task<IActionResult> Create()
        {
            var venues = await _context.Venue.ToListAsync();
            ViewData["Venues"] = new SelectList(_context.Venue, "id", "venueName");

          
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("id,hostId,title,venueId,showtime,externalLink,price, description,photoURL")] Event @event, IFormFile file)
        {
            //Remove hostId, get ID of logged in user, add it to the Event obj
            ModelState.Remove("hostId");
            var user = await GetUserAsync();
            @event.hostId = user.Id;

            if (ModelState.IsValid)
            {
                try
                {
                    @event.photoURL = await SaveFile(file, user.Id);
                }
                catch (Exception ex)
                {
                    return NotFound();
                }

                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }
        [Authorize]
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
            //if (user == null)
            //{
            //    return NotFound
            //}
            if (user != null && user.Id == @event.hostId)
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
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("id,hostId,title,venueId,showtime,description,price,externalLink,photoURL")] Event @event, IFormFile file)
        {
            if (id != @event.id)
            {
                return NotFound();
            }
            //Remove hostId, get ID of logged in user, add it to the Event obj
            ModelState.Remove("hostId");
            var user = await GetUserAsync();
            @event.hostId = user.Id;
            bool newFilePresent = true;
            if(file == null)
            {
                var @oldEvent = await _context.Event.FindAsync(id);
                @event.photoURL = oldEvent.photoURL;
                newFilePresent = false;
            }

            if (ModelState.IsValid)
            {
                if (newFilePresent)
                {
                    try
                    {
                        @event.photoURL = await SaveFile(file, user.Id);
                    }
                    catch (Exception ex)
                    {
                        return NotFound();
                    }
                }

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
        [Authorize]
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
        [Authorize]
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
        private async Task<string> SaveFile(IFormFile file, string userId)
        {
            if (file.Length > 5242880) throw new Exception("File too large!");
            var ext = GetMimeType(file.FileName);
            if (ext == null) throw new Exception("Invalid file type");

            var epoch = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            var fileName = $"{epoch}-{userId}.{ext}";
            var webRoot = _env.WebRootPath;
            var absoluteFilePath = Path.Combine(
                webRoot,
                "images",
                fileName);
            string relFilePath = null;
            if (file.Length > 0)
            {
                using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    relFilePath = $"~/images/{fileName}";
                };
            }


            return relFilePath;
        }
        private string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            provider.TryGetContentType(fileName, out contentType);
            if (contentType == "image/jpeg") contentType = "jpg";
            else contentType = null;

            return contentType;
        }
    }
    
}
