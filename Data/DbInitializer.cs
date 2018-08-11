using ApplicationRequestIt.Models;
using ApplicationRequestIt.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Data
{
    public class DbInitializer
    {
        private static object serviceProvider;

        public static async Task Initialize(ApplicationDbContext context, IServiceProvider serviceProvider)
        {

            await context.Database.EnsureCreatedAsync();

            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { SD.AdminEndUser, SD.BehandelaarEndUser, SD.CustomerEndUser };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //rollen maken
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            // hier 4 initiele users aanmaken
            if (UserManager.Users.Count() == 0)
            {
                var admin = new ApplicationUser
                {
                    UserName = "Admin@admin.be",
                    Email = "Admin@admin.be",
                    isAdmin = true,
                    Voornaam = "Seppe",
                    Achternaam = "Berghmans",
                };
                var Behandelaar = new ApplicationUser
                {
                    UserName = "Behandelaar@behandelaar.be",
                    Email = "Behandelaar@behandelaar.be",
                    isBehandelaar = true,
                    Voornaam = "Felix",
                    Achternaam = "Bergs",
                };
                var user = new ApplicationUser
                {
                    UserName = "User@user.be",
                    Email = "User@user.be",
                    Voornaam = "Pieter",
                    Achternaam = "Kardinaals",
                    Klant = "Torfs"
                };
                var user2 = new ApplicationUser
                {
                    UserName = "koen@devoslemmens.be",
                    Email = "koen@devoslemmens.be",
                    Voornaam = "Koen",
                    Achternaam = "Lemmens",
                    Klant = "De vos lemmens"
                };
                // paswoorden voor de users

                string behandelaarPWD = "Behandelaar!234";
                string userPWD = "User!234";
                string koenpsw = "Koen!234";
                string adminPWD = "Admin!234";
                
                // user zoeken, indien niet bestaat word deze aangemaakt door usermanager.

                var _behandelaar = await UserManager.FindByEmailAsync("Behandelaar@behandelaar.be");
                var _user = await UserManager.FindByEmailAsync("User@user.be");
                var _koen = await UserManager.FindByEmailAsync("koen@devoslemmens.be");                
                var _admin = await UserManager.FindByEmailAsync("Admin@admin.be");

                if (_admin == null)
                {
                    var createPowerUser = await UserManager.CreateAsync(admin, adminPWD);
                    if (createPowerUser.Succeeded)
                    {
                        // aan rol toevoegen
                        await UserManager.AddToRoleAsync(admin, SD.AdminEndUser);
                    }
                }
                if (_behandelaar == null)
                {
                    var createPowerUser = await UserManager.CreateAsync(Behandelaar, behandelaarPWD);
                    if (createPowerUser.Succeeded)
                    {
                        // aan rol toevoegen
                        await UserManager.AddToRoleAsync(Behandelaar, SD.BehandelaarEndUser);
                    }
                }
                if (_user == null)
                {
                    var createPowerUser = await UserManager.CreateAsync(user, userPWD);
                    if (createPowerUser.Succeeded)
                    {
                        // aan rol toevoegen
                        await UserManager.AddToRoleAsync(user, SD.CustomerEndUser);
                    }
                }
                if (_koen == null)
                {
                    var createPowerUser = await UserManager.CreateAsync(user2, koenpsw);
                    if (createPowerUser.Succeeded)
                    {
                        // aan rol toevoegen
                        await UserManager.AddToRoleAsync(user2, SD.CustomerEndUser);

                    }
                }
            }

            if (!context.Statussen.Any())
            {
                var statussen = new Status[]
             {
                new Status { Naam = "Verstuurd", Omschrijving = "Status is verstuurd en wacht op ontvangst" },
                new Status { Naam = "Ontvangen", Omschrijving = "Aanvraag is onvangen en wordt z.s.m toegewezen" },
                new Status { Naam = "Toegewezen", Omschrijving = "Aanvraag is toegewezen aan een behandelaar" },
                new Status { Naam = "In Behandeling", Omschrijving = "Aanvraag is in behandeling" },
                new Status { Naam = "Wacht op goedkeuring", Omschrijving = "Aanvraag is klaar en wacht op goedkeuring klant" },
                new Status { Naam = "Klaar", Omschrijving = "Aanvraag is klaar" },
                new Status { Naam = "Afgesloten", Omschrijving = "Aanvraag afgehandeld" }
             };
                foreach (Status s in statussen)
                {
                    context.Statussen.Add(s);
                }
            }

            await context.SaveChangesAsync();

            // dit het ik in een aparte klasse gestoken, gaf errors omdat deze status objecten niet kon vinden.

            //if (!context.Aanvragen.Any())
            //{
            //    var Aanvragen = new Aanvraag[]
            // {
            //     new Aanvraag { ApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("Koen")), Titel="Niewe saus", BehandelaarApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("Felix")),
            //         StartDatum = new DateTime(2018,08,20), EindDatum = new DateTime(2018,10,20), StatusId = 3, LinkVoorbeeldKlant = "www.niewesaus.be", LinkVoorbeeldBehandelaar = "www.voorbeeldsaus.be",
            //         Omschrijving ="we hebben een nieuwe saus ontwikkelt in ons assortiment en zouden dit graat laten zien op een signage schermen.", Aanmaakdatum = DateTime.Now },

            //      new Aanvraag { ApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("Koen")), Titel="Promotie op Pita saus",
            //         StartDatum = new DateTime(2018,10,01), EindDatum = new DateTime(2019,02,01), StatusId = 1, LinkVoorbeeldKlant = "www.pitasaus.be",
            //         Omschrijving ="voor onze promotie op onze pita saus willen we graag extra content.", Aanmaakdatum = DateTime.Now.AddDays(20) },

            //       new Aanvraag { ApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("pieter")), Titel="Wintercollectie", BehandelaarApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("Felix")),
            //         StartDatum = new DateTime(2018,07,20), EindDatum = new DateTime(2019,02,20), StatusId = 2, LinkVoorbeeldKlant = "www.wintercollectietorfs.be", LinkVoorbeeldBehandelaar = "www.wintercollectiefinal.be",
            //         Omschrijving ="er komt een wintercollectie aan die we in de kijker willen zetten, kijk link voor welke", Aanmaakdatum = DateTime.Now.AddDays(10) },

            //        new Aanvraag { ApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("pieter")), Titel="Niewe mannenmode", BehandelaarApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("Felix")),
            //         StartDatum = new DateTime(2018,05,10), EindDatum = new DateTime(2018,10,20), StatusId = 4, LinkVoorbeeldKlant = "www.mannenmodetorfs.be", LinkVoorbeeldBehandelaar = "www.mannenmodevoorbeeld.be",
            //         Omschrijving ="onze mannamode collectie is vernieuwe en willen we in de kijker zetten", Aanmaakdatum = DateTime.Now.AddDays(12) },
            // };
            //    foreach (Aanvraag s in Aanvragen)
            //    {
            //        context.Aanvragen.Add(s);
            //    }
            //}

            //if (!context.Berichten.Any())
            //{
            //    var berichten = new Bericht[]
            // {          
            // };
            //    foreach (Bericht s in berichten)
            //    {
            //        context.Berichten.Add(s);
            //    }
            //}
        }
        public static async Task Aanvragen(ApplicationDbContext context, IServiceProvider serviceProvider)
        {

            await context.Database.EnsureCreatedAsync();

            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            if (!context.Aanvragen.Any())
            {
                var Aanvragen = new Aanvraag[]
             {
                 new Aanvraag { ApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("Koen")),
                     Titel ="Niewe saus",
                     BehandelaarApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("Felix")),
                     StartDatum = new DateTime(2018,08,20), EindDatum = new DateTime(2018,10,20),
                     StatusId = 3, LinkVoorbeeldKlant = "www.niewesaus.be", LinkVoorbeeldBehandelaar = "www.voorbeeldsaus.be",
                     Omschrijving ="we hebben een nieuwe saus ontwikkelt in ons assortiment en zouden dit graat laten zien op een signage schermen.",
                     Aanmaakdatum = DateTime.Now },

                  new Aanvraag { ApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("Koen")), Titel="Promotie op Pita saus",
                     StartDatum = new DateTime(2018,10,01), EindDatum = new DateTime(2019,02,01), StatusId = 1, LinkVoorbeeldKlant = "www.pitasaus.be",
                     Omschrijving ="voor onze promotie op onze pita saus willen we graag extra content.", Aanmaakdatum = DateTime.Now.AddDays(20) },

                   new Aanvraag { ApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("pieter")), Titel="Wintercollectie", BehandelaarApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("Felix")),
                     StartDatum = new DateTime(2018,07,20), EindDatum = new DateTime(2019,02,20), StatusId = 2, LinkVoorbeeldKlant = "www.wintercollectietorfs.be", LinkVoorbeeldBehandelaar = "www.wintercollectiefinal.be",
                     Omschrijving ="er komt een wintercollectie aan die we in de kijker willen zetten, kijk link voor welke", Aanmaakdatum = DateTime.Now.AddDays(10) },

                    new Aanvraag { ApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("pieter")), Titel="Niewe mannenmode", BehandelaarApplicationUser = UserManager.Users.Single(s => s.Voornaam.Equals("Felix")),
                     StartDatum = new DateTime(2018,05,10), EindDatum = new DateTime(2018,10,20), StatusId = 4, LinkVoorbeeldKlant = "www.mannenmodetorfs.be", LinkVoorbeeldBehandelaar = "www.mannenmodevoorbeeld.be",
                     Omschrijving ="onze mannamode collectie is vernieuwe en willen we in de kijker zetten", Aanmaakdatum = DateTime.Now.AddDays(12) },
             };
                foreach (Aanvraag s in Aanvragen)
                {
                    context.Aanvragen.Add(s);
                }
            }            
            await context.SaveChangesAsync();
        }
    }
}
