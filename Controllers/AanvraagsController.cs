using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationRequestIt.Data;
using ApplicationRequestIt.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ApplicationRequestIt.Utility;
using ApplicationRequestIt.Models.AanvraagsBerichtenViewmodel;

namespace ApplicationRequestIt.Controllers
{
    [Authorize]
    public class AanvraagsController : Controller
    {

        public bool IsIndexallaanvragen;
        public bool IsIndex;
        public bool IsIndexToegewezen;

        private readonly ApplicationDbContext _context;

        public AanvraagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Aanvraags
        public async Task<IActionResult> Index(string userId, string searchString)
        {
            IsIndexallaanvragen = false;
            IsIndexToegewezen = false;
            IsIndex = false;

            ViewData["CurrentFilter"] = searchString;


            if (userId == null)
            {
                userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            var applicationDbContext = from s in _context.Aanvragen.Where(u => u.UserId == userId)
                .Include(a => a.ApplicationUser)
                .Include(a => a.Status)
                .Include(a => a.BehandelaarApplicationUser)
                                       select s;


            if (!string.IsNullOrEmpty(searchString))
            {
                applicationDbContext = applicationDbContext
                    .Where(s => s.ApplicationUser.Voornaam.Contains(searchString) ||
                    s.ApplicationUser.Achternaam.Contains(searchString) ||
                    s.Titel.Contains(searchString) ||
                    s.Omschrijving.Contains(searchString) ||
                    s.Status.Naam.Contains(searchString));
            }

            return View(await applicationDbContext.ToListAsync());
        }

        //Get : Aanvraags
        [Authorize(Roles = SD.AdminBehandelaar)]
        public async Task<IActionResult> IndexAllAanvragen(string searchString, string sortOrder, string path)
        {
            IsIndexallaanvragen = false;
            IsIndexToegewezen = false;
            IsIndex = false;
            ViewBag.Url = Request.Path;
            //search box
            ViewData["CurrentFilter"] = searchString;
            //sorteren
            ViewData["GebruikerSort"] = String.IsNullOrEmpty(sortOrder) ? "gebruiker_desc" : "";
            ViewData["BehandelaarSort"] = String.IsNullOrEmpty(sortOrder) ? "behandelaar_desc" : "";
            ViewData["StatusSort"] = String.IsNullOrEmpty(sortOrder) ? "status_desc" : "";



            var lijstaanvragen = from a in _context.Aanvragen
               .Include(a => a.ApplicationUser)
               .Include(a => a.Status)
               .Include(a => a.BehandelaarApplicationUser)
               .OrderBy(a => a.ApplicationUser.UserName)
                                 select a;
            //sort           

            //search
            if (!string.IsNullOrEmpty(searchString))
            {
                lijstaanvragen = lijstaanvragen
                    .Where(
                       s => s.ApplicationUser.Voornaam.Contains(searchString)
                    || s.ApplicationUser.Achternaam.Contains(searchString)
                    || s.Titel.Contains(searchString)
                    || s.Omschrijving.Contains(searchString)
                    || s.ApplicationUser.UserName.Contains(searchString)
                    || s.ApplicationUser.Email.Contains(searchString)
                    || s.Status.Naam.Contains(searchString));
            }

            return View(await lijstaanvragen.ToListAsync());
        }
        // Get: Aanvraags
        [Authorize(Roles = SD.AdminBehandelaar)]
        public async Task<IActionResult> IndexToegewezen(string userId)
        {
            IsIndexallaanvragen = false;
            IsIndexToegewezen = false;
            IsIndex = false;

            if (userId == null)
            {
                userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var lijstaanvragen = _context.Aanvragen
                .Include(a => a.ApplicationUser)
                .Include(a => a.BehandelaarApplicationUser)
                .Include(a => a.Status)
                .Where(a => a.BehandelaarId == userId);
            return View(await lijstaanvragen.ToListAsync());
        }



        // GET: Aanvraags/Details/5
        public async Task<IActionResult> Details(int? id, bool index, bool isAlles, bool isToegezen)
        {


            ViewBag.index = index;
            ViewBag.isAlles = isAlles;
            ViewBag.isToegezen = isToegezen;


            if (id == null)
            {
                return NotFound();
            }

            return await AanvraagberichtenViewmodel(id);
        }


        private async Task<IActionResult> AanvraagberichtenViewmodel(int? id)
        {
            var aanvraag = await _context.Aanvragen
                            .Include(a => a.ApplicationUser)
                            .Include(a => a.Status)
                            .Include(a => a.BehandelaarApplicationUser)
                            .Include(a => a.Berichten)
                            .SingleOrDefaultAsync(m => m.Id == id);

            var berichten = await _context.Berichten
                .Where(a => a.AanvraagId.Equals(id))
                .Include(a => a.applicationuser)
                .ToListAsync();


            if (aanvraag == null)
            {
                return NotFound();
            }

            var model = new AanvraagBerichtenViewmodel
            {
                aanvraag = aanvraag,
                berichten = berichten
            };

            return View(model);
        }

        //post
        public async Task<IActionResult> AddBericht(Bericht bericht)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            bericht.applicationuserId = userId;


            int aanvraagId = bericht.AanvraagId;
            if (ModelState.IsValid)
            {
                _context.Add(bericht);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Aanvraags", new { id = aanvraagId });
            }

            return await AanvraagberichtenViewmodel(bericht.AanvraagId);
        }

        // GET: Aanvraags/Create
        public IActionResult Create(bool isVraag, string UserId, bool index, bool isAlles, bool isToegezen)
        {

            //ViewBag.index = index;
            //ViewBag.isAlles = isAlles;
            //ViewBag.isToegezen = isToegezen;

            Aanvraag aanvraag;

            if (UserId == null)
            {
                UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            aanvraag = new Aanvraag
            {
                IsVraag = true,
                UserId = UserId               
            };

            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Voornaam");
            //ViewData["StatusId"] = new SelectList(_context.Statussen, "Id", "Naam");
            return View(aanvraag);
        }

        // POST: Aanvraags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Aanvraag aanvraag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aanvraag);
                await _context.SaveChangesAsync();
                if (IsIndex)
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (IsIndexallaanvragen)
                {
                    return RedirectToAction(nameof(IndexAllAanvragen));
                }
                else if (IsIndexToegewezen)
                {
                    return RedirectToAction(nameof(IndexToegewezen));
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", aanvraag.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statussen, "Id", "Id", aanvraag.StatusId);
            return View(aanvraag);
        }

        // GET: Aanvraags/Edit/5
        public async Task<IActionResult> Edit(int? id, bool isVraag, string Url, bool index, bool isAlles, bool isToegezen)
        {
            ViewBag.index = index;
            ViewBag.isAlles = isAlles;
            ViewBag.isToegezen = isToegezen;

            IsIndex = index;
            IsIndexToegewezen = isToegezen;
            IsIndexallaanvragen = isAlles;

            if (id == null)
            {
                return NotFound();
            }
            ViewData["UrlPath"] = Url;

            var aanvraag = await _context.Aanvragen.SingleOrDefaultAsync(m => m.Id == id);
            isVraag = aanvraag.IsVraag;
            if (aanvraag == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", aanvraag.UserId);
            ViewData["StatusList"] = new SelectList(_context.Statussen, "Id", "Naam");
            ViewData["BehandelaarId"] = new SelectList(_context.Users.Where(u => u.isBehandelaar == true), "Id", "Voornaam", aanvraag.BehandelaarId);

            return View(aanvraag);
        }

        // POST: Aanvraags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Aanvraag aanvraag, bool index, bool isAlles, bool isToegezen)
        {
            if (id != aanvraag.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aanvraag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AanvraagExists(aanvraag.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (index)
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (isAlles)
                {
                    return RedirectToAction(nameof(IndexAllAanvragen));
                }
                else if (isToegezen)
                {
                    return RedirectToAction(nameof(IndexToegewezen));
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", aanvraag.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statussen, "Id", "Id", aanvraag.StatusId);
            return View(aanvraag);
        }

        // GET: Aanvraags/Delete/5
        public async Task<IActionResult> Delete(int? id, bool index, bool isAlles, bool isToegezen)
        {

            ViewBag.index = index;
            ViewBag.isAlles = isAlles;
            ViewBag.isToegezen = isToegezen;

            IsIndex = index;
            IsIndexToegewezen = isToegezen;
            IsIndexallaanvragen = isAlles;

            if (id == null)
            {
                return NotFound();
            }

            var aanvraag = await _context.Aanvragen
                .Include(a => a.ApplicationUser)
                .Include(a => a.BehandelaarApplicationUser)
                .Include(a => a.Status)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (aanvraag == null)
            {
                return NotFound();
            }

            return View(aanvraag);
        }

        // POST: Aanvraags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool index, bool isAlles, bool isToegezen)
        {

            ViewBag.index = index;
            ViewBag.isAlles = isAlles;
            ViewBag.isToegezen = isToegezen;

            IsIndex = index;
            IsIndexToegewezen = isToegezen;
            IsIndexallaanvragen = isAlles;

            var aanvraag = await _context.Aanvragen.SingleOrDefaultAsync(m => m.Id == id);
            _context.Aanvragen.Remove(aanvraag);
            await _context.SaveChangesAsync();
            if (IsIndex)
            {
                return RedirectToAction(nameof(Index));
            }
            else if (IsIndexallaanvragen)
            {
                return RedirectToAction(nameof(IndexAllAanvragen));
            }
            else if (IsIndexToegewezen)
            {
                return RedirectToAction(nameof(IndexToegewezen));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AanvraagExists(int id)
        {
            return _context.Aanvragen.Any(e => e.Id == id);
        }
    }
}
