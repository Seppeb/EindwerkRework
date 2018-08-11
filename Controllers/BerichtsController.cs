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

namespace ApplicationRequestIt.Controllers
{
    public class BerichtsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BerichtsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Berichts
        public async Task<IActionResult> Index(int AanvraagId)
        {
            ViewData["AanvraagId"] = AanvraagId;
            var applicationDbContext = _context.Berichten
                .Include(b => b.Aanvraag)
                .Where(a => a.AanvraagId == AanvraagId)
                .Include(a => a.applicationuser)
                ;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Berichts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bericht = await _context.Berichten
                .Include(b => b.Aanvraag)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (bericht == null)
            {
                return NotFound();
            }

            return View(bericht);
        }

        // GET: Berichts/Create
        public IActionResult Create(int AanvraagId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            Bericht bericht = new Bericht
            {
                AanvraagId = AanvraagId,
                applicationuserId = userId
            };
            ViewData["AanvraagId"] = AanvraagId;

            return View(bericht);
        }

        // POST: Berichts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titel,Inhoud,StartDate,AanvraagId,applicationuserId")] Bericht bericht)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(bericht);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AanvraagId"] = new SelectList(_context.Aanvragen, "Id", "Omschrijving", bericht.AanvraagId);
            return View(bericht);
        }

        // GET: Berichts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bericht = await _context.Berichten.SingleOrDefaultAsync(m => m.Id == id);
            if (bericht == null)
            {
                return NotFound();
            }
            ViewData["AanvraagId"] = new SelectList(_context.Aanvragen, "Id", "Omschrijving", bericht.AanvraagId);
            return View(bericht);
        }

        // POST: Berichts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titel,Inhoud,StartDate,AanvraagId,UserId")] Bericht bericht)
        {
            if (id != bericht.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bericht);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BerichtExists(bericht.Id))
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
            ViewData["AanvraagId"] = new SelectList(_context.Aanvragen, "Id", "Omschrijving", bericht.AanvraagId);
            return View(bericht);
        }

        // GET: Berichts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bericht = await _context.Berichten
                .Include(b => b.Aanvraag)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (bericht == null)
            {
                return NotFound();
            }

            return View(bericht);
        }

        // POST: Berichts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bericht = await _context.Berichten.SingleOrDefaultAsync(m => m.Id == id);
            _context.Berichten.Remove(bericht);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BerichtExists(int id)
        {
            return _context.Berichten.Any(e => e.Id == id);
        }
    }
}
