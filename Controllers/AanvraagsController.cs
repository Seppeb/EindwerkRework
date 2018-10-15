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
using ApplicationRequestIt.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ApplicationRequestIt.Controllers
{
    [Authorize]
    public class AanvraagsController : Controller
    {

        public bool IsIndexallaanvragen;
        public bool IsIndex;
        public bool IsIndexToegewezen;
        public string Email { get; set; }

        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public AanvraagsController(ApplicationDbContext context, IEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
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

           // aanvragen van de user met behandelaar 
            var Aanvragen = from s in _context.Aanvragen.Where(u => u.UserId == userId)
                .Include(a => a.ApplicationUser)
                .Include(a => a.Status)
                .Include(a => a.AanvraagBehandelaars).ThenInclude(s => s.Behandelaar)              
                                       select s;
            // toegewezen behandelaars
            var toegewezenBehandelaars = _context.AanvraagBehandelaars.Include(s => s.Behandelaar).ThenInclude(s=>s.Voornaam + s.Achternaam);


            if (!string.IsNullOrEmpty(searchString))
            {
                Aanvragen = Aanvragen
                    .Where(
                    s => s.ApplicationUser.Voornaam.Contains(searchString)
                    || s.ApplicationUser.Achternaam.Contains(searchString)
                    || s.Titel.Contains(searchString)
                    || s.Omschrijving.Contains(searchString)
                    || s.Status.Naam.Contains(searchString)
                                        ); 
                 
            }

            return View(await Aanvragen.ToListAsync());
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
               .Include(ab => ab.AanvraagBehandelaars)
               .ThenInclude(ab => ab.Behandelaar)
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
                    //|| s.AanvraagBehandelaars.SelectMany(searchString)
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

            var user = await _userManager.FindByIdAsync(userId);

            var aanvraagbehandelaarslist = _context.AanvraagBehandelaars.ToListAsync();

            var lijstaanvragen = _context.AanvraagBehandelaars
                .Where(a => a.BehandelaarId == userId)
                .Include(a => a.Aanvraag)
                .ThenInclude(a => a.ApplicationUser)
                .Include(a => a.Aanvraag).ThenInclude(a => a.Status)
                .Include(ab => ab.Behandelaar);
                              
                
                //.FirstOrDefault(a => a.AanvraagBehandelaars.Behandelaar.BehandelaarId == userId);
            //var toegewezenAanvragen = lijstaanvragen);
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
                            .Include(a => a.AanvraagBehandelaars)
                            .ThenInclude(s => s.Behandelaar)
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
                berichten = berichten,
            };

            return View(model);
        }

        //post
        public async Task<IActionResult> AddBericht(Bericht bericht)
        {
            
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            


            bericht.applicationuserId = userId;
            int aanvraagId = bericht.AanvraagId;
            var aanvraag = _context.Aanvragen.Where(x => x.Id == aanvraagId).Include(a=>a.ApplicationUser).SingleOrDefault();
            var status = await _context.Statussen.Where(s => s.Id.Equals(aanvraag.StatusId)).FirstOrDefaultAsync();

            var behandelaar = _context.Users.Where(x => x.Id == bericht.applicationuserId).SingleOrDefault();

            if (ModelState.IsValid)
            {
                var aanmaak = new AanvraagGeschiedenis
                {
                    AanvraagId = aanvraag.Id,
                    Aanvraag = aanvraag,
                    Actie = "Bericht toegevoegd aan de aanvraag : " + bericht.Inhoud,
                    time = DateTime.Now,
                    Voornaam = user.Voornaam,
                    Achternaam = user.Achternaam,
                    Status = status.Naam
                };
                _context.AanvraagGeschiedenis.Add(aanmaak);
                _context.Add(bericht);
                await _context.SaveChangesAsync();

               

                // dat user van aanvraag zelf bericht plaatst
                if (bericht.applicationuser.Email == aanvraag.ApplicationUser.Email)
                {
                    //mail naar user dat bericht geplaatst is
                    await _emailSender.SendEmailAsync(user.Email, "Bericht voor aanvraag: " + aanvraag.Titel, $"Je bericht: " + bericht.Inhoud + " is succesvol geplaatst, de behandelaars zullen je zo spoedig mogelijk helpen.");
                    //moet nog komen, mail naar behandelaar dat user bericht heeft geplaatst
                }
                //dat gebruiker bericht plaatst
                else if(bericht.applicationuser.Email != aanvraag.ApplicationUser.Email)
                {
                    //mail naar user als behandelaar bericht plaatst
                    await _emailSender.SendEmailAsync(aanvraag.ApplicationUser.Email, "Nieuw bericht voor aanvraag: " + aanvraag.Titel, $"De behandelaar: " + behandelaar.Voornaam + " " + behandelaar.Achternaam + " heeft het bericht met inhoud: " + bericht.Inhoud + " geplaatst, meer info vindt je terug in de details van je aanvraag.");
                    //mail naar behandelaar dat bericht goed geplaatst is
                    await _emailSender.SendEmailAsync(user.Email, "Bericht voor aanvraag: " + aanvraag.Titel, $"Je bericht: " + bericht.Inhoud + " is succesvol geplaatst.");

                }
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
            
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var status = await _context.Statussen.Where(s => s.Id.Equals(aanvraag.StatusId)).FirstOrDefaultAsync();


            if (ModelState.IsValid)
            {

                var aanmaak = new AanvraagGeschiedenis
                {
                    AanvraagId = aanvraag.Id,
                    Aanvraag = aanvraag,
                    Actie = "Aanmaak van de aanvraag",
                    time = DateTime.Now,
                    Voornaam = user.Voornaam,
                    Achternaam = user.Achternaam,
                    Status = status.Naam
                };

                _context.AanvraagGeschiedenis.Add(aanmaak);
                _context.Add(aanvraag);             
                await _context.SaveChangesAsync();
                await _emailSender.SendEmailAsync(user.Email, "Aanmaak Gelukt", $"Het aanmaken van de aanvraag:  " + aanvraag.Titel + " is gelukt.");


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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBehandelaar(int id, Aanvraag aanvraag, bool index, bool isAlles, bool isToegezen, string bid)
        {
            //ViewBag.behandelaar = behandelaar;
            //om behandelaar te krijgen die aan de tussentabel moet gekoppelt worden
            
            var B = await _userManager.FindByIdAsync(bid);
            //om email te verkrijgen van de aanvraag gebruiker
            var user = await _userManager.FindByIdAsync(aanvraag.UserId);

            //ingelogde user 
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ingelogdeUser = await _userManager.FindByIdAsync(userId);
            var status = await _context.Statussen.Where(s => s.Id.Equals(aanvraag.StatusId)).FirstOrDefaultAsync();




            var aanvraagbehandelaar = new AanvraagBehandelaar
            {
                AanvraagId = id,
                Aanvraag = aanvraag,
                BehandelaarId = bid,
                Behandelaar = B
            };


            if (id != aanvraag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var aanmaak = new AanvraagGeschiedenis
                    {
                        AanvraagId = aanvraag.Id,
                        Aanvraag = aanvraag,
                        Actie = "Verwijderen van behandelaar " + aanvraagbehandelaar.Behandelaar.Voornaam + " " + aanvraagbehandelaar.Behandelaar.Achternaam,
                        time = DateTime.Now,
                        Voornaam = ingelogdeUser.Voornaam,
                        Achternaam = ingelogdeUser.Achternaam,
                        Status = status.Naam
                    };
                    _context.AanvraagGeschiedenis.Add(aanmaak);

                    _context.AanvraagBehandelaars.Remove(aanvraagbehandelaar);
                    _context.Update(aanvraag);
                    await _context.SaveChangesAsync();
                    await _emailSender.SendEmailAsync(user.Email, "Verandering aan je aanvraag", $"De behandelaar: " + B.Voornaam + " " + B.Achternaam + " van de aanvraag: " + aanvraag.Titel  + " is verwijderd. ");
                    await _emailSender.SendEmailAsync(aanvraagbehandelaar.Behandelaar.Email, "Verwijderd van aanvraag: " + aanmaak.Aanvraag.Titel, $"Je bent verwijderd van de aanvraag: " + aanmaak.Aanvraag.Titel + ", log in om meer details te bekijken");


                }
                catch (Exception)
                {
                    throw;
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
            return View(aanvraag);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBehandelaar(int id, Aanvraag aanvraag, bool index, bool isAlles, bool isToegezen)
        {
            //om behandelaar te krijgen die aan de tussentabel moet gekoppelt worden
            var behandelaar = await _userManager.FindByIdAsync(aanvraag.BehandelaarId);
            //om email te verkrijgen van de aanvraag gebruiker
            var user = await _userManager.FindByIdAsync(aanvraag.UserId);

            //ingelogde user 
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ingelogdeUser = await _userManager.FindByIdAsync(userId);
            var status = await _context.Statussen.Where(s => s.Id.Equals(aanvraag.StatusId)).FirstOrDefaultAsync();



            var aanvraagbehandelaar = new AanvraagBehandelaar
            {
                AanvraagId = id,
                Aanvraag = aanvraag,
                BehandelaarId = aanvraag.BehandelaarId,
                Behandelaar = behandelaar                
            };
            if (id != aanvraag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var aanmaak = new AanvraagGeschiedenis
                    {
                        AanvraagId = aanvraag.Id,
                        Aanvraag = aanvraag,
                        Actie = "Toevoegen van behandelaar " + aanvraagbehandelaar.Behandelaar.Voornaam + " " + aanvraagbehandelaar.Behandelaar.Achternaam,
                        time = DateTime.Now,
                        Voornaam = ingelogdeUser.Voornaam,
                        Achternaam = ingelogdeUser.Achternaam,
                        Status = status.Naam
                    };
                    _context.AanvraagGeschiedenis.Add(aanmaak);


                    await _context.AanvraagBehandelaars.AddAsync(aanvraagbehandelaar);
                   _context.Update(aanvraag);
                   await _context.SaveChangesAsync();
                   await _emailSender.SendEmailAsync(user.Email, "Verandering aan je aanvraag", $"Er is een nieuwe behandelaar aan je aanvraag gehangen. " + aanvraagbehandelaar.Behandelaar.Voornaam + " " + aanvraagbehandelaar.Behandelaar.Achternaam + " zal nu meewerken aan een oplossing voor de aanvraag: " + aanvraag.Titel);
                   await _emailSender.SendEmailAsync(aanvraagbehandelaar.Behandelaar.Email, "Niewe toegewezen aanvraag" + aanmaak.Aanvraag.Titel, $"Je ben toegevoegd aan een de aanvraag " + aanmaak.Aanvraag.Titel + ", log in om meer details te bekijken");

                }
                catch (Exception)
                {
                    throw;
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
            return View(aanvraag);
        }

        // GET: Aanvraags/Edit/5
        public async Task<IActionResult> Edit(int? id, bool isVraag, string Url, bool index, bool isAlles, bool isToegezen)
        {

            //redirecting naar juiste pagina
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
            //aanvraag die ingeladen moet worden
            var aanvraag = await _context.Aanvragen.Include(s => s.AanvraagBehandelaars).ThenInclude(s => s.Behandelaar)
                .SingleOrDefaultAsync(m => m.Id == id);
            //user van de aanvraag
            var user = _context.Users.Where(a => a.Id == aanvraag.UserId).SingleOrDefault();
            //lijst van alle behandelaars
            var AlleBehandelaars = _context.Users
                .Where(x => x.isBehandelaar == true);
           
            isVraag = aanvraag.IsVraag;
            if (aanvraag == null)
            {
                return NotFound();
            }

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", aanvraag.UserId);
            ViewData["StatusList"] = new SelectList(_context.Statussen, "Id", "Naam");
            ViewData["BehandelaarId"] = new SelectList(AlleBehandelaars, "Id", "Voornaam", aanvraag.AanvraagBehandelaars);
            ViewData["isEnabled"] = user.isEnabled;
            // ViewData["Behandelaars"] = aanvraag.AanvraagBehandelaars.ToList(); 


            return View(aanvraag);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Aanvraag aanvraag, bool index, bool isAlles, bool isToegezen)
        {
            var user = await _userManager.FindByIdAsync(aanvraag.UserId);

            //lijstvanbehandelaarsvanaanvraag
            var lijstbehandelaarsAanvraag = await _context.AanvraagBehandelaars.Include(x => x.Behandelaar).ToListAsync();

            // var result = _context.AanvraagBehandelaars.Where(x => x.Behandelaar);

            //ingelogde user 
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ingelogdeUser = await _userManager.FindByIdAsync(userId);

            //load related data
            var status = await _context.Statussen.Where(s => s.Id.Equals(aanvraag.StatusId)).FirstOrDefaultAsync();


            if (id != aanvraag.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //aanmaak van history data 
                    var aanmaak = new AanvraagGeschiedenis
                    {
                        AanvraagId = aanvraag.Id,
                        Aanvraag = aanvraag,
                        Actie = "Aanpassen van details van de aanvraag",
                        time = DateTime.Now,
                        Voornaam = ingelogdeUser.Voornaam,
                        Achternaam = ingelogdeUser.Achternaam,
                        Status = status.Naam
                    };

                    //toevoegen van data aan table              
                    _context.AanvraagGeschiedenis.Add(aanmaak);
                    //update van table zelf 

                    _context.Update(aanvraag);
                    await _context.SaveChangesAsync();
                    await _emailSender.SendEmailAsync(user.Email, "Verandering aan je aanvraag", $"Er is een aanpassing gebeurt aan de aanvraag " + aanvraag.Titel + ",om de veranderingen te bekijken gelieve in te loggen.");
                    //if (aanvraag.AanvraagBehandelaars.Any())
                    //{ }
                 

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
                .Include(a => a.AanvraagBehandelaars)
                .ThenInclude(s => s.Behandelaar)
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
