using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using booking_facilities.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
using booking_facilities.Repositories;

namespace booking_facilities
{
    public class Startup
    {
        private readonly IConfiguration config;
        private readonly IConfigurationSection appConfig;
        private readonly IHostingEnvironment environment;

        public Startup(IConfiguration config, IHostingEnvironment environment)
        {
            this.environment = environment;
            this.config = config;
            appConfig = this.config.GetSection("Booking_Facilities");

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddHttpClient("gatekeeper");
            services.AddSingleton<IApiClient, ApiClient>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = appConfig.GetValue<string>("GatekeeperUrl");
                options.ClientId = appConfig.GetValue<string>("ClientId");
                options.ClientSecret = appConfig.GetValue<string>("ClientSecret");
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("profile");
                options.Scope.Add("offline_access");
                options.ClaimActions.MapJsonKey("locale", "locale");
                options.ClaimActions.MapJsonKey("user_type", "user_type");
            }).AddIdentityServerAuthentication("Bearer", options =>
            {
                options.Authority = appConfig.GetValue<string>("GatekeeperUrl");
                options.ApiName = appConfig.GetValue<string>("ApiResourceName");
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("APIPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddAuthenticationSchemes("oidc", "Bearer");
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", pb => pb.RequireClaim("user_type", "administrator"));

            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<booking_facilitiesContext>(options =>
                    options.UseMySql(config.GetConnectionString("booking_facilitiesContext")));
            services.AddScoped<ISportRepository, SportRepository>();
            if (!environment.IsDevelopment())
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    var proxyAddresses = Dns.GetHostAddresses(appConfig.GetValue<string>("ReverseProxyHostname", "http://nginx"));
                    foreach (var ip in proxyAddresses)
                    {
                        options.KnownProxies.Add(ip);
                    };
                });
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                var pathBase = appConfig.GetValue<string>("PathBase", "/booking-facilities");
                RunMigrations(app);
                app.UsePathBase(pathBase);
                app.Use((context, next) =>
                {
                    context.Request.PathBase = new PathString(pathBase);
                    return next();
                });
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Bookings}/{action=Index}/{id?}");
            });
        }

        private void RunMigrations(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var dbContext = serviceProvider.GetService<booking_facilitiesContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
