﻿using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdServer.DB;
using IdServer.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace IdServer
{
    public class Startup
    {
        IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = configuration.Build();

            var levelSwitch = new LoggingLevelSwitch();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.File("C:\\logs\\rentd\\log.txt")
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //add my app settings to the runtime
            // Add our Config object so it can be injected
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //add sql database
            services
                .AddDbContext<IdContext>(o => o
                    .UseSqlServer(Configuration["Data:IdContext:ConnectionString"])
            );

            //add identity(user account management framework)
            services.AddIdentity<User, IdentityRole>(o => 
            {
                o.Password.RequireDigit = false;
                o.Password.RequiredLength = 8;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<IdContext>()
            .AddDefaultTokenProviders();

            //configure identityserver4 framework
            services
                .AddIdentityServer(o =>
                {
                    o.UserInteraction.LogoutUrl = "/logout";
                    o.UserInteraction.LoginUrl = "/login";
                })
                .AddConfigurationStore(o =>
                o.ConfigureDbContext(new DbContextOptionsBuilder()
                        .UseSqlServer(Configuration["Data:IdContext:ConnectionString"])))
                .AddOperationalStore(o => 
                    o.ConfigureDbContext(new DbContextOptionsBuilder()
                        .UseSqlServer(Configuration["Data:IdContext:ConnectionString"])))
                .AddSigningCredential(new LoadCert().LoadCertificate(_env))
                .AddAspNetIdentity<User>();
            services.AddMvc();

            //get allowed user profiles
            services.AddScoped<IProfileService, RentdProfileServie>();

            //get allwoed client (api apps) profiles
            services.AddScoped<IClientStore,RentdClientStore>();

            //authorize endpoints that only apis are allowed
            services.AddAuthorization(o => {
                o.AddPolicy("WebAPI1", policy => policy.RequireClaim("scope","RENTDAPI1"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
