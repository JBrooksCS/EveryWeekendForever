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

namespace LocalShowsOnly.Controllers
{
    public class BandsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;
        public BandsController(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _userManager = userManager;
            _context = ctx;
            _env = env;
        }

        // GET: Bands
        public async Task<IActionResult> Index()
        {
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
            return View(await _context.Band.ToListAsync());
        }

        // GET: Bands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var band = await _context.Band
                .FirstOrDefaultAsync(m => m.id == id);
            if (band == null)
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

            return View(band);
        }
        // GET: Bands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,bandName,bio,externalLink,genre,photoURL,isActive")] Band band, IFormFile file)
        {
            //Remove hostId, get ID of logged in user, add it to the Event obj
            ModelState.Remove("hostId");
            var user = await GetUserAsync();
            band.hostId = user.Id;
            band.isActive = true;
            if (ModelState.IsValid)
            {
                try
                {
                    band.photoURL = await SaveFile(file, user.Id);
                }
                catch (Exception ex)
                {
                    return NotFound();
                }

                _context.Add(band);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(band);
        }
        [Authorize]
        // GET: Bands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var band = await _context.Band.FindAsync(id);
            if (band == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            //if (user == null)
            //{
            //    return NotFound
            //}
            if (user != null && user.Id == band.hostId)
            {
                return View(band);
            }
            return NotFound();
        }

        // POST: Bands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("id,bandName,bio,externalLink,genre,photoURL,isActive")] Band band, IFormFile file)
        {
            if (id != band.id)
            {
                return NotFound();
            }
            ModelState.Remove("hostId");
            var user = await GetUserAsync();
            band.hostId = user.Id;
            bool newFilePresent = true;
            if (file == null)
            {
                var @oldBand = await _context.Band.FindAsync(id);
                band.photoURL = oldBand.photoURL;
                newFilePresent = false;
            }

            if (ModelState.IsValid)
            {
                if (newFilePresent)
                {
                    try
                    {
                        band.photoURL = await SaveFile(file, user.Id);
                    }
                    catch (Exception ex)
                    {
                        return NotFound();
                    }
                }
                try
                {
                    _context.Update(band);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BandExists(band.id))
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
            return View(band);
        }

        // GET: Bands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var band = await _context.Band
                .FirstOrDefaultAsync(m => m.id == id);
            if (band == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user.Id == band.hostId)
            {
                return View(band);
            }
            return NotFound();
        }

        // POST: Bands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var band = await _context.Band.FindAsync(id);
            _context.Band.Remove(band);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BandExists(int id)
        {
            return _context.Band.Any(e => e.id == id);
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
