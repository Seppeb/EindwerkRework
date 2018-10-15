using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationRequestIt.Data;
using ApplicationRequestIt.Models;

namespace ApplicationRequestIt.Controllers
{
    public class AanvraagGeschiedenisController : Controller
    {
        private readonly ApplicationDbContext _context;
        public bool IsIndexallaanvragen;
        public bool IsIndex;
        public bool IsIndexToegewezen;

        public AanvraagGeschiedenisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AanvraagGeschiedenis
        public async Task<IActionResult> Index(int id, bool index, bool isAlles, bool isToegezen)
        {

            ViewBag.index = index;
            ViewBag.isAlles = isAlles;
            ViewBag.isToegezen = isToegezen;


            IsIndex = index;
            IsIndexToegewezen = isToegezen;
            IsIndexallaanvragen = isAlles;

            

            var applicationDbContext = from s in _context.AanvraagGeschiedenis
                                       .Where(a => a.AanvraagId == id)
                                       select s;
            ViewBag.id = id;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AanvraagGeschiedenis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aanvraagGeschiedenis = await _context.AanvraagGeschiedenis
                .Include(a => a.Aanvraag)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (aanvraagGeschiedenis == null)
            {
                return NotFound();
            }

            return View(aanvraagGeschiedenis);
        }

        // GET: AanvraagGeschiedenis/Create
        public IActionResult Create()
        {
            ViewData["AanvraagId"] = new SelectList(_context.Aanvragen, "Id", "Omschrijving");
            return View();
        }

        // POST: AanvraagGeschiedenis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Actie,time,Voornaam,Achternaam,AanvraagId")] AanvraagGeschiedenis aanvraagGeschiedenis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aanvraagGeschiedenis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AanvraagId"] = new SelectList(_context.Aanvragen, "Id", "Omschrijving", aanvraagGeschiedenis.AanvraagId);
            return View(aanvraagGeschiedenis);
        }

        // GET: AanvraagGeschiedenis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aanvraagGeschiedenis = await _context.AanvraagGeschiedenis.SingleOrDefaultAsync(m => m.Id == id);
            if (aanvraagGeschiedenis == null)
            {
                return NotFound();
            }
            ViewData["AanvraagId"] = new SelectList(_context.Aanvragen, "Id", "Omschrijving", aanvraagGeschiedenis.AanvraagId);
            return View(aanvraagGeschiedenis);
        }

        // POST: AanvraagGeschiedenis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Actie,time,Voornaam,Achternaam,AanvraagId")] AanvraagGeschiedenis aanvraagGeschiedenis)
        {
            if (id != aanvraagGeschiedenis.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aanvraagGeschiedenis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AanvraagGeschiedenisExists(aanvraagGeschiedenis.Id))
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
            ViewData["AanvraagId"] = new SelectList(_context.Aanvragen, "Id", "Omschrijving", aanvraagGeschiedenis.AanvraagId);
            return View(aanvraagGeschiedenis);
        }

        // GET: AanvraagGeschiedenis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aanvraagGeschiedenis = await _context.AanvraagGeschiedenis
                .Include(a => a.Aanvraag)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (aanvraagGeschiedenis == null)
            {
                return NotFound();
            }

            return View(aanvraagGeschiedenis);
        }

        // POST: AanvraagGeschiedenis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aanvraagGeschiedenis = await _context.AanvraagGeschiedenis.SingleOrDefaultAsync(m => m.Id == id);
            _context.AanvraagGeschiedenis.Remove(aanvraagGeschiedenis);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AanvraagGeschiedenisExists(int id)
        {
            return _context.AanvraagGeschiedenis.Any(e => e.Id == id);
        }
    }
}
