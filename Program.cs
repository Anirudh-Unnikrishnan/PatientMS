using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientMS.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PatientMSContextConnection") ?? throw new InvalidOperationException("Connection string 'PatientMSContextConnection' not found.");

builder.Services.AddDbContext<PatientMSContext>(options =>
    options.UseSqlite("Data Source=PatientMS.db"));

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<PatientMSContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed roles and all users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    await SeedRolesAndUsers(roleManager, userManager);
}

app.MapRazorPages();
app.Run();

async Task SeedRolesAndUsers(RoleManager<IdentityRole> roleManager,
    UserManager<IdentityUser> userManager)
{
    // Create all roles
    string[] roles = { "Admin", "Doctor", "Receptionist" };
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    // Seed Admin
    var adminEmail = "admin@patientms.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "Admin@123");
    }
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        await userManager.AddToRoleAsync(adminUser, "Admin");

    // Seed Doctor
    var doctorEmail = "doctor@patientms.com";
    var doctorUser = await userManager.FindByEmailAsync(doctorEmail);
    if (doctorUser == null)
    {
        doctorUser = new IdentityUser
        {
            UserName = doctorEmail,
            Email = doctorEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(doctorUser, "Doctor@123");
    }
    if (!await userManager.IsInRoleAsync(doctorUser, "Doctor"))
        await userManager.AddToRoleAsync(doctorUser, "Doctor");

    // Seed Receptionist
    var receptionistEmail = "receptionist@patientms.com";
    var receptionistUser = await userManager.FindByEmailAsync(receptionistEmail);
    if (receptionistUser == null)
    {
        receptionistUser = new IdentityUser
        {
            UserName = receptionistEmail,
            Email = receptionistEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(receptionistUser, "Receptionist@123");
    }
    if (!await userManager.IsInRoleAsync(receptionistUser, "Receptionist"))
        await userManager.AddToRoleAsync(receptionistUser, "Receptionist");
}