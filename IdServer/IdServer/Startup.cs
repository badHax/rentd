using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdServer
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
            //should replace "TestUsers" with an actual client store like a db
            services.AddIdentityServer()/*.AddTestUsers(TestUsers);*/

            //This code should be in client
            //
            //services.AddAuthentication()
            //    .AddGoogle(googleOptions =>
            //    {
            //        googleOptions.ClientId = "";
            //        googleOptions.ClientSecret = "";
            //        googleOptions.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //    })
            //    .AddFacebook(facebookOptions =>
            //    {
            //        facebookOptions.ClientSecret = "";
            //        facebookOptions.ClientId = "";
            //        facebookOptions.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //    })
            //    .AddOpenIdConnect(openIdConnectOptions => {
            //        openIdConnectOptions.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //        openIdConnectOptions.Authority = "oidc";
            //        openIdConnectOptions.ClientId = "our mvc app id";
            //        openIdConnectOptions.ClientSecret = "our app secret"
            //    });
            services.AddMvc();
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
