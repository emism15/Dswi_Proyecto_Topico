-- Verificar base de datos actual
SELECT DB_NAME() AS BaseDeDatosActual

-- Insertar Roles (si no existen)
IF NOT EXISTS (SELECT * FROM Roles WHERE NombreRol = 'Administrador')
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, Activo, FechaCreacion)
    VALUES 
        ('Administrador', 'Acceso total al sistema', 1, GETDATE()),
        ('Enfermera', 'Gestión de citas y recetas', 1, GETDATE()),
        ('Paciente', 'Consulta de información personal', 1, GETDATE())
    
    PRINT '✅ Roles insertados correctamente'
END

-- Limpiar usuarios existentes (opcional)
DELETE FROM Usuarios

-- Insertar Usuarios con los hashes correctos
INSERT INTO Usuarios (RolId, NombreCompleto, DNI, Email, Telefono, NombreUsuario, Contraseña, FechaNacimiento, Direccion, FechaRegistro, Activo, DebecambiarContraseña)
VALUES 
    -- Administrador (admin / admin123)
    (1, 'Juan Pérez Admin', '12345678', 'admin@topico.com', '999888777', 'admin', 
     '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 
     '1985-03-15', 'Av. Principal 123, Lima', GETDATE(), 1, 0),
    
    -- Enfermera (enfermera / enfermera123)
    (2, 'María Rodríguez García', '87654321', 'enfermera@topico.com', '987654321', 'enfermera', 
     '90bf2403fac61ad700d74a750fbabb599d17097dc98ca07be6caa643a7d23a29', 
     '1988-05-20', 'Av. Salud 456, Lima', GETDATE(), 1, 0),
    
    -- Paciente (paciente / paciente123)
    (3, 'Carlos López Mendoza', '45678912', 'paciente@topico.com', '912345678', 'paciente', 
     '299fbb455c42239c86d2ee3b15403ed1b468259ecaedf0c3527451e1f0d63d59', 
     '1995-08-10', 'Jr. Los Olivos 789, Lima', GETDATE(), 1, 0)

PRINT '✅ Usuarios insertados correctamente'

-- Verificar que se insertaron
SELECT 
    u.UsuarioId, 
    u.NombreUsuario, 
    u.NombreCompleto, 
    r.NombreRol, 
    u.Activo,
    LEFT(u.Contraseña, 20) + '...' AS HashPreview
FROM Usuarios u
INNER JOIN Roles r ON u.RolId = r.RolId
ORDER BY u.RolId
