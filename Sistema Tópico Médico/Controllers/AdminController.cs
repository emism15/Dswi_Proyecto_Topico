using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopicoMedico.Data;
using TopicoMedico.Filters;
using TopicoMedico.Helpers;
using TopicoMedico.Models;
using TopicoMedico.Models.ViewModels;
using TopicoMedico.Services;
using TopicoMedico.Services.Interfaces;

namespace TopicoMedico.Controllers
{


    // ========== AdminController - Panel del Administrador ==========
    [AuthorizeRole("Administrador")]
    public class AdminController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService;
        private readonly ICompraService _compraService;
        private readonly IAlertaService _alertaService;
        private readonly TopicoDbContext _context;

        public AdminController(
            IUsuarioService usuarioService,
            IProductoService productoService,
            ICompraService compraService,
            IAlertaService alertaService,
            TopicoDbContext context)
        {
            _usuarioService = usuarioService;
            _productoService = productoService;
            _compraService = compraService;
            _alertaService = alertaService;
            _context = context;
        }

        // GET: Admin/Index - Dashboard Principal
        public async Task<IActionResult> Index()
        {
            // 1️⃣ Generar alertas automáticas
            await _alertaService.GenerarAlertasStockAsync();

            // 2️⃣ Obtener alertas YA GENERADAS
            var alertasAdmin = await _alertaService.ObtenerPorRolAsync("Administrador");

            var viewModel = new DashboardAdminViewModel
            {
                TotalUsuarios = _context.Usuarios.Count(u => u.Activo),
                TotalPacientes = _context.Usuarios.Count(u => u.RolId == 3 && u.Activo),
                TotalEnfermeras = _context.Usuarios.Count(u => u.RolId == 2 && u.Activo),
                ProductosStockBajo = (await _productoService.ObtenerConStockBajoAsync()).Count,
                ProductosPorVencer = (await _productoService.ObtenerPorVencerAsync(30)).Count,
                MontoComprasMes = await _compraService.ObtenerMontoTotalMesAsync(),
                AlertasCriticas = alertasAdmin,
                ProductosAlerta = await _productoService.ObtenerConStockBajoAsync()
            };

            return View(viewModel);
        }


        // ===== GESTIÓN DE USUARIOS =====

        // GET: Admin/Usuarios
        public async Task<IActionResult> Usuarios()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();
            return View(usuarios);
        }

        // GET: Admin/CrearUsuario
        public IActionResult CrearUsuario()
        {
            var model = new UsuarioViewModel
            {
                RolesDisponibles = _context.Roles.Where(r => r.Activo).ToList()
            };
            return View(model);
        }

        // POST: Admin/CrearUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario(UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RolesDisponibles = _context.Roles.Where(r => r.Activo).ToList();
                return View(model);
            }

            // Validar usuario único
            if (await _usuarioService.ExisteUsuarioAsync(model.NombreUsuario))
            {
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya existe");
                model.RolesDisponibles = _context.Roles.Where(r => r.Activo).ToList();
                return View(model);
            }

            // Validar DNI único
            if (await _usuarioService.ExisteDNIAsync(model.DNI))
            {
                ModelState.AddModelError("DNI", "El DNI ya está registrado");
                model.RolesDisponibles = _context.Roles.Where(r => r.Activo).ToList();
                return View(model);
            }

            var usuarioCreadorId = HttpContext.Session.GetUsuarioId().Value;
            var resultado = await _usuarioService.CrearAsync(model, usuarioCreadorId);

            if (resultado)
            {
                TempData["Success"] = "Usuario creado exitosamente";
                return RedirectToAction("Usuarios");
            }

            ModelState.AddModelError("", "Error al crear el usuario");
            model.RolesDisponibles = _context.Roles.Where(r => r.Activo).ToList();
            return View(model);
        }

        // GET: Admin/EditarUsuario/5
        public async Task<IActionResult> EditarUsuario(int id)
        {
            var usuario = await _usuarioService.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound();

            var model = new UsuarioViewModel
            {
                UsuarioId = usuario.UsuarioId,
                RolId = usuario.RolId,
                NombreCompleto = usuario.NombreCompleto,
                DNI = usuario.DNI,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                NombreUsuario = usuario.NombreUsuario,
                FechaNacimiento = usuario.FechaNacimiento,
                Direccion = usuario.Direccion,
                Activo = usuario.Activo,
                RolesDisponibles = _context.Roles.Where(r => r.Activo).ToList()
            };

            return View(model);
        }

        // POST: Admin/EditarUsuario/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(int id, UsuarioViewModel model)
        {
            if (id != model.UsuarioId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.RolesDisponibles = _context.Roles.Where(r => r.Activo).ToList();
                return View(model);
            }

            var resultado = await _usuarioService.ActualizarAsync(model);

            if (resultado)
            {
                TempData["Success"] = "Usuario actualizado exitosamente";
                return RedirectToAction("Usuarios");
            }

            ModelState.AddModelError("", "Error al actualizar el usuario");
            model.RolesDisponibles = _context.Roles.Where(r => r.Activo).ToList();
            return View(model);
        }

        // POST: Admin/EliminarUsuario/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var resultado = await _usuarioService.EliminarAsync(id);

            if (resultado)
                TempData["Success"] = "Usuario eliminado exitosamente";
            else
                TempData["Error"] = "Error al eliminar el usuario";

            return RedirectToAction("Usuarios");
        }

        // ===== GESTIÓN DE PRODUCTOS =====

        // GET: Admin/Productos
        public async Task<IActionResult> Productos()
        {
            var productos = await _productoService.ObtenerTodosAsync();
            return View(productos);
        }

        // GET: Admin/CrearProducto
        public IActionResult CrearProducto()
        {
            var model = new ProductoViewModel
            {
                CategoriasDisponibles = _context.CategoriasProducto.Where(c => c.Activo).ToList()
            };
            return View(model);
        }

        // POST: Admin/CrearProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProducto(ProductoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoriasDisponibles = _context.CategoriasProducto.Where(c => c.Activo).ToList();
                return View(model);
            }

            var resultado = await _productoService.CrearAsync(model);

            if (resultado)
            {
                TempData["Success"] = "Producto creado exitosamente";
                return RedirectToAction("Productos");
            }

            ModelState.AddModelError("", "Error al crear el producto");
            model.CategoriasDisponibles = _context.CategoriasProducto.Where(c => c.Activo).ToList();
            return View(model);
        }

        // GET: Admin/EditarProducto/5
        public async Task<IActionResult> EditarProducto(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
                return NotFound();

            var model = new ProductoViewModel
            {
                ProductoId = producto.ProductoId,
                CategoriaId = producto.CategoriaId,
                CodigoProducto = producto.CodigoProducto,
                NombreProducto = producto.NombreProducto,
                Descripcion = producto.Descripcion,
                TipoProducto = producto.TipoProducto,
                UnidadMedida = producto.UnidadMedida,
                StockActual = producto.StockActual,
                StockMinimo = producto.StockMinimo,
                PrecioUnitario = producto.PrecioUnitario,
                FechaVencimiento = producto.FechaVencimiento,
                Laboratorio = producto.Laboratorio,
                Lote = producto.Lote,
                RequiereReceta = producto.RequiereReceta,
                Activo = producto.Activo,
                CategoriasDisponibles = _context.CategoriasProducto.Where(c => c.Activo).ToList()
            };

            return View(model);
        }

        // POST: Admin/EditarProducto/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProducto(int id, ProductoViewModel model)
        {
            if (id != model.ProductoId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.CategoriasDisponibles = _context.CategoriasProducto.Where(c => c.Activo).ToList();
                return View(model);
            }

            var resultado = await _productoService.ActualizarAsync(model);

            if (resultado)
            {
                TempData["Success"] = "Producto actualizado exitosamente";
                return RedirectToAction("Productos");
            }

            ModelState.AddModelError("", "Error al actualizar el producto");
            model.CategoriasDisponibles = _context.CategoriasProducto.Where(c => c.Activo).ToList();
            return View(model);
        }

        // ===== GESTIÓN DE COMPRAS =====

        // GET: Admin/Compras
        public async Task<IActionResult> Compras()
        {
            var compras = await _compraService.ObtenerTodasAsync();
            return View(compras);
        }

        // GET: Admin/CrearCompra
        public async Task<IActionResult> CrearCompra()
        {
            var model = new CompraViewModel
            {
                ProveedoresDisponibles = _context.Proveedores.Where(p => p.Activo).ToList(),
                ProductosDisponibles = await _productoService.ObtenerActivosAsync(),
                FechaCompra = DateTime.Now
            };
            return View(model);
        }

        // TopicoMedico.Controllers/AdminController.cs (Fragmento corregido)

        // ... resto del Controller ...

        // POST: Admin/CrearCompra
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCompra(CompraViewModel model)
        {
            // Validación SOLO de campos principales
            if (model.ProveedorId <= 0 ||
                string.IsNullOrEmpty(model.NumeroComprobante) ||
                string.IsNullOrEmpty(model.TipoComprobante))
            {
                ModelState.AddModelError("", "Complete los datos principales de la compra");

                model.ProveedoresDisponibles = _context.Proveedores.Where(p => p.Activo).ToList();
                model.ProductosDisponibles = await _productoService.ObtenerActivosAsync();
                return View(model);
            }


            // 2. Validar que la lista de detalles NO esté vacía (sólo si los campos principales son válidos)
            if (model.Detalles == null || !model.Detalles.Any())
            {
                // Agrega el error específico sobre la lista, pero solo si los campos principales eran OK
                ModelState.AddModelError(string.Empty, "Debe agregar al menos un producto a la compra");

                // Recargar las listas antes de retornar la vista
                model.ProveedoresDisponibles = _context.Proveedores.Where(p => p.Activo).ToList();
                model.ProductosDisponibles = await _productoService.ObtenerActivosAsync();
                return View(model);
            }

            // 3. Lógica de registro (Solo se ejecuta si ModelState.IsValid y Detalles NO está vacío)

            // Obtener el ID del usuario logueado (CLAVE: Necesitas tu helper GetUsuarioId())
            var usuarioId = HttpContext.Session.GetUsuarioId().Value;

            // Recalcular monto total en el servidor (medida de seguridad)
            model.MontoTotal = model.Detalles.Sum(d => d.Subtotal);

            var resultado = await _compraService.CrearAsync(model, usuarioId);

            if (resultado)
            {
                TempData["Success"] = "Compra registrada exitosamente. Stock actualizado.";
                return RedirectToAction("Compras");
            }

            // 4. Manejo de error del servicio/BD
            ModelState.AddModelError(string.Empty, "Error al registrar la compra en la base de datos. Intente nuevamente.");

            // Recargar las listas antes de retornar la vista
            model.ProveedoresDisponibles = _context.Proveedores.Where(p => p.Activo).ToList();
            model.ProductosDisponibles = await _productoService.ObtenerActivosAsync();
            return View(model);
        }

        // GET: Admin/DetalleCompra/5
        public async Task<IActionResult> DetalleCompra(int id)
        {
            var compra = await _compraService.ObtenerPorIdAsync(id);
            if (compra == null)
                return NotFound();

            return View(compra);
        }

        // ===== REPORTES =====

        // GET: Admin/ReporteCompras
        public IActionResult ReporteCompras()
        {
            var model = new ReporteComprasViewModel
            {
                FechaInicio = DateTime.Now.AddMonths(-1),
                FechaFin = DateTime.Now,
                Proveedores = _context.Proveedores.Where(p => p.Activo).ToList()
            };
            return View(model);
        }

        // POST: Admin/ReporteCompras
        [HttpPost]
        public async Task<IActionResult> ReporteCompras(ReporteComprasViewModel model)
        {
            model.Compras = await _compraService.ObtenerPorFechasAsync(model.FechaInicio, model.FechaFin);

            if (model.ProveedorId.HasValue)
                model.Compras = model.Compras.Where(c => c.ProveedorId == model.ProveedorId.Value).ToList();

            model.MontoTotal = model.Compras.Sum(c => c.MontoTotal);
            model.Proveedores = _context.Proveedores.Where(p => p.Activo).ToList();

            return View(model);
        }

        // GET: Admin/Alertas
        public async Task<IActionResult> Alertas()
        {
            await _alertaService.GenerarAlertasStockAsync();
            var alertas = await _alertaService.ObtenerPorRolAsync("Administrador");
            return View(alertas);
        }


        // POST: Admin/MarcarAlertaLeida/5
        [HttpPost]
        public async Task<IActionResult> MarcarAlertaLeida(int id)
        {
            await _alertaService.MarcarComoLeidaAsync(id);
            return RedirectToAction("Alertas");
        }
    }
}