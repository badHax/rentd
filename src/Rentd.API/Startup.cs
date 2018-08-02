/////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rentd.Data;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.IdentityModel.Tokens.Jwt;
using System.IO;

/////////////////////////////////////////////////////////////////////////////////////////


namespace Rentd.API
{
    public class Startup
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        IHostingEnvironment _env;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Startup(IHostingEnvironment env)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings{env.EnvironmentName}.json", optional : true, reloadOnChange : true);

            Configuration = config.Build();

            var levelSwitch = new LoggingLevelSwitch();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.File(Path.Combine(env.ContentRootPath,"logs\\log.txt"))
                .CreateLogger();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public IConfiguration Configuration { get; }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //add database connection
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

            //clear all inbound claims
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //Add Jwt Authentication to the DI
            services.AddAuthentication(options => {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwtOptions => {
                    jwtOptions.Audience = "";
                    jwtOptions.Authority = "";
                });

            services.AddMvc(options => {
                options.Filters.Add(new GlobalExceptionFilter(Log.Logger.ForContext<GlobalExceptionFilter>()));
            });

            //authorize endpoints that only apis are allowed
            services.AddAuthorization(o => {
                o.AddPolicy("WebAPI1", policy => policy.RequireClaim("scope", "RENTDAPI1"));
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
