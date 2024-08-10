using BDFA.BL;
using BDFA.Data;
using BDFA.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddMemoryCache();

        services.AddRazorPages();

        services.AddDbContext<DirectoryContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DirectoryContext")));

        services.AddControllers();

        Manager.InitializeSMTPSettings(Configuration);
        Manager.InitializeDBSettings(Configuration);
        Manager.InitializeSiteAdmin(Configuration);

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseMiddleware<BDFA.ExceptionMiddleware>();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            endpoints.MapPost("/track-click", async context =>
            {
                using var scope = context.RequestServices.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryContext>();

                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var clickData = JsonSerializer.Deserialize<ClickDataJSON>(body);

                // Validate ClickDateTime is not null or empty
                if (string.IsNullOrWhiteSpace(clickData.ClickDateTime))
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync("ClickDateTime is required.");
                    return;
                }

                DateTime clickDateTime;
                try
                {
                    clickDateTime = DateTime.Parse(clickData.ClickDateTime);
                }
                catch (FormatException)
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync("ClickDateTime is invalid.");
                    return;
                }

                dbContext.Clicks.Add(new ClickData { ProfileId = clickData.ProfileId.ToString(), Link = clickData.Link, ClickDateTime = clickDateTime });
                await dbContext.SaveChangesAsync();

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 200;
            });
        });
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

    public class ClickDataJSON
    {
        public int ProfileId { get; set; }
        public string Link { get; set; }
        public string ClickDateTime { get; set; }
    }
}
