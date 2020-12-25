using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PISBusinessLogic.BusinessLogic;
using PISBusinessLogic.HelperModels;
using PISBusinessLogic.Interfaces;
using PISDatabaseImplement.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PISCourseworkARMAccountant
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
            services.AddControllersWithViews();
            services.AddTransient<IUserLogic, UserLogic>();
            services.AddTransient<IBookLogic, BookLogic>();
            services.AddTransient<IGenreLogic, GenreLogic>();
            services.AddTransient<ILibraryCardLogic, LibraryCardLogic>();
            services.AddTransient<IContractLogic, ContractLogic>();
            services.AddTransient<IBookingLogic, BookingLogic>();
            services.AddTransient<IPaymentLogic, PaymentLogic>();
            services.AddTransient<ArchiveLogic>();
            services.AddTransient<ReportLogic>();
            services.AddTransient<EncryptionLogic>();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                   name: "Librarian",
                   pattern: "Librarian/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}