-- =============================================
-- SCRIPT COMPLETO BASE DE DATOS - SISTEMA TÓPICO MÉDICO
-- SQL Server 2019+
-- =============================================

USE master;
GO

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TopicoMedicoDB')
BEGIN
    CREATE DATABASE TopicoMedicoDB;
END
GO

USE TopicoMedicoDB;
GO

-- =============================================
-- TABLAS PRINCIPALES
-- =============================================

-- Tabla de Roles
CREATE TABLE Roles (
    RolId INT PRIMARY KEY IDENTITY(1,1),
    NombreRol NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200),
    FechaCreacion DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1
);

-- Tabla de Usuarios
CREATE TABLE Usuarios (
    UsuarioId INT PRIMARY KEY IDENTITY(1,1),
    RolId INT NOT NULL,
    NombreCompleto NVARCHAR(100) NOT NULL,
    DNI NVARCHAR(20) NOT NULL UNIQUE,
    Email NVARCHAR(100) UNIQUE,
    Telefono NVARCHAR(20),
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    Contraseña NVARCHAR(255) NOT NULL, -- Será hasheada en la aplicación
    FechaNacimiento DATE,
    Direccion NVARCHAR(200),
    FechaRegistro DATETIME DEFAULT GETDATE(),
    UltimoAcceso DATETIME,
    Activo BIT DEFAULT 1,
    DebecambiarContraseña BIT DEFAULT 1,
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolId) REFERENCES Roles(RolId)
);

-- Tabla de Categorías de Productos
CREATE TABLE CategoriasProducto (
    CategoriaId INT PRIMARY KEY IDENTITY(1,1),
    NombreCategoria NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200),
    TipoCategoria NVARCHAR(20) CHECK (TipoCategoria IN ('Medicamento', 'Implemento')),
    Activo BIT DEFAULT 1
);

-- Tabla de Productos (Medicamentos e Implementos)
CREATE TABLE Productos (
    ProductoId INT PRIMARY KEY IDENTITY(1,1),
    CategoriaId INT NOT NULL,
    CodigoProducto NVARCHAR(50) UNIQUE NOT NULL,
    NombreProducto NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(300),
    TipoProducto NVARCHAR(20) CHECK (TipoProducto IN ('Medicamento', 'Implemento')),
    UnidadMedida NVARCHAR(30),
    StockActual INT DEFAULT 0,
    StockMinimo INT DEFAULT 10,
    PrecioUnitario DECIMAL(10,2),
    FechaVencimiento DATE,
    Laboratorio NVARCHAR(100),
    Lote NVARCHAR(50),
    RequiereReceta BIT DEFAULT 0,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1,
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId) REFERENCES CategoriasProducto(CategoriaId)
);

-- Tabla de Proveedores
CREATE TABLE Proveedores (
    ProveedorId INT PRIMARY KEY IDENTITY(1,1),
    NombreProveedor NVARCHAR(100) NOT NULL,
    RUC NVARCHAR(20) UNIQUE NOT NULL,
    Telefono NVARCHAR(20),
    Email NVARCHAR(100),
    Direccion NVARCHAR(200),
    ContactoNombre NVARCHAR(100),
    FechaRegistro DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1
);

-- Tabla de Compras
CREATE TABLE Compras (
    CompraId INT PRIMARY KEY IDENTITY(1,1),
    ProveedorId INT NOT NULL,
    NumeroComprobante NVARCHAR(50) UNIQUE NOT NULL,
    TipoComprobante NVARCHAR(30) CHECK (TipoComprobante IN ('Factura', 'Boleta', 'Guía')),
    FechaCompra DATETIME DEFAULT GETDATE(),
    UsuarioRegistroId INT NOT NULL,
    MontoTotal DECIMAL(12,2) DEFAULT 0,
    Observaciones NVARCHAR(500),
    Estado NVARCHAR(20) DEFAULT 'Completada' CHECK (Estado IN ('Pendiente', 'Completada', 'Anulada')),
    CONSTRAINT FK_Compras_Proveedores FOREIGN KEY (ProveedorId) REFERENCES Proveedores(ProveedorId),
    CONSTRAINT FK_Compras_Usuarios FOREIGN KEY (UsuarioRegistroId) REFERENCES Usuarios(UsuarioId)
);

-- Tabla de Detalle de Compras
CREATE TABLE DetalleCompras (
    DetalleCompraId INT PRIMARY KEY IDENTITY(1,1),
    CompraId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(12,2) NOT NULL,
    FechaVencimiento DATE,
    Lote NVARCHAR(50),
    CONSTRAINT FK_DetalleCompras_Compras FOREIGN KEY (CompraId) REFERENCES Compras(CompraId),
    CONSTRAINT FK_DetalleCompras_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(ProductoId)
);

-- Tabla de Citas
CREATE TABLE Citas (
    CitaId INT PRIMARY KEY IDENTITY(1,1),
    PacienteId INT NOT NULL,
    EnfermeraId INT,
    FechaCita DATETIME NOT NULL,
    MotivoConsulta NVARCHAR(300) NOT NULL,
    Diagnostico NVARCHAR(500),
    Observaciones NVARCHAR(500),
    EstadoCita NVARCHAR(20) DEFAULT 'Pendiente' CHECK (EstadoCita IN ('Pendiente', 'Atendida', 'Cancelada', 'NoAsistió')),
    FechaRegistro DATETIME DEFAULT GETDATE(),
    FechaAtencion DATETIME,
    SignosVitales NVARCHAR(300), -- JSON: {temperatura, presion, pulso, etc}
    CONSTRAINT FK_Citas_Pacientes FOREIGN KEY (PacienteId) REFERENCES Usuarios(UsuarioId),
    CONSTRAINT FK_Citas_Enfermeras FOREIGN KEY (EnfermeraId) REFERENCES Usuarios(UsuarioId)
);

-- Tabla de Recetas
CREATE TABLE Recetas (
    RecetaId INT PRIMARY KEY IDENTITY(1,1),
    CitaId INT NOT NULL,
    PacienteId INT NOT NULL,
    EnfermeraId INT NOT NULL,
    FechaEmision DATETIME DEFAULT GETDATE(),
    Indicaciones NVARCHAR(500),
    Observaciones NVARCHAR(300),
    Estado NVARCHAR(20) DEFAULT 'Vigente' CHECK (Estado IN ('Vigente', 'Cumplida', 'Anulada')),
    CONSTRAINT FK_Recetas_Citas FOREIGN KEY (CitaId) REFERENCES Citas(CitaId),
    CONSTRAINT FK_Recetas_Pacientes FOREIGN KEY (PacienteId) REFERENCES Usuarios(UsuarioId),
    CONSTRAINT FK_Recetas_Enfermeras FOREIGN KEY (EnfermeraId) REFERENCES Usuarios(UsuarioId)
);

-- Tabla de Detalle de Recetas (Medicamentos/Implementos recetados)
CREATE TABLE DetalleRecetas (
    DetalleRecetaId INT PRIMARY KEY IDENTITY(1,1),
    RecetaId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    Dosis NVARCHAR(100),
    Frecuencia NVARCHAR(100),
    Duracion NVARCHAR(50),
    Indicaciones NVARCHAR(300),
    FechaEntrega DATETIME,
    Entregado BIT DEFAULT 0,
    CONSTRAINT FK_DetalleRecetas_Recetas FOREIGN KEY (RecetaId) REFERENCES Recetas(RecetaId),
    CONSTRAINT FK_DetalleRecetas_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(ProductoId)
);

-- Tabla de Alertas del Sistema
CREATE TABLE Alertas (
    AlertaId INT PRIMARY KEY IDENTITY(1,1),
    TipoAlerta NVARCHAR(50) CHECK (TipoAlerta IN ('StockBajo', 'ProductoVencido', 'CitaProxima', 'CitaHoy')),
    Mensaje NVARCHAR(300) NOT NULL,
    RolDestino NVARCHAR(50), -- A qué rol va dirigida
    UsuarioDestinoId INT, -- Usuario específico (opcional)
    FechaGeneracion DATETIME DEFAULT GETDATE(),
    Leida BIT DEFAULT 0,
    FechaLectura DATETIME,
    Prioridad NVARCHAR(20) DEFAULT 'Media' CHECK (Prioridad IN ('Baja', 'Media', 'Alta', 'Crítica')),
    ReferenciaId INT, -- ID del objeto relacionado (ProductoId, CitaId, etc)
    TipoReferencia NVARCHAR(50), -- 'Producto', 'Cita', etc
    CONSTRAINT FK_Alertas_Usuarios FOREIGN KEY (UsuarioDestinoId) REFERENCES Usuarios(UsuarioId)
);

-- Tabla de Auditoría
CREATE TABLE Auditoria (
    AuditoriaId INT PRIMARY KEY IDENTITY(1,1),
    UsuarioId INT NOT NULL,
    Accion NVARCHAR(100) NOT NULL,
    Tabla NVARCHAR(50) NOT NULL,
    RegistroId INT,
    ValoresAnteriores NVARCHAR(MAX),
    ValoresNuevos NVARCHAR(MAX),
    FechaAccion DATETIME DEFAULT GETDATE(),
    IPAddress NVARCHAR(50),
    CONSTRAINT FK_Auditoria_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(UsuarioId)
);

GO

-- =============================================
-- ÍNDICES PARA OPTIMIZACIÓN
-- =============================================

CREATE INDEX IX_Usuarios_RolId ON Usuarios(RolId);
CREATE INDEX IX_Usuarios_NombreUsuario ON Usuarios(NombreUsuario);
CREATE INDEX IX_Productos_CodigoProducto ON Productos(CodigoProducto);
CREATE INDEX IX_Productos_StockActual ON Productos(StockActual);
CREATE INDEX IX_Citas_PacienteId ON Citas(PacienteId);
CREATE INDEX IX_Citas_FechaCita ON Citas(FechaCita);
CREATE INDEX IX_Citas_EstadoCita ON Citas(EstadoCita);
CREATE INDEX IX_Recetas_PacienteId ON Recetas(PacienteId);
CREATE INDEX IX_Alertas_RolDestino ON Alertas(RolDestino);
CREATE INDEX IX_Alertas_Leida ON Alertas(Leida);

GO

-- =============================================
-- DATOS INICIALES
-- =============================================

-- Insertar Roles
INSERT INTO Roles (NombreRol, Descripcion) VALUES
('Administrador', 'Control total del sistema, gestión de usuarios y compras'),
('Enfermera', 'Atención de pacientes, registro de citas y recetas'),
('Paciente', 'Estudiante o paciente del tópico');

-- Insertar usuario Administrador por defecto
-- Usuario: admin | Contraseña: Admin123! (debe ser hasheada en la app)
INSERT INTO Usuarios (RolId, NombreCompleto, DNI, Email, NombreUsuario, Contraseña, Activo, DebecambiarContraseña) 
VALUES (1, 'Administrador del Sistema', '00000000', 'admin@topico.edu', 'admin', 'Admin123!', 1, 0);

-- Insertar Categorías de Productos
INSERT INTO CategoriasProducto (NombreCategoria, Descripcion, TipoCategoria) VALUES
('Analgésicos', 'Medicamentos para alivio del dolor', 'Medicamento'),
('Antiinflamatorios', 'Medicamentos antiinflamatorios', 'Medicamento'),
('Antibióticos', 'Medicamentos antibacterianos', 'Medicamento'),
('Antialérgicos', 'Medicamentos para alergias', 'Medicamento'),
('Material de Curación', 'Gasas, vendas, algodón', 'Implemento'),
('Instrumental', 'Termómetros, tensiómetros, etc', 'Implemento'),
('Insumos', 'Jeringas, guantes, mascarillas', 'Implemento');

GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS
-- =============================================

-- SP: Autenticar Usuario
CREATE PROCEDURE SP_AutenticarUsuario
    @NombreUsuario NVARCHAR(50),
    @Contraseña NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.UsuarioId, u.RolId, u.NombreCompleto, u.DNI, u.Email, 
        u.NombreUsuario, u.Telefono, u.DebecambiarContraseña, u.Activo,
        r.NombreRol
    FROM Usuarios u
    INNER JOIN Roles r ON u.RolId = r.RolId
    WHERE u.NombreUsuario = @NombreUsuario 
        AND u.Contraseña = @Contraseña 
        AND u.Activo = 1;
    
    -- Actualizar último acceso
    UPDATE Usuarios 
    SET UltimoAcceso = GETDATE() 
    WHERE NombreUsuario = @NombreUsuario;
END
GO

-- SP: Crear Usuario
CREATE PROCEDURE SP_CrearUsuario
    @RolId INT,
    @NombreCompleto NVARCHAR(100),
    @DNI NVARCHAR(20),
    @Email NVARCHAR(100),
    @Telefono NVARCHAR(20),
    @NombreUsuario NVARCHAR(50),
    @Contraseña NVARCHAR(255),
    @FechaNacimiento DATE,
    @Direccion NVARCHAR(200),
    @UsuarioCreadorId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO Usuarios (RolId, NombreCompleto, DNI, Email, Telefono, NombreUsuario, Contraseña, FechaNacimiento, Direccion)
        VALUES (@RolId, @NombreCompleto, @DNI, @Email, @Telefono, @NombreUsuario, @Contraseña, @FechaNacimiento, @Direccion);
        
        DECLARE @NuevoUsuarioId INT = SCOPE_IDENTITY();
        
        -- Registrar auditoría
        INSERT INTO Auditoria (UsuarioId, Accion, Tabla, RegistroId, ValoresNuevos)
        VALUES (@UsuarioCreadorId, 'INSERT', 'Usuarios', @NuevoUsuarioId, 
                'Usuario: ' + @NombreUsuario + ', Rol: ' + CAST(@RolId AS NVARCHAR));
        
        COMMIT TRANSACTION;
        SELECT @NuevoUsuarioId AS UsuarioId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: Actualizar Contraseña
CREATE PROCEDURE SP_ActualizarContraseña
    @UsuarioId INT,
    @ContraseñaAnterior NVARCHAR(255),
    @ContraseñaNueva NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ContraseñaActual NVARCHAR(255);
        
        SELECT @ContraseñaActual = Contraseña 
        FROM Usuarios 
        WHERE UsuarioId = @UsuarioId;
        
        IF @ContraseñaActual = @ContraseñaAnterior
        BEGIN
            UPDATE Usuarios 
            SET Contraseña = @ContraseñaNueva, 
                DebecambiarContraseña = 0 
            WHERE UsuarioId = @UsuarioId;
            
            -- Registrar auditoría
            INSERT INTO Auditoria (UsuarioId, Accion, Tabla, RegistroId)
            VALUES (@UsuarioId, 'UPDATE_PASSWORD', 'Usuarios', @UsuarioId);
            
            SELECT 1 AS Resultado, 'Contraseña actualizada correctamente' AS Mensaje;
        END
        ELSE
        BEGIN
            SELECT 0 AS Resultado, 'Contraseña anterior incorrecta' AS Mensaje;
        END
    END TRY
    BEGIN CATCH
        SELECT -1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- SP: Listar Usuarios por Rol
CREATE PROCEDURE SP_ListarUsuariosPorRol
    @RolId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.UsuarioId, u.RolId, u.NombreCompleto, u.DNI, u.Email, 
        u.Telefono, u.NombreUsuario, u.FechaNacimiento, u.Direccion,
        u.FechaRegistro, u.UltimoAcceso, u.Activo,
        r.NombreRol
    FROM Usuarios u
    INNER JOIN Roles r ON u.RolId = r.RolId
    WHERE (@RolId IS NULL OR u.RolId = @RolId)
    ORDER BY u.NombreCompleto;
END
GO

-- SP: Registrar Compra
CREATE PROCEDURE SP_RegistrarCompra
    @ProveedorId INT,
    @NumeroComprobante NVARCHAR(50),
    @TipoComprobante NVARCHAR(30),
    @UsuarioRegistroId INT,
    @MontoTotal DECIMAL(12,2),
    @Observaciones NVARCHAR(500),
    @DetallesJSON NVARCHAR(MAX) -- JSON con los detalles
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Insertar compra
        INSERT INTO Compras (ProveedorId, NumeroComprobante, TipoComprobante, UsuarioRegistroId, MontoTotal, Observaciones)
        VALUES (@ProveedorId, @NumeroComprobante, @TipoComprobante, @UsuarioRegistroId, @MontoTotal, @Observaciones);
        
        DECLARE @CompraId INT = SCOPE_IDENTITY();
        
        -- Insertar detalles y actualizar stock
        INSERT INTO DetalleCompras (CompraId, ProductoId, Cantidad, PrecioUnitario, Subtotal, FechaVencimiento, Lote)
        SELECT 
            @CompraId,
            ProductoId,
            Cantidad,
            PrecioUnitario,
            Cantidad * PrecioUnitario,
            FechaVencimiento,
            Lote
        FROM OPENJSON(@DetallesJSON)
        WITH (
            ProductoId INT,
            Cantidad INT,
            PrecioUnitario DECIMAL(10,2),
            FechaVencimiento DATE,
            Lote NVARCHAR(50)
        );
        
        -- Actualizar stock de productos
        UPDATE p
        SET p.StockActual = p.StockActual + d.Cantidad,
            p.PrecioUnitario = d.PrecioUnitario,
            p.FechaVencimiento = ISNULL(d.FechaVencimiento, p.FechaVencimiento),
            p.Lote = ISNULL(d.Lote, p.Lote)
        FROM Productos p
        INNER JOIN (
            SELECT ProductoId, Cantidad, PrecioUnitario, FechaVencimiento, Lote
            FROM OPENJSON(@DetallesJSON)
            WITH (
                ProductoId INT,
                Cantidad INT,
                PrecioUnitario DECIMAL(10,2),
                FechaVencimiento DATE,
                Lote NVARCHAR(50)
            )
        ) d ON p.ProductoId = d.ProductoId;
        
        -- Registrar auditoría
        INSERT INTO Auditoria (UsuarioId, Accion, Tabla, RegistroId, ValoresNuevos)
        VALUES (@UsuarioRegistroId, 'INSERT', 'Compras', @CompraId, 
                'Comprobante: ' + @NumeroComprobante + ', Monto: ' + CAST(@MontoTotal AS NVARCHAR));
        
        COMMIT TRANSACTION;
        SELECT @CompraId AS CompraId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: Registrar Cita
CREATE PROCEDURE SP_RegistrarCita
    @PacienteId INT,
    @FechaCita DATETIME,
    @MotivoConsulta NVARCHAR(300),
    @UsuarioRegistroId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO Citas (PacienteId, FechaCita, MotivoConsulta)
        VALUES (@PacienteId, @FechaCita, @MotivoConsulta);
        
        DECLARE @CitaId INT = SCOPE_IDENTITY();
        
        -- Registrar auditoría
        INSERT INTO Auditoria (UsuarioId, Accion, Tabla, RegistroId)
        VALUES (@UsuarioRegistroId, 'INSERT', 'Citas', @CitaId);
        
        COMMIT TRANSACTION;
        SELECT @CitaId AS CitaId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: Atender Cita
CREATE PROCEDURE SP_AtenderCita
    @CitaId INT,
    @EnfermeraId INT,
    @Diagnostico NVARCHAR(500),
    @Observaciones NVARCHAR(500),
    @SignosVitales NVARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE Citas
        SET EnfermeraId = @EnfermeraId,
            Diagnostico = @Diagnostico,
            Observaciones = @Observaciones,
            SignosVitales = @SignosVitales,
            EstadoCita = 'Atendida',
            FechaAtencion = GETDATE()
        WHERE CitaId = @CitaId;
        
        -- Registrar auditoría
        INSERT INTO Auditoria (UsuarioId, Accion, Tabla, RegistroId)
        VALUES (@EnfermeraId, 'UPDATE', 'Citas', @CitaId);
        
        COMMIT TRANSACTION;
        SELECT 1 AS Resultado;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: Crear Receta
CREATE PROCEDURE SP_CrearReceta
    @CitaId INT,
    @PacienteId INT,
    @EnfermeraId INT,
    @Indicaciones NVARCHAR(500),
    @Observaciones NVARCHAR(300),
    @DetallesJSON NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Crear receta
        INSERT INTO Recetas (CitaId, PacienteId, EnfermeraId, Indicaciones, Observaciones)
        VALUES (@CitaId, @PacienteId, @EnfermeraId, @Indicaciones, @Observaciones);
        
        DECLARE @RecetaId INT = SCOPE_IDENTITY();
        
        -- Insertar detalles
        INSERT INTO DetalleRecetas (RecetaId, ProductoId, Cantidad, Dosis, Frecuencia, Duracion, Indicaciones)
        SELECT 
            @RecetaId,
            ProductoId,
            Cantidad,
            Dosis,
            Frecuencia,
            Duracion,
            Indicaciones
        FROM OPENJSON(@DetallesJSON)
        WITH (
            ProductoId INT,
            Cantidad INT,
            Dosis NVARCHAR(100),
            Frecuencia NVARCHAR(100),
            Duracion NVARCHAR(50),
            Indicaciones NVARCHAR(300)
        );
        
        -- Descontar stock
        UPDATE p
        SET p.StockActual = p.StockActual - d.Cantidad
        FROM Productos p
        INNER JOIN (
            SELECT ProductoId, Cantidad
            FROM OPENJSON(@DetallesJSON)
            WITH (ProductoId INT, Cantidad INT)
        ) d ON p.ProductoId = d.ProductoId;
        
        -- Registrar auditoría
        INSERT INTO Auditoria (UsuarioId, Accion, Tabla, RegistroId)
        VALUES (@EnfermeraId, 'INSERT', 'Recetas', @RecetaId);
        
        COMMIT TRANSACTION;
        SELECT @RecetaId AS RecetaId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: Listar Citas Por Paciente
CREATE PROCEDURE SP_ListarCitasPorPaciente
    @PacienteId INT,
    @EstadoCita NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CitaId, c.FechaCita, c.MotivoConsulta, c.Diagnostico,
        c.Observaciones, c.EstadoCita, c.FechaAtencion, c.SignosVitales,
        e.NombreCompleto AS NombreEnfermera
    FROM Citas c
    LEFT JOIN Usuarios e ON c.EnfermeraId = e.UsuarioId
    WHERE c.PacienteId = @PacienteId
        AND (@EstadoCita IS NULL OR c.EstadoCita = @EstadoCita)
    ORDER BY c.FechaCita DESC;
END
GO

-- SP: Listar Recetas Por Paciente
CREATE PROCEDURE SP_ListarRecetasPorPaciente
    @PacienteId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.RecetaId, r.FechaEmision, r.Indicaciones, r.Observaciones, r.Estado,
        e.NombreCompleto AS NombreEnfermera,
        c.FechaCita, c.Diagnostico
    FROM Recetas r
    INNER JOIN Usuarios e ON r.EnfermeraId = e.UsuarioId
    INNER JOIN Citas c ON r.CitaId = c.CitaId
    WHERE r.PacienteId = @PacienteId
    ORDER BY r.FechaEmision DESC;
END
GO

-- SP: Obtener Detalle de Receta
CREATE PROCEDURE SP_ObtenerDetalleReceta
    @RecetaId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        dr.DetalleRecetaId, dr.Cantidad, dr.Dosis, dr.Frecuencia, 
        dr.Duracion, dr.Indicaciones, dr.Entregado, dr.FechaEntrega,
        p.NombreProducto, p.TipoProducto, p.UnidadMedida
    FROM DetalleRecetas dr
    INNER JOIN Productos p ON dr.ProductoId = p.ProductoId
    WHERE dr.RecetaId = @RecetaId;
END
GO

-- SP: Listar Productos con Stock Bajo
CREATE PROCEDURE SP_ListarProductosStockBajo
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ProductoId, CodigoProducto, NombreProducto, TipoProducto,
        StockActual, StockMinimo, UnidadMedida,
        (StockMinimo - StockActual) AS CantidadFaltante
    FROM Productos
    WHERE StockActual <= StockMinimo AND Activo = 1
    ORDER BY StockActual ASC;
END
GO

-- SP: Listar Productos Por Vencer
CREATE PROCEDURE SP_ListarProductosPorVencer
    @DiasAnticipacion INT = 30
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ProductoId, CodigoProducto, NombreProducto, TipoProducto,
        FechaVencimiento, StockActual, Lote,
        DATEDIFF(DAY, GETDATE(), FechaVencimiento) AS DiasParaVencer
    FROM Productos
    WHERE FechaVencimiento IS NOT NULL 
        AND FechaVencimiento <= DATEADD(DAY, @DiasAnticipacion, GETDATE())
        AND Activo = 1
    ORDER BY FechaVencimiento ASC;
END
GO

-- SP: Listar Citas Próximas
CREATE PROCEDURE SP_ListarCitasProximas
    @DiasAnticipacion INT = 3,
    @PacienteId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CitaId, c.FechaCita, c.MotivoConsulta, c.EstadoCita,
        p.NombreCompleto AS NombrePaciente, p.DNI, p.Telefono,
        DATEDIFF(HOUR, GETDATE(), c.FechaCita) AS HorasParaCita
    FROM Citas c
    INNER JOIN Usuarios p ON c.PacienteId = p.UsuarioId
    WHERE
        c.FechaCita >= GETDATE()                                     -- solo futuras
        AND c.FechaCita <= DATEADD(DAY, @DiasAnticipacion, GETDATE()) -- dentro del rango
        AND (@PacienteId IS NULL OR c.PacienteId = @PacienteId)       -- filtro opcional
    ORDER BY 
        c.FechaCita ASC;
END
GO
-- ============================================================
-- SCRIPT DE DATOS INICIALES ADAPTADO A TopicoMedicoDB
-- 15 Categorías + 15 Proveedores + 15 Productos
-- ============================================================

USE TopicoMedicoDB;
GO

-- ============================================================
-- 1. LIMPIAR DATOS EXISTENTES (OPCIONAL - COMENTAR SI NO DESEAS)
-- ============================================================
/*
DELETE FROM DetalleRecetas;
DELETE FROM Recetas;
DELETE FROM DetalleCompras;
DELETE FROM Compras;
DELETE FROM Citas;
DELETE FROM Productos;
DELETE FROM Proveedores;
DELETE FROM CategoriasProducto WHERE CategoriaId > 7; -- Mantener las 7 iniciales
*/

-- ============================================================
-- 2. CATEGORÍAS DE PRODUCTOS ADICIONALES
-- ============================================================
SET IDENTITY_INSERT CategoriasProducto ON;

-- Mantener las 7 categorías iniciales y agregar 8 más
INSERT INTO CategoriasProducto (CategoriaId, NombreCategoria, Descripcion, TipoCategoria, Activo)
VALUES 
(8, 'Vitaminas y Suplementos', 'Complementos vitamínicos', 'Medicamento', 1),
(9, 'Antisépticos', 'Productos para desinfección', 'Implemento', 1),
(10, 'Soluciones Médicas', 'Soluciones y líquidos', 'Implemento', 1),
(11, 'Equipos de Diagnóstico', 'Termómetros, oxímetros', 'Implemento', 1),
(12, 'Material Quirúrgico', 'Bisturís, pinzas', 'Implemento', 1),
(13, 'Respiratorio', 'Nebulizadores, mascarillas', 'Implemento', 1),
(14, 'Cardiovascular', 'Tensiómetros, estetoscopios', 'Implemento', 1),
(15, 'Primeros Auxilios', 'Kits y material de emergencia', 'Implemento', 1);

SET IDENTITY_INSERT CategoriasProducto OFF;
GO

-- ============================================================
-- 3. PROVEEDORES (15 proveedores peruanos)
-- ============================================================
SET IDENTITY_INSERT Proveedores ON;

INSERT INTO Proveedores (ProveedorId, NombreProveedor, RUC, Telefono, Email, Direccion, ContactoNombre, FechaRegistro, Activo)
VALUES 
(1, 'Farmacias Peruanas S.A.', '20123456789', '014251200', 'ventas@inkafarma.pe', 'Av. Aviación 2405, San Borja, Lima', 'Carlos Mendoza', GETDATE(), 1),
(2, 'Química Suiza S.A.', '20234567890', '016285000', 'contacto@qsuiza.com.pe', 'Av. Grau 551, Callao', 'Ana García', GETDATE(), 1),
(3, 'Albis S.A.', '20345678901', '015112500', 'ventas@albis.com.pe', 'Av. Angamos Este 2520, Surquillo, Lima', 'Roberto Silva', GETDATE(), 1),
(4, 'Drokasa Perú S.A.', '20456789012', '016177100', 'info@drokasa.com.pe', 'Av. Separadora Industrial 1781, Ate, Lima', 'María Torres', GETDATE(), 1),
(5, 'Medifarma S.A.', '20567890123', '015185000', 'contacto@medifarma.com.pe', 'Av. Tomás Marsano 2595, Surquillo, Lima', 'José Ramírez', GETDATE(), 1),
(6, 'Droguería Misti S.A.', '20678901234', '054229400', 'ventas@misti.com.pe', 'Calle Moral 318, Arequipa', 'Patricia Flores', GETDATE(), 1),
(7, 'Alfaro S.A.', '20789012345', '017067000', 'alfaro@alfaro.com.pe', 'Jr. Lampa 861, Cercado de Lima', 'Luis Vargas', GETDATE(), 1),
(8, 'Tawa Médica E.I.R.L.', '20890123456', '014251800', 'ventas@tawamedica.pe', 'Av. Javier Prado Este 485, San Isidro, Lima', 'Carmen Rojas', GETDATE(), 1),
(9, 'Dismed Perú S.A.C.', '20901234567', '016284500', 'info@dismedperu.com', 'Av. La Marina 3568, San Miguel, Lima', 'Fernando Castro', GETDATE(), 1),
(10, 'Global Medical Supplies', '21012345678', '015178900', 'contacto@globalmedical.pe', 'Av. Universitaria 1875, Los Olivos, Lima', 'Sandra Quispe', GETDATE(), 1),
(11, 'Laboratorios Bagó del Perú', '21123456789', '016360000', 'ventas@bago.com.pe', 'Av. Angamos Oeste 1380, Miraflores, Lima', 'Ricardo Chávez', GETDATE(), 1),
(12, 'Roche Perú S.A.', '21234567890', '012119000', 'info@roche.com.pe', 'Av. Paseo de la República 3587, San Isidro, Lima', 'Elena Morales', GETDATE(), 1),
(13, 'Bayer S.A.', '21345678901', '016130000', 'contacto@bayer.com.pe', 'Av. Paseo de la República 3074, San Isidro, Lima', 'Miguel Paredes', GETDATE(), 1),
(14, 'Pfizer S.A.', '21456789012', '016100900', 'pfizer@pfizer.com.pe', 'Av. El Derby 055, Santiago de Surco, Lima', 'Claudia Sánchez', GETDATE(), 1),
(15, 'Abbott Laboratorios S.A.', '21567890123', '016190000', 'abbott@abbott.com.pe', 'Av. Víctor Andrés Belaúnde 280, San Isidro, Lima', 'Jorge Vega', GETDATE(), 1);

SET IDENTITY_INSERT Proveedores OFF;
GO

-- ============================================================
-- 4. PRODUCTOS (30 productos variados)
-- ============================================================
SET IDENTITY_INSERT Productos ON;

INSERT INTO Productos (
    ProductoId, CategoriaId, CodigoProducto, NombreProducto, Descripcion, 
    TipoProducto, UnidadMedida, StockActual, StockMinimo, PrecioUnitario, 
    FechaVencimiento, Laboratorio, Lote, RequiereReceta, Activo, FechaRegistro
)
VALUES 
-- ANTIBIÓTICOS (CategoriaId = 3)
(1, 3, 'ANT-001', 'Amoxicilina 500mg', 'Antibiótico de amplio espectro', 'Medicamento', 'Caja x 24 cápsulas', 150, 30, 25.50, '2026-12-31', 'Medifarma', 'AMX2024001', 1, 1, GETDATE()),
(2, 3, 'ANT-002', 'Azitromicina 500mg', 'Antibiótico macrólido', 'Medicamento', 'Caja x 6 tabletas', 120, 25, 35.00, '2026-08-15', 'Laboratorios Bagó', 'AZI2024002', 1, 1, GETDATE()),
(3, 3, 'ANT-003', 'Ciprofloxacino 500mg', 'Antibiótico fluoroquinolona', 'Medicamento', 'Caja x 10 tabletas', 90, 20, 42.00, '2026-07-31', 'Roche', 'CIP2024003', 1, 1, GETDATE()),
(4, 3, 'ANT-004', 'Cefalexina 500mg', 'Antibiótico cefalosporina', 'Medicamento', 'Caja x 12 cápsulas', 110, 25, 38.50, '2026-09-30', 'Medifarma', 'CEF2024004', 1, 1, GETDATE()),

-- ANALGÉSICOS (CategoriaId = 1)
(5, 1, 'ANG-001', 'Paracetamol 500mg', 'Analgésico y antipirético', 'Medicamento', 'Caja x 100 tabletas', 300, 50, 15.00, '2027-03-20', 'Medifarma', 'PAR2024001', 0, 1, GETDATE()),
(6, 1, 'ANG-002', 'Ibuprofeno 400mg', 'Antiinflamatorio no esteroideo', 'Medicamento', 'Frasco x 100 tabletas', 200, 40, 22.00, '2026-11-30', 'Bayer', 'IBU2024002', 0, 1, GETDATE()),
(7, 1, 'ANG-003', 'Naproxeno 550mg', 'Antiinflamatorio analgésico', 'Medicamento', 'Caja x 20 tabletas', 150, 30, 28.00, '2027-01-31', 'Bayer', 'NAP2024003', 0, 1, GETDATE()),
(8, 1, 'ANG-004', 'Tramadol 50mg', 'Analgésico opioide débil', 'Medicamento', 'Caja x 20 cápsulas', 15, 15, 55.00, '2026-11-30', 'Laboratorios Bagó', 'TRA2024004', 1, 1, GETDATE()),

-- ANTIINFLAMATORIOS (CategoriaId = 2)
(9, 2, 'AIF-001', 'Diclofenaco 50mg', 'Antiinflamatorio potente', 'Medicamento', 'Caja x 20 tabletas', 100, 20, 18.50, '2026-09-25', 'Roche', 'DIC2024001', 1, 1, GETDATE()),
(10, 2, 'AIF-002', 'Ketoprofeno 100mg', 'Antiinflamatorio y analgésico', 'Medicamento', 'Caja x 10 cápsulas', 80, 15, 32.00, '2026-10-15', 'Pfizer', 'KET2024002', 0, 1, GETDATE()),

-- VITAMINAS (CategoriaId = 8)
(11, 8, 'VIT-001', 'Complejo B Forte', 'Vitaminas del complejo B', 'Medicamento', 'Frasco x 100 cápsulas', 180, 30, 45.00, '2027-06-30', 'Abbott', 'CPB2024001', 0, 1, GETDATE()),
(12, 8, 'VIT-002', 'Vitamina C 1000mg', 'Ácido ascórbico', 'Medicamento', 'Frasco x 60 tabletas', 250, 40, 32.00, '2027-04-15', 'Pfizer', 'VTC2024002', 0, 1, GETDATE()),
(13, 8, 'VIT-003', 'Multivitamínico Centrum', 'Complejo multivitamínico', 'Medicamento', 'Frasco x 100 tabletas', 140, 25, 68.00, '2027-08-20', 'Pfizer', 'CEN2024003', 0, 1, GETDATE()),

-- MATERIAL DE CURACIÓN (CategoriaId = 5)
(14, 5, 'CUR-001', 'Gasa Estéril 10x10cm', 'Gasa para curaciones', 'Implemento', 'Paquete x 100 unidades', 400, 80, 35.00, '2027-12-31', 'Tawa Médica', 'GAS2024001', 0, 1, GETDATE()),
(15, 5, 'CUR-002', 'Venda Elástica 10cm x 4.5m', 'Venda elástica cohesiva', 'Implemento', 'Unidad', 200, 40, 12.50, '2028-06-30', 'Tawa Médica', 'VEN2024002', 0, 1, GETDATE()),
(16, 5, 'CUR-003', 'Apósito Adhesivo 10x10cm', 'Apósito estéril con adhesivo', 'Implemento', 'Caja x 50 unidades', 180, 35, 68.00, '2027-08-31', 'Tawa Médica', 'APO2024003', 0, 1, GETDATE()),
(17, 5, 'CUR-004', 'Esparadrapo 5cm x 10m', 'Cinta adhesiva hipoalergénica', 'Implemento', 'Rollo', 220, 45, 15.50, '2027-12-31', 'Albis', 'ESP2024004', 0, 1, GETDATE()),

-- INSUMOS (CategoriaId = 7)
(18, 7, 'INS-001', 'Jeringa Descartable 10ml', 'Jeringa estéril con aguja', 'Implemento', 'Caja x 100 unidades', 500, 100, 85.00, '2028-12-31', 'Dismed', 'JER2024001', 0, 1, GETDATE()),
(19, 7, 'INS-002', 'Guantes de Látex Talla M', 'Guantes descartables', 'Implemento', 'Caja x 100 unidades', 800, 150, 65.00, '2027-12-31', 'Global Medical', 'GLT2024002', 0, 1, GETDATE()),
(20, 7, 'INS-003', 'Mascarilla Quirúrgica', 'Mascarilla tricapa descartable', 'Implemento', 'Caja x 50 unidades', 1000, 200, 45.00, '2027-06-30', 'Global Medical', 'MSC2024003', 0, 1, GETDATE()),

-- ANTISÉPTICOS (CategoriaId = 9)
(21, 9, 'ASP-001', 'Alcohol Yodado 120ml', 'Solución antiséptica', 'Implemento', 'Frasco x 120ml', 280, 50, 12.00, '2027-08-31', 'Química Suiza', 'ALY2024001', 0, 1, GETDATE()),
(22, 9, 'ASP-002', 'Alcohol Gel 70% 1L', 'Alcohol gel antiséptico', 'Implemento', 'Frasco x 1 litro', 450, 90, 25.00, '2026-12-31', 'Drokasa', 'ALC2024002', 0, 1, GETDATE()),

-- SOLUCIONES (CategoriaId = 10)
(23, 10, 'SOL-001', 'Suero Fisiológico 0.9% 1000ml', 'Cloruro de sodio', 'Implemento', 'Bolsa x 1000ml', 350, 70, 8.50, '2026-10-31', 'Albis', 'SUE2024001', 0, 1, GETDATE()),
(24, 10, 'SOL-002', 'Dextrosa 5% 1000ml', 'Solución glucosada', 'Implemento', 'Bolsa x 1000ml', 160, 35, 12.00, '2026-09-30', 'Albis', 'DEX2024002', 0, 1, GETDATE()),

-- INSTRUMENTAL (CategoriaId = 6)
(25, 6, 'INM-001', 'Termómetro Digital', 'Termómetro infrarrojo', 'Implemento', 'Unidad', 45, 10, 85.00, NULL, 'Global Medical', 'TER2024001', 0, 1, GETDATE()),
(26, 6, 'INM-002', 'Oxímetro de Pulso', 'Medidor de saturación', 'Implemento', 'Unidad', 8, 8, 120.00, NULL, 'Global Medical', 'OXI2024002', 0, 1, GETDATE()),
(27, 6, 'INM-003', 'Tensiómetro Digital', 'Medidor de presión arterial', 'Implemento', 'Unidad', 25, 5, 180.00, NULL, 'Dismed', 'TEN2024003', 0, 1, GETDATE()),

-- MATERIAL QUIRÚRGICO (CategoriaId = 12)
(28, 12, 'QUI-001', 'Bisturí Descartable No. 11', 'Hoja de bisturí estéril', 'Implemento', 'Caja x 100 unidades', 120, 25, 95.00, '2028-12-31', 'Tawa Médica', 'BIS2024001', 0, 1, GETDATE()),
(29, 12, 'QUI-002', 'Pinza Kelly Curva 16cm', 'Pinza hemostática', 'Implemento', 'Unidad', 20, 25, 125.00, NULL, 'Dismed', 'PIN2024002', 0, 1, GETDATE()),

-- RESPIRATORIO (CategoriaId = 13)
(30, 13, 'RES-001', 'Nebulizador Portátil', 'Nebulizador ultrasónico', 'Implemento', 'Unidad', 25, 5, 280.00, NULL, 'Dismed', 'NEB2024001', 0, 1, GETDATE());

SET IDENTITY_INSERT Productos OFF;
GO

-- ============================================================
-- 5. VERIFICACIÓN Y RESUMEN
-- ============================================================
PRINT '============================================================';
PRINT 'RESUMEN DE DATOS INSERTADOS:';
PRINT '============================================================';
PRINT '';

SELECT 'Categorías' AS Tabla, COUNT(*) AS Total FROM CategoriasProducto
UNION ALL
SELECT 'Proveedores', COUNT(*) FROM Proveedores
UNION ALL
SELECT 'Productos', COUNT(*) FROM Productos;

PRINT '';
PRINT '============================================================';
PRINT 'PRODUCTOS POR CATEGORÍA:';
PRINT '============================================================';

SELECT 
    c.NombreCategoria AS Categoría,
    c.TipoCategoria AS Tipo,
    COUNT(p.ProductoId) AS Cantidad,
    SUM(p.StockActual) AS [Stock Total]
FROM CategoriasProducto c
LEFT JOIN Productos p ON c.CategoriaId = p.CategoriaId
GROUP BY c.NombreCategoria, c.TipoCategoria
ORDER BY Cantidad DESC;

PRINT '';
PRINT '============================================================';
PRINT 'PRODUCTOS CON STOCK CRÍTICO:';
PRINT '============================================================';

SELECT 
    CodigoProducto AS Código,
    NombreProducto AS Producto,
    StockActual AS [Stock],
    StockMinimo AS Mínimo,
    CASE 
        WHEN StockActual < StockMinimo THEN 'CRÍTICO'
        WHEN StockActual = StockMinimo THEN 'BAJO'
        ELSE 'OK'
    END AS Estado
FROM Productos
WHERE StockActual <= StockMinimo
ORDER BY StockActual;

PRINT '';
PRINT '============================================================';
PRINT 'VALOR TOTAL DEL INVENTARIO:';
PRINT '============================================================';

SELECT 
    COUNT(*) AS [Total Productos],
    SUM(StockActual) AS [Unidades Totales],
    FORMAT(SUM(StockActual * ISNULL(PrecioUnitario, 0)), 'C', 'es-PE') AS [Valor Inventario]
FROM Productos;

PRINT '';
PRINT '============================================================';
PRINT '✅ DATOS INSERTADOS CORRECTAMENTE';
PRINT '============================================================';
GO

USE TopicoMedicoDB;
GO

-- ============================================
-- PASO 1: Verificar y crear roles
-- ============================================

-- Limpiar roles existentes (opcional)
-- DELETE FROM Roles;

-- Crear roles si no existen
IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'Administrador')
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, FechaCreacion, Activo)
    VALUES ('Administrador', 'Control total del sistema, gestión de usuarios y compras', GETDATE(), 1);
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'Enfermera')
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, FechaCreacion, Activo)
    VALUES ('Enfermera', 'Atención de pacientes, registro de citas y recetas', GETDATE(), 1);
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'Paciente')
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, FechaCreacion, Activo)
    VALUES ('Paciente', 'Estudiante o paciente del tópico', GETDATE(), 1);
END
GO

-- Verificar roles creados
SELECT * FROM Roles;
GO

-- ============================================
-- PASO 2: Crear usuario administrador
-- ============================================

-- Eliminar admin si existe
DELETE FROM Usuarios WHERE NombreUsuario = 'admin';
GO

-- Obtener el RolId de Administrador
DECLARE @RolAdminId INT;
SELECT @RolAdminId = RolId FROM Roles WHERE NombreRol = 'Administrador';

-- Insertar usuario admin con el hash correcto generado desde tu app
INSERT INTO Usuarios (
    RolId, 
    NombreCompleto, 
    DNI, 
    Email, 
    Telefono,
    NombreUsuario, 
    Contraseña,
    FechaNacimiento,
    Direccion,
    Activo, 
    DebecambiarContraseña,
    FechaRegistro
) 
VALUES (
    @RolAdminId,                                                              -- RolId
    'Administrador del Sistema',                                              -- NombreCompleto
    '00000000',                                                               -- DNI
    'admin@topico.edu',                                                       -- Email
    '999999999',                                                              -- Telefono
    'admin',                                                                  -- NombreUsuario
    '3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121',    -- Hash de Admin123!
    '1990-01-01',                                                            -- FechaNacimiento
    'Oficina Principal',                                                      -- Direccion
    1,                                                                        -- Activo
    0,                                                                        -- DebecambiarContraseña
    GETDATE()                                                                 -- FechaRegistro
);
GO

-- ============================================
-- PASO 3: Verificar que todo está correcto
-- ============================================

-- Ver roles
SELECT 'ROLES:' AS Tabla, * FROM Roles;
GO

-- Ver usuario admin
SELECT 
    'USUARIO ADMIN:' AS Tabla,
    u.UsuarioId,
    u.NombreUsuario, 
    LEFT(u.Contraseña, 20) + '...' AS HashInicio,
    LEN(u.Contraseña) AS LongitudHash,
    u.Activo,
    u.RolId,
    r.NombreRol
FROM Usuarios u
INNER JOIN Roles r ON u.RolId = r.RolId
WHERE u.NombreUsuario = 'admin';
GO

SELECT * FROM Productos;

INSERT INTO CategoriasProducto (Nombre, Activo)
VALUES ('Medicamentos', 1);

DECLARE @CategoriaId INT = SCOPE_IDENTITY();

INSERT INTO Productos
(
    Nombre,
    Descripcion,
    PrecioUnitario,
    Stock,
    FechaVencimiento,
    Activo,
    CategoriaProductoId
)
VALUES
(
    'Paracetamol 500mg',
    'Analgésico y antipirético',
    2.50,
    100,
    DATEADD(MONTH, 12, GETDATE()),
    1,
    @CategoriaId
);

SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Productos';

SELECT COUNT(*) FROM Productos;
SELECT TOP 10 * FROM Productos;


SELECT * FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'DetallesCompra'

SELECT CompraId, Observaciones
FROM Compras
WHERE Observaciones IS NULL;

SELECT 
    AlertaId,
    TipoAlerta,
    Prioridad,
    RolDestino,
    UsuarioDestinoId,
    Leida,
    FechaGeneracion
FROM Alertas
ORDER BY FechaGeneracion DESC;

SELECT *
FROM Alertas
WHERE RolDestino = 'Administrador' AND Leida = 0;

UPDATE Alertas
SET RolDestino = 'Administrador'
WHERE RolDestino IS NULL;

SELECT AlertaId, RolDestino, Leida
FROM Alertas;

SELECT *
FROM Alertas
WHERE RolDestino = 'Administrador' AND Leida = 0;




