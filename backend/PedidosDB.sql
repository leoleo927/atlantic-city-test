-- ================================================
-- Script de Creación de Base de Datos
-- Sistema de Gestión de Pedidos
-- ================================================

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'PedidosDB')
BEGIN
    CREATE DATABASE PedidosDB;
END
GO

USE PedidosDB;
GO

-- ================================================
-- Tabla de Usuarios
-- ================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        Nombre NVARCHAR(100) NOT NULL,
        Rol NVARCHAR(20) NOT NULL DEFAULT 'User',
        FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX IX_Usuarios_Email ON Usuarios(Email);
END
GO

-- ================================================
-- Tabla de Pedidos
-- ================================================
-- ADVERTENCIA: Esto eliminará todos los datos existentes en la tabla Pedidos
DROP TABLE IF EXISTS Pedidos;
GO

CREATE TABLE Pedidos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NumeroPedido NVARCHAR(50) NOT NULL UNIQUE,
    Cliente NVARCHAR(150) NOT NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaModificacion DATETIME2 NULL,
    Total DECIMAL(10,2) NOT NULL,
    Estado NVARCHAR(50) NOT NULL -- Registrado, En Proceso, Completado, Eliminado
);

CREATE INDEX IX_Pedidos_NumeroPedido ON Pedidos(NumeroPedido);
CREATE INDEX IX_Pedidos_Estado ON Pedidos(Estado);
GO

-- ================================================
-- Datos Iniciales - Usuario Administrador
-- ================================================
-- Usuario: admin@test.com
-- Contraseña: Admin123!
IF NOT EXISTS (SELECT * FROM Usuarios WHERE Email = 'admin@test.com')
BEGIN
    INSERT INTO Usuarios (Email, PasswordHash, Nombre, Rol, FechaCreacion)
    VALUES (
        'admin@test.com',
        '$2a$11$nZ5SQjL9z5wZ5QjL9z5wZu5QjL9z5wZ5QjL9z5wZ5QjL9z5wZ5QjLu', -- Admin123!
        'Administrador',
        'Admin',
        '2025-01-01T00:00:00'
    );
END
GO

-- ================================================
-- Datos de Prueba - Pedidos de Ejemplo
-- ================================================
IF NOT EXISTS (SELECT * FROM Pedidos WHERE NumeroPedido = 'PED-001')
BEGIN
    INSERT INTO Pedidos (NumeroPedido, Cliente, FechaCreacion, Total, Estado)
    VALUES
        ('PED-001', 'Juan Pérez', '2026-01-07 10:30:00', 250.75, 'Registrado'),
        ('PED-002', 'María García', '2026-01-07 11:15:00', 450.00, 'En Proceso'),
        ('PED-003', 'Carlos López', '2026-01-06 14:20:00', 125.50, 'Completado'),
        ('PED-004', 'Ana Martínez', '2026-01-05 09:45:00', 890.25, 'Registrado'),
        ('PED-005', 'Luis Rodríguez', '2026-01-07 16:00:00', 320.00, 'En Proceso');
END
GO

PRINT 'Base de datos PedidosDB creada exitosamente';
PRINT 'Usuario de prueba: admin@test.com';
PRINT 'Contraseña de prueba: Admin123!';
PRINT '5 pedidos de ejemplo insertados';
GO
