using ClientesApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Banco de Dados
builder.Services.AddDbContext<ClientesContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ClientesDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ClientesDb"))
    )
);

// Sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Autenticação por Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddSession();


var app = builder.Build();

// Erros
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// pipeline
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Rotas
app.MapControllerRoute(
    name: "default",
   // pattern: "{controller=Login}/{action=Index}/{id?}");
    pattern: "{controller=Auth}/{action=Login}/{id?}");


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Clientes}/{action=Index}/{id?}");

app.Run();
