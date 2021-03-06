using System;
using Epicentre.Areas.Identity.Data;
using Epicentre.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Epicentre.Areas.Identity.IdentityHostingStartup))]
namespace Epicentre.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<EpicentreAuthenticationContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("EpicentreConnection")));

                services.AddDefaultIdentity<EpicentreUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<EpicentreAuthenticationContext>();

                services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromHours(1));
            });
        }
    }
}