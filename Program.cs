using EventPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseWebSockets();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                 name: "default",
                  pattern: "{controller=Events}/{action=Index}/{id?}");

            app.MapControllerRoute(
                 name: "guests",
                pattern: "Guests/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "tasks",
                pattern: "Tasks/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "rating",
                pattern: "{controller=Events}/{action=Rate}/{id?}");
            app.MapControllerRoute(
                name: "reports",
                pattern: "{controller=Events}/{action=Reports}/{id?}");

            app.Run();
        }
    }
}
