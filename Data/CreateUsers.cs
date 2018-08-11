using ApplicationRequestIt.Models;
using ApplicationRequestIt.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Data
{
    public class CreateUsers
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
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

            //hier 3 initiele users aanmaken
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
                Achternaam = "bergs",

            };
            var user = new ApplicationUser
            {
                UserName = "User@user.be",
                Email = "User@user.be",
                Voornaam = "pieter",
                Achternaam = "kardinaals",
                Klant = "torfs"
            };

            //paswoorden voor de users
            string adminPWD = "Admin!234";
            string behandelaarPWD = "Behandelaar!234";
            string userPWD = "User!234";

            var _admin = await UserManager.FindByEmailAsync("Admin@admin.be");
            var _behandelaar = await UserManager.FindByEmailAsync("Behandelaar@behandelaar.be");
            var _user = await UserManager.FindByEmailAsync("User@user.be");


            if (_admin == null)
            {
                var createPowerUser = await UserManager.CreateAsync(admin, adminPWD);
                if (createPowerUser.Succeeded)
                {
                    //aan rol toevoegen
                    await UserManager.AddToRoleAsync(admin, SD.AdminEndUser);
                }
            }
            if (_behandelaar == null)
            {
                var createPowerUser = await UserManager.CreateAsync(Behandelaar, behandelaarPWD);
                if (createPowerUser.Succeeded)
                {
                    //aan rol toevoegen
                    await UserManager.AddToRoleAsync(admin, SD.AdminEndUser);
                }
            }
            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(user, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //aan rol toevoegen
                    await UserManager.AddToRoleAsync(admin, SD.AdminEndUser);
                }
            }
        }

    }
}
