using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApplicationRequestIt.Data;
using ApplicationRequestIt.Models;
using ApplicationRequestIt.Services;
using ApplicationRequestIt.Utility;

namespace ApplicationRequestIt
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //users
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddSingleton<IEmailSender, EmailSender>();


            // Add Database Initializer
            services.AddScoped<IdentityDBIitializer, LatestUserDBInitializer>();

            // add email service.
            //services.AddSingleton<AuthMessageSenderOptions>(Configuration);

            services.AddMvc();

            //email integratie
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            //services.Configure & lt;AuthMessageSenderOptions & gt; (options = &gt;
            //{
            //    options.SendGridKey = "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";
            //    options.SendGridUser = "ApplicationRequestIt";
            //});
          //testveranderingstartup
          //test2 verandeirngsfsf
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //aanmaak data
            DbInitializer.Initialize(context, serviceProvider).Wait();

            //DbInitializer.Aanvragen(context, serviceProvider).Wait();
            
        }
        //    private async Task CreateUserRoles(IServiceProvider serviceProvider)
        //    { 
        //        var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //        var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        //        string[] roleNames = { SD.AdminEndUser, SD.BehandelaarEndUser, SD.CustomerEndUser};
        //        IdentityResult roleResult;

        //        foreach (var roleName in roleNames)
        //        {
        //            var roleExist = await RoleManager.RoleExistsAsync(roleName);
        //            if (!roleExist)
        //            {
        //                //rollen maken
        //                roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
        //            }
        //        }

        //        //hier 3 initiele users aanmaken
        //        var admin = new ApplicationUser
        //        {
        //            UserName = "Admin@admin.be",
        //            Email = "Admin@admin.be",
        //            isAdmin = true,
        //            Voornaam="Seppe",
        //            Achternaam="Berghmans",
        //        };
        //        var Behandelaar = new ApplicationUser
        //        {
        //            UserName = "Behandelaar@behandelaar.be",
        //            Email = "Behandelaar@behandelaar.be",
        //            isBehandelaar = true,
        //            Voornaam = "Felix",
        //            Achternaam = "bergs",

        //        };
        //        var user = new ApplicationUser
        //        {
        //            UserName = "User@user.be",
        //            Email = "User@user.be",                
        //            Voornaam = "pieter",
        //            Achternaam = "kardinaals",
        //            Klant="torfs"
        //        };

        //        //paswoorden voor de users
        //        string adminPWD = "Admin!234";
        //        string behandelaarPWD = "Behandelaar!234";
        //        string userPWD = "User!234";

        //        var _admin = await UserManager.FindByEmailAsync("Admin@admin.be");
        //        var _behandelaar = await UserManager.FindByEmailAsync("Behandelaar@behandelaar.be");
        //        var _user = await UserManager.FindByEmailAsync("User@user.be");


        //        if (_admin == null)
        //        {
        //            var createPowerUser = await UserManager.CreateAsync(admin, adminPWD);
        //            if (createPowerUser.Succeeded)
        //            {
        //                //aan rol toevoegen
        //                await UserManager.AddToRoleAsync(admin, SD.AdminEndUser);
        //            }
        //        }
        //        if (_behandelaar == null)
        //        {
        //            var createPowerUser = await UserManager.CreateAsync(Behandelaar, behandelaarPWD);
        //            if (createPowerUser.Succeeded)
        //            {
        //                //aan rol toevoegen
        //                await UserManager.AddToRoleAsync(admin, SD.AdminEndUser);
        //            }
        //        }
        //        if (_user == null)
        //        {
        //            var createPowerUser = await UserManager.CreateAsync(user, userPWD);
        //            if (createPowerUser.Succeeded)
        //            {
        //                //aan rol toevoegen
        //                await UserManager.AddToRoleAsync(admin, SD.AdminEndUser);
        //            }
        //        }
        //    }        
    }
}

