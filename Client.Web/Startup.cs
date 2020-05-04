using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client.Web
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
            services.AddRazorPages();
            services.AddAuthentication(o =>
            {
                o.DefaultScheme =
                CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme =
                OpenIdConnectDefaults.AuthenticationScheme;
            })
               .AddCookie()
               .AddOpenIdConnect(options =>
               {
                   options.Authority = "http://localhost:5000";
                   options.RequireHttpsMetadata = false; // DON'T DO THIS IN PROD
                   options.ClientId = "mvc";
                   //Store in application secrets
                   options.ClientSecret = "secret";
                   options.CallbackPath = "/signin-oidc";
                   options.Scope.Add("api1");
                   options.SignedOutRedirectUri = "/Privacy";
                   options.SaveTokens = true;

                   options.GetClaimsFromUserInfoEndpoint = true;

                   options.ResponseType = "code";
                   options.ResponseMode = "form_post";

                   options.UsePkce = true;
               });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
