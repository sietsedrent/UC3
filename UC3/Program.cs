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
            builder.Services.AddScoped<IWorkoutContext>(provider =>
    provider.GetRequiredService<WorkoutContext>());

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
            builder.Services.AddSignalR();
            builder.Services.AddDistributedMemoryCache(); 
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(180);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; 
            });
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            app.UseNToastNotify();



            // Configure HTTPpipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.MapHub<WorkoutHub>("/workoutHub");

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WorkoutContext>();
                context.Database.Migrate();
                app.Run();
            }
        }
    }
}
