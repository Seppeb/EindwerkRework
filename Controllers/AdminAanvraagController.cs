using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApplicationRequestIt.Data;
using ApplicationRequestIt.Utility;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationRequestIt.Models;

namespace ApplicationRequestIt.Controllers
{
    public class AdminAanvraagController : Controller
    {

        private readonly ApplicationDbContext _context;

        public AdminAanvraagController(ApplicationDbContext context)
        {
            _context = context;
        }
        //get: Aanvraags
        [Authorize(Roles = SD.AdminBehandelaar)]
        public async Task<IActionResult> IndexAllAanvragen()
        {
            var lijstaanvragen = _context.Aanvragen
                .Include(a => a.ApplicationUser)
                .Include(a => a.Status)
                .Include(a => a.BehandelaarApplicationUser);
            return View(await lijstaanvragen.ToListAsync());
        }

        [Authorize(Roles = SD.AdminBehandelaar)]
        public async Task<IActionResult> IndexToegewezen(string userId)
        {
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
        // GET: Aanvraags/Edit/5
        public async Task<IActionResult> EditAllAanvragen(int? id, bool isVraag)
        {
            if (id == null)
            {
                return NotFound();
            }

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

        public async Task<IActionResult> EditToegewezen(int? id, bool isVraag)
        {
            if (id == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> EditAllAanvragen(int id, Aanvraag aanvraag)
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
                //return RedirectToAction(nameof(IndexAllAanvragen));
                return RedirectToAction("IndexAllAanvragen");

            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", aanvraag.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statussen, "Id", "Id", aanvraag.StatusId);
            return View(aanvraag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditToegewezen(int id, Aanvraag aanvraag)
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
                return RedirectToAction(nameof(IndexToegewezen));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", aanvraag.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statussen, "Id", "Id", aanvraag.StatusId);
            return View(aanvraag);
        }

        private bool AanvraagExists(int id)
        {
            return _context.Aanvragen.Any(e => e.Id == id);
        }

    }
}