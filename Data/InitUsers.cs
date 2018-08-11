using ApplicationRequestIt.Models;
using ApplicationRequestIt.Utility;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Data
    // Testklasse om users te initialiseren maar werkte niet, werkende implemenentatie staat in startup
{    
        public interface IdentityDBIitializer
        {
            void Initialize();
        }

        public class LatestUserDBInitializer : IdentityDBIitializer
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public LatestUserDBInitializer(
                ApplicationDbContext context,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager)
            {
                _context = context;
                _userManager = userManager;
                _roleManager = roleManager;
            }

            //This example just creates an Administrator role and one Admin users
            public async void Initialize()
            {
                //create database schema if none existsC:\Users\bergh\Documents\Visual Studio 2017\Projects\ApplicationRequestIt\ApplicationRequestIt\Startup.cs
                _context.Database.EnsureCreated();

            await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
            await _roleManager.CreateAsync(new IdentityRole(SD.BehandelaarEndUser));
            await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));

            ////Create the Roles
            //if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
            //    {
            //        await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
            //    }
            //    if (!await _roleManager.RoleExistsAsync(SD.BehandelaarEndUser))
            //    {
            //        await _roleManager.CreateAsync(new IdentityRole(SD.BehandelaarEndUser));
            //    }
            //    if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
            //    {
            //        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));
            //    }
               
                //Create the default Admin account and apply the Administrator role
                string useradmin = "admin@admin.com";
                string passwordadmin = "Admin!234";
                await _userManager.CreateAsync(new ApplicationUser { UserName = useradmin, Email = useradmin }, passwordadmin);
                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(useradmin), SD.AdminEndUser);
                //Create Behandelaar
                string userbehandelaar = "behandelaar@behandelaar.com";
                string passwordbehandelaar = "Behandelaar!234";
                await _userManager.CreateAsync(new ApplicationUser { UserName = userbehandelaar, Email = userbehandelaar }, passwordbehandelaar);
                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(userbehandelaar), SD.BehandelaarEndUser);
                //Create user1
                string useruser1 = "user1@user.com";
                string passworduser1 = "User1!234";
                await _userManager.CreateAsync(new ApplicationUser { UserName = useruser1, Email = useruser1 }, passworduser1);
                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(useruser1), SD.CustomerEndUser);
                //create user2
                string useruser2 = "user2@user.com";
                string passworduser2 = "User2!234";
                await _userManager.CreateAsync(new ApplicationUser { UserName = useruser2, Email = useruser2 }, passworduser2);
                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(useruser2), SD.CustomerEndUser);
                //create user3
                string useruser3 = "user3@admin.com";
                string passworduser3 = "User3!234";
                await _userManager.CreateAsync(new ApplicationUser { UserName = useruser3, Email = useruser3 }, passworduser3);
                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(useruser3), SD.CustomerEndUser);
            }
        }
    }

