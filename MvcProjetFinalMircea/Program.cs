using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcProjetFinalMircea.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

await DbInitializer.SeedRoles(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await SeedUsersAndRoles(app.Services);

app.Run();


async Task SeedUsersAndRoles(IServiceProvider services)
{
    using var scope = services.CreateScope();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // ---- 1. Créer les rôles ----
    string[] roles = { "Admin", "Auteur" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // ---- 2. Créer administrateur ----
    string adminEmail = "admin@site.com";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);

        if (createAdmin.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // ---- 3. Créer Auteur ----
    string authorEmail = "auteur@site.com";
    string authorPassword = "Auteur123!";

    var authorUser = await userManager.FindByEmailAsync(authorEmail);

    if (authorUser == null)
    {
        authorUser = new IdentityUser
        {
            UserName = authorEmail,
            Email = authorEmail,
            EmailConfirmed = true
        };

        var createAuthor = await userManager.CreateAsync(authorUser, authorPassword);

        if (createAuthor.Succeeded)
            await userManager.AddToRoleAsync(authorUser, "Auteur");
    }
}