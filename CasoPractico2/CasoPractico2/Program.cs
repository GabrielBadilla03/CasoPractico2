using CasoPractico2.Data;
using CasoPractico2.NoEmailSender;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//prueba
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailSender, NoEmailSender>();

var app = builder.Build();

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

app.MapGet("/api/events", async (ApplicationDbContext db) =>
{
    var eventos = await db.Eventos
        .Select(e => new
        {
            e.Titulo,
            e.Descripcion,
            Categoria = e.Categoria != null ? e.Categoria.Nombre : null,
            e.Fecha,
            e.DuracionMinutos,
            e.Ubicacion,
            e.CupoMaximo
        })
        .ToListAsync();

    return Results.Ok(eventos);
});

app.MapGet("/api/events/{id:int}", async (int id, ApplicationDbContext db) =>
{
    var evento = await db.Eventos
        .Where(e => e.Id == id)
        .Select(e => new
        {
            e.Id,
            e.Titulo,
            e.Descripcion,
            Categoria = e.Categoria != null ? e.Categoria.Nombre : null,
            e.Fecha,
            e.DuracionMinutos,
            e.Ubicacion,
            e.CupoMaximo,
            e.FechaRegistro
        })
        .FirstOrDefaultAsync();

    return evento != null ? Results.Ok(evento) : Results.NotFound();
});

app.Run();
