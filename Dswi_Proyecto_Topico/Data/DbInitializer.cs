using Dswi_Proyecto_Topico.Helpers;
using Dswi_Proyecto_Topico.Models.Entitties;

namespace Dswi_Proyecto_Topico.Data
{
   
    
        public static class DbInitializer
        {
            public static void Initialize(TopicoDbContext context)
            {
                // 🟢 CAMBIO CRÍTICO: NO usar EnsureCreated() - usar Migrate() en Program.cs

                // Verificar si ya existen roles
                if (context.Roles.Any())
                {
                    return; // La BD ya tiene datos
                }

                // Crear Roles
                var roles = new Rol[]
                {
                new Rol { NombreRol = "Administrador", Descripcion = "Acceso total al sistema", Activo = true },
                new Rol { NombreRol = "Enfermera", Descripcion = "Gestión de citas y recetas", Activo = true },
                new Rol { NombreRol = "Paciente", Descripcion = "Consulta de información personal", Activo = true }
                };

                context.Roles.AddRange(roles);
                context.SaveChanges();

                // Crear Categorías de Productos
                var categorias = new CategoriaProducto[]
                {
                new CategoriaProducto { NombreCategoria = "Analgésicos", Descripcion = "Medicamentos para el dolor", TipoCategoria = "Medicamento", Activo = true },
                new CategoriaProducto { NombreCategoria = "Antibióticos", Descripcion = "Medicamentos antibacterianos", TipoCategoria = "Medicamento", Activo = true },
                new CategoriaProducto { NombreCategoria = "Material de Curación", Descripcion = "Gasas, vendas, etc.", TipoCategoria = "Implemento", Activo = true },
                new CategoriaProducto { NombreCategoria = "Equipos Médicos", Descripcion = "Termómetros, tensiómetros", TipoCategoria = "Implemento", Activo = true }
                };

                context.CategoriasProducto.AddRange(categorias);
                context.SaveChanges();

                // Crear Usuarios de Prueba
                var usuarios = new Usuario[]
                {
                new Usuario
                {
                    RolId = 1,
                    NombreCompleto = "Juan Pérez Admin",
                    DNI = "12345678",
                    Email = "admin@topico.com",
                    Telefono = "999888777",
                    NombreUsuario = "admin",
                    PasswordHash = PasswordHelper.HashPassword("admin123"),
                    FechaNacimiento = new DateTime(1985, 3, 15),
                    Direccion = "Av. Principal 123, Lima",
                    FechaRegistro = DateTime.Now,
                    Activo = true,
                    DebecambiarContraseña = false
                },
                new Usuario
                {
                    RolId = 2,
                    NombreCompleto = "María Rodríguez García",
                    DNI = "87654321",
                    Email = "enfermera@topico.com",
                    Telefono = "987654321",
                    NombreUsuario = "enfermera",
                    PasswordHash = PasswordHelper.HashPassword("enfermera123"),
                    FechaNacimiento = new DateTime(1988, 5, 20),
                    Direccion = "Av. Salud 456, Lima",
                    FechaRegistro = DateTime.Now,
                    Activo = true,
                    DebecambiarContraseña = false
                },
                new Usuario
                {
                    RolId = 3,
                    NombreCompleto = "Carlos López Mendoza",
                    DNI = "45678912",
                    Email = "paciente@topico.com",
                    Telefono = "912345678",
                    NombreUsuario = "paciente",
                    PasswordHash = PasswordHelper.HashPassword("paciente123"),
                    FechaNacimiento = new DateTime(1995, 8, 10),
                    Direccion = "Jr. Los Olivos 789, Lima",
                    FechaRegistro = DateTime.Now,
                    Activo = true,
                    DebecambiarContraseña = false
                }
                };

                context.Usuarios.AddRange(usuarios);
                context.SaveChanges();

                // Crear Proveedores de Prueba
                var proveedores = new Proveedor[]
                {
                new Proveedor
                {
                    NombreProveedor = "Farmacéutica del Pacífico S.A.",
                    RUC = "20123456789",
                    Telefono = "014567890",
                    Email = "ventas@farmapacifico.com",
                    Direccion = "Av. Industrial 456, Lima",
                    ContactoNombre = "Roberto García",
                    Activo = true
                },
                new Proveedor
                {
                    NombreProveedor = "Distribuidora Médica Nacional",
                    RUC = "20987654321",
                    Telefono = "017654321",
                    Email = "contacto@dismednal.com",
                    Direccion = "Jr. Comercio 789, Lima",
                    ContactoNombre = "Ana Torres",
                    Activo = true
                }
                };

                context.Proveedores.AddRange(proveedores);
                context.SaveChanges();

                // Crear Productos de Prueba
                var productos = new Producto[]
                {
                new Producto
                {
                    CategoriaId = 1,
                    CodigoProducto = "MED-001",
                    NombreProducto = "Paracetamol 500mg",
                    Descripcion = "Analgésico y antipirético",
                    TipoProducto = "Medicamento",
                    UnidadMedida = "Tableta",
                    StockActual = 500,
                    StockMinimo = 100,
                    PrecioUnitario = 0.50m,
                    Laboratorio = "Farmindustria",
                    RequiereReceta = false,
                    Activo = true
                },
                new Producto
                {
                    CategoriaId = 2,
                    CodigoProducto = "MED-002",
                    NombreProducto = "Amoxicilina 500mg",
                    Descripcion = "Antibiótico de amplio espectro",
                    TipoProducto = "Medicamento",
                    UnidadMedida = "Cápsula",
                    StockActual = 300,
                    StockMinimo = 80,
                    PrecioUnitario = 1.20m,
                    Laboratorio = "Laboratorios Unidos",
                    RequiereReceta = true,
                    Activo = true
                },
                new Producto
                {
                    CategoriaId = 3,
                    CodigoProducto = "IMP-001",
                    NombreProducto = "Gasa Estéril 10x10cm",
                    Descripcion = "Material de curación estéril",
                    TipoProducto = "Implemento",
                    UnidadMedida = "Unidad",
                    StockActual = 1000,
                    StockMinimo = 200,
                    PrecioUnitario = 0.30m,
                    Activo = true
                }
                };

                context.Productos.AddRange(productos);
                context.SaveChanges();
            }
        }

    
}
