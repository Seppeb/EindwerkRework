using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationRequestIt.Data;
using ApplicationRequestIt.Models;
using Microsoft.AspNetCore.Authorization;
using ApplicationRequestIt.Utility;
using Microsoft.AspNetCore.Identity;

namespace ApplicationRequestIt.Controllers
{
    [Authorize(Roles = (SD.AdminEndUser))]
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public ApplicationUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: ApplicationUsers
        public async Task<IActionResult> Index(bool isAdmin, bool isBehandelaar, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var lijstgebruikers = _context.ApplicationUser.AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                lijstgebruikers = lijstgebruikers
                    .Where(
                    s => s.Voornaam.Contains(searchString) ||
                    s.Achternaam.Contains(searchString) ||
                    s.Klant.Contains(searchString) ||
                    s.UserName.Contains(searchString) ||
                    s.Email.Contains(searchString));
            }

            return View(await lijstgebruikers.ToListAsync());
        }

        // GET: ApplicationUsers/Details/5
        public async Task<IActionResult> Details(string id, bool isAdmin, bool isBehandelaar)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        public async Task<IActionResult> Edit(string id, bool isAdmin, bool isBehandelaar)
        {

            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }


        public void setAdmin(ApplicationUser user)
        {
            if (user.isAdmin == true)
            {
                _userManager.AddToRoleAsync(user, SD.AdminEndUser);
                _context.SaveChangesAsync();
            }
            else if (user.isAdmin != true)
            {
                _userManager.RemoveFromRoleAsync(user, SD.AdminEndUser);
                _context.SaveChangesAsync();
            }

        }
        public void setBehandelaar(ApplicationUser user)
        {
            if (user.isBehandelaar == true)
            {
                _userManager.AddToRoleAsync(user, SD.BehandelaarEndUser);
                _context.SaveChangesAsync();
            }
            else if (user.isBehandelaar != true)
            {
                _userManager.RemoveFromRoleAsync(user, SD.BehandelaarEndUser);
                _context.SaveChangesAsync();
            }



        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser applicationUser)
        {
            setAdmin(applicationUser);
            setBehandelaar(applicationUser);

            if (id != applicationUser.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationUser);
                    _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id))
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
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Delete/5
        public async Task<IActionResult> Delete(string id, bool isAdmin, bool isBehandelaar)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .Include(a => a.BehandelaarAanvragen)
                .Include(a => a.CustomerAanvragen)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);


            // _context.ApplicationUser.Remove(applicationUser);
            if (applicationUser.isEnabled == true)
            {
                applicationUser.isEnabled = false;
            }
            else if (applicationUser.isEnabled == false)
            {
                applicationUser.isEnabled = true;
            }
            
            _context.Update(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUser.Any(e => e.Id == id);
        }

        [HttpPost, ActionName("Activeer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activeer(string id)
        {
            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
                        
            applicationUser.isEnabled = true;
            _context.Update(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
                
    }
}
