/////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Rentd.Data;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Buffers;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;

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
            services.AddIdentity<User,IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequiredLength = 8;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireDigit = false;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdContext>()
            .AddDefaultTokenProviders();

            //clear all inbound claims
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //Add Jwt Authentication to the DI
            services
                .AddAuthentication(options => {
                    options.DefaultScheme =
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                //.AddOpenIdConnect(oidcOpts => {
                //    oidcOpts.ClientId = "";
                //    oidcOpts.ClientSecret = "";
                //    oidcOpts.SignInScheme = "Cookies";
                //    oidcOpts.Authority = ""; //auth server
                //    oidcOpts.SignedOutRedirectUri = ""; //the webapp
                //    oidcOpts.RequireHttpsMetadata = true;
                //    oidcOpts.SaveTokens = true;
                //    oidcOpts.GetClaimsFromUserInfoEndpoint = true;

                //    oidcOpts.Scope.Clear();
                //    oidcOpts.Scope.Add("profile");
                //    oidcOpts.Scope.Add("RENTDAPI1");
                //    oidcOpts.Scope.Add("offline_access");
                //})
                .AddJwtBearer(jwtOptions => {
                    jwtOptions.Audience = Configuration["AppSettings:BaseUrls:Auth"];
                    jwtOptions.Authority = Configuration["AppSettings:BaseUrls:Auth"]+"/resources";//IDserver
                    jwtOptions.IncludeErrorDetails = true;
                    jwtOptions.RequireHttpsMetadata = false;

                    //jwtOptions.TokenValidationParameters = new TokenValidationParameters() {
                    //    ValidateIssuer = true,
                    //    ValidIssuer = "",
                    //    ValidateAudience = true,
                    //    ValidAudience = "",
                    //    ValidateIssuerSigningKey = true,
                    //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(""))
                    //};
                });

            //authorize endpoints that only apis are allowed
            services.AddAuthorization(o => {
                o.AddPolicy("api1_access", policy => policy.RequireClaim("scope", "RENTDAPI1"));
            });

            //database
            services.AddDbContext<IdContext>();

            //Add mvc services to the DI
            services.AddMvc(options => {
                options.OutputFormatters.Add(new JsonOutputFormatter(new JsonSerializerSettings() {
                    Culture = CultureInfo.InvariantCulture
                }, charPool: ArrayPool<char>.Shared));
                options.Filters.Add(new GlobalExceptionFilter(Log.Logger.ForContext<GlobalExceptionFilter>()));
            });

            
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddSerilog();
            app.UseMvc();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
