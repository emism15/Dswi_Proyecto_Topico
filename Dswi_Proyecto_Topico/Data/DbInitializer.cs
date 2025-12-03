using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Helpers;
using Dswi_Proyecto_Topico.Models.Entities;

namespace Dswi_Proyecto_Topico.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TopicoDbContext context)
        {
            context.Database.EnsureCreated();

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

            foreach (var rol in roles)
            {
                context.Roles.Add(rol);
            }
            context.SaveChanges();

            // Crear Usuarios de Prueba
            var usuarios = new Usuario[]
            {
                // Administrador
                new Usuario
                {
                    RolId = 1,
                    NombreCompleto = "Juan Pérez Admin",
                    DNI = "12345678",
                    Email = "admin@topico.com",
                    Telefono = "999888777",
                    NombreUsuario = "admin",
                    Contraseña = PasswordHelper.HashPassword("admin123"),
                    FechaNacimiento = new DateTime(1985, 3, 15),
                    Direccion = "Av. Principal 123, Lima",
                    FechaRegistro = DateTime.Now,
                    Activo = true,
                    DebecambiarContraseña = false
                },
                
                // Enfermera
                new Usuario
                {
                    RolId = 2,
                    NombreCompleto = "María Rodríguez García",
                    DNI = "87654321",
                    Email = "enfermera@topico.com",
                    Telefono = "987654321",
                    NombreUsuario = "enfermera",
                    Contraseña = PasswordHelper.HashPassword("enfermera123"),
                    FechaNacimiento = new DateTime(1988, 5, 20),
                    Direccion = "Av. Salud 456, Lima",
                    FechaRegistro = DateTime.Now,
                    Activo = true,
                    DebecambiarContraseña = false
                },
                
                // Paciente
                new Usuario
                {
                    RolId = 3,
                    NombreCompleto = "Carlos López Mendoza",
                    DNI = "45678912",
                    Email = "paciente@topico.com",
                    Telefono = "912345678",
                    NombreUsuario = "paciente",
                    Contraseña = PasswordHelper.HashPassword("paciente123"),
                    FechaNacimiento = new DateTime(1995, 8, 10),
                    Direccion = "Jr. Los Olivos 789, Lima",
                    FechaRegistro = DateTime.Now,
                    Activo = true,
                    DebecambiarContraseña = false
                }
            };

            foreach (var usuario in usuarios)
            {
                context.Usuarios.Add(usuario);
            }
            context.SaveChanges();
        }
    }
}