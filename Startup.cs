using BDFA.BL;
using BDFA.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BDFA.Models;

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
        services.AddRouting(); // Add routing services to the container.
        services.AddDistributedMemoryCache(); // Stores session in-memory

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout.
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddRazorPages(); // Add services to the container.

        services.AddDbContext<DirectoryContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DirectoryContext")));

        services.AddControllers();

        Manager.InitializeSMTPSettings(Configuration);
        Manager.InitializeDBSettings(Configuration);
        Manager.InitializeSiteAdmin();
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
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseSession(); // Enable session

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
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var clickData = JsonSerializer.Deserialize<ClickDataJSON>(body);

                using (var dbContext = app.ApplicationServices.GetRequiredService<DirectoryContext>())
                {
                    dbContext.Clicks.Add(new ClickData { ProfileId = clickData.ProfileId.ToString(), Link = clickData.Link, ClickDateTime = DateTime.Parse(clickData.ClickDateTime) });
                    await dbContext.SaveChangesAsync();
                }

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