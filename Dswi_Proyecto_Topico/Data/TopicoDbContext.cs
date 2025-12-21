using Dswi_Proyecto_Topico.Models.Entitties;
using Microsoft.EntityFrameworkCore;

namespace Dswi_Proyecto_Topico.Data
{
    public class TopicoDbContext : DbContext
    {
        public TopicoDbContext(DbContextOptions<TopicoDbContext> options) : base(options)
        {
        }

        // DbSets - Tablas
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CategoriaProducto> CategoriasProducto { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetallesCompras { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<DetalleReceta> DetallesReceta { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relaciones múltiples Usuario-Cita
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Alumno)
                .WithMany(u => u.CitasComoAlumno)
                .HasForeignKey(c => c.AlumnoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Enfermera)
                .WithMany(u => u.CitasComoEnfermera)
                .HasForeignKey(c => c.EnfermeraId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relaciones múltiples Usuario-Receta
            // Receta → Alumno
            modelBuilder.Entity<Receta>()
                .HasOne(r => r.Alumno)
                .WithMany(a => a.RecetasComoAlumno)
                .HasForeignKey(r => r.AlumnoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Receta → Enfermera (Usuario)
            modelBuilder.Entity<Receta>()
                .HasOne(r => r.Enfermera)
                .WithMany(u => u.RecetasComoEnfermera)
                .HasForeignKey(r => r.EnfermeraId)
                .OnDelete(DeleteBehavior.Restrict);


            // Índices únicos
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.DNI)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario)
                .IsUnique();

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.CodigoProducto)
                .IsUnique();

            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.RUC)
                .IsUnique();

            modelBuilder.Entity<Compra>()
                .HasIndex(c => c.NumeroComprobante)
                .IsUnique();

            // Configurar precisión decimal
            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioUnitario)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Compra>()
                .Property(c => c.MontoTotal)
                .HasPrecision(12, 2);

            modelBuilder.Entity<DetalleCompra>()
                .Property(d => d.PrecioUnitario)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetalleCompra>()
                .Property(d => d.Subtotal)
                .HasPrecision(12, 2);
            modelBuilder.Entity<Alerta>()
            .HasOne(a => a.UsuarioDestino)
            .WithMany()
            .HasForeignKey(a => a.UsuarioDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        }
    }

}
