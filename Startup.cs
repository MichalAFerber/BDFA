using BDFA.BL;
using BDFA.Data;
using Microsoft.EntityFrameworkCore;

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

        //services.AddDbContext<SiteAdminContext>(options =>
        //        options.UseSqlite(Configuration.GetConnectionString("SiteAdminContext")));

        //services.AddDbContext<SiteSettingsContext>(options =>
        //        options.UseSqlite(Configuration.GetConnectionString("SiteSettingsContext")));

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
        });
    }
}