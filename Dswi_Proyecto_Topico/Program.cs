using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Services;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<AlumnoRepository>();
builder.Services.AddScoped<AtencionRepository>();
builder.Services.AddScoped<ReporteAtencionRepository>();
builder.Services.AddScoped<HistorialReporteRepository>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSession();

var app = builder.Build();

RotativaConfiguration.Setup(
    app.Environment.WebRootPath,
    "Rotativa"
);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
