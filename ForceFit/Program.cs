using ForceFit.Models;
using ForceFit.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = false;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Passo 2 - Serviço de autenticação fake
builder.Services.AddSingleton<FakeAuthService>();

// ✅ Passo 5 - Autenticação via cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login"; // redireciona para o login se não autenticado
    });

// Entity Framework e Identity (caso use futuramente com banco)
builder.Services.AddDbContext<Context>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        })
        .ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    SeedData.semearDados(app);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Ativa autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Public}/{id?}");
app.MapControllerRoute(
    name: "aluno", pattern: "Aluno/{action=Perfil}/{id?}");

// Banco opcional
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<Context>();
    context.Database.EnsureCreated();

    // Obtém o UserManager e cria as roles
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // Chama o método para criar as roles
    await CriarRoles(services, userManager);

    await SeedData.Initialize(services, userManager);
}

app.Run();

// Método que cria as roles no banco de dados, se não existirem
async Task CriarRoles(IServiceProvider services, UserManager<IdentityUser> userManager)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Cria as roles "Aluno" e "Personal" se não existirem
    string[] roles = new string[] { "Aluno", "Personal" };

    foreach (var role in roles)
    {
        var roleExist = await roleManager.RoleExistsAsync(role);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
