using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Services;
using Dswi_Proyecto_Topico.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Configurar DbContext con SQL Server
builder.Services.AddDbContext<TopicoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar sesiones para autenticación
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registrar servicios personalizados
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<IRecetaService, RecetaService>();
builder.Services.AddScoped<ICompraService, CompraService>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<AtencionRepository>();
builder.Services.AddScoped<AlumnoRepository>();

// Configurar HttpContextAccessor para acceder al contexto HTTP
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Habilitar sesiones
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TopicoDbContext>();
    DbInitializer.Initialize(context);
}
app.Run();
