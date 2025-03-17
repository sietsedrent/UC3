using Microsoft.EntityFrameworkCore;
using UC3.Data;
using UC3.Business;
using UC3.Controllers;
using UC3.Services;

namespace UC3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<WorkoutContext>(x => x.UseSqlite(connectionString));
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<HomeService>();
            builder.Services.AddScoped<WorkoutService>();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddHttpClient<AccountController>(options =>
            {
                options.BaseAddress = new Uri("https://localhost:7205");
            });
            builder.Services.AddMvc().AddNToastNotifyToastr();
            builder.Services.AddScoped<EmailService>();

            builder.Services.AddDistributedMemoryCache(); // Dit configureert een tijdelijke cache in het geheugen
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(180); // Stelt een timeout in voor sessies (optioneel)
                options.Cookie.HttpOnly = true; // De cookie is alleen toegankelijk via HTTP (meer veiligheid)
                options.Cookie.IsEssential = true; // Nodig voor de werking van de sessie
            });
            builder.Services.AddHttpContextAccessor();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            app.UseNToastNotify();

            

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WorkoutContext>();
                context.Database.Migrate(); // Of gebruik context.Database.EnsureCreated(); voor een eenvoudige setup
            }
            app.Run();
        }
    }
}
