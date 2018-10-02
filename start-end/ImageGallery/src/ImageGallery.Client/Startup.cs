using IdentityModel;
using ImageGallery.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ImageGallery.Client
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //To get the claim type as it is without changing name. like given_name, family_name
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); //System.IdentityModel.Tokens.Jwt
        }
 
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // register an IImageGalleryHttpClient
            services.AddScoped<IImageGalleryHttpClient, ImageGalleryHttpClient>();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = "Cookies";
                o.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies", 
            (o) => 
                { o.AccessDeniedPath = "/Authorization/AccessDenied";
                }
            )
            .AddOpenIdConnect("oidc", options => {
                options.SignInScheme = "Cookies";
                options.Authority = "https://localhost:44365";
                options.ClientId = "imagegalleryclient";
                options.ResponseType = "code id_token";
                //options.CallbackPath = new PathString("...");
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("roles");

                //So the Scope is in access token, which is included in request to the user info end point
                options.Scope.Add("address");

                options.SaveTokens = true;
                options.ClientSecret = "secret";
                options.GetClaimsFromUserInfoEndpoint = true;

                //To remove the filter for amr so we can get amr in Id token
                options.ClaimActions.Remove("amr");

                //These will not be in identity token (claims identity), we will get it by  userinfo end point.
                options.ClaimActions.DeleteClaim("sid");
                options.ClaimActions.DeleteClaim("idp");
                options.ClaimActions.DeleteClaim("address");

                options.ClaimActions.MapUniqueJsonKey("role","role"); // to add role in identityclaim (identity token)

                //This is to make work 'User.IsInRole("PayingUser")'
                options.TokenValidationParameters = new TokenValidationParameters() {
                    NameClaimType = JwtClaimTypes.GivenName,
                    RoleClaimType = JwtClaimTypes.Role
                };

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }
    } 
}
