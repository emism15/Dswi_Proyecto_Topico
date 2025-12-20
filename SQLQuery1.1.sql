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
