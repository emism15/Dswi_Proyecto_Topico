using Microsoft.EntityFrameworkCore;
using TopicoMedico.Data;
using TopicoMedico.Services;
using TopicoMedico.Services.Interfaces;

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

// 🟢 INICIALIZAR DATOS DE PRUEBA (SOLO SI LA BD ESTÁ VACÍA)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TopicoDbContext>();

        // 🟢 APLICAR MIGRACIONES PENDIENTES AUTOMÁTICAMENTE
        context.Database.Migrate();

        // 🟢 INSERTAR DATOS INICIALES
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

app.Run();