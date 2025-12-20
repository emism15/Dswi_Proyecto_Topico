esto es lo que tengo hasta ahora:
CREATE TABLE [dbo].[Alumno] (
    [AlumnoId]        INT           IDENTITY (1, 1) NOT NULL,
    [Codigo]          VARCHAR (20)  NOT NULL,
    [NombreCompleto]  VARCHAR (120) NOT NULL,
    [FechaNacimiento] DATE          NOT NULL,
    [Edad]            INT           NOT NULL,
    [DNI]             CHAR (8)      NOT NULL,
    [Telefono]        VARCHAR (20)  NULL,
    [Correo]          VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([AlumnoId] ASC),
    UNIQUE NONCLUSTERED ([Codigo] ASC)
);

---------------------------------------------------------------------------------
CREATE TABLE [dbo].[Atencion] (
    [AtencionId]  INT           IDENTITY (1, 1) NOT NULL,
    [AlumnoId]    INT           NOT NULL,
    [Fecha]       DATE          NOT NULL,
    [Hora]        TIME (7)      NOT NULL,
    [Detalles]    VARCHAR (MAX) NOT NULL,
    [Diagnostico] VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([AtencionId] ASC),
    FOREIGN KEY ([AlumnoId]) REFERENCES [dbo].[Alumno] ([AlumnoId])
);
-----------------------------------------------------------------------------
CREATE TABLE [dbo].[AtencionMedicamento] (
    [AtencionMedicamentoId] INT IDENTITY (1, 1) NOT NULL,
    [AtencionId]            INT NOT NULL,
    [MedicamentoId]         INT NOT NULL,
    [Cantidad]              INT NOT NULL,
    PRIMARY KEY CLUSTERED ([AtencionMedicamentoId] ASC),
    FOREIGN KEY ([AtencionId]) REFERENCES [dbo].[Atencion] ([AtencionId]),
    FOREIGN KEY ([MedicamentoId]) REFERENCES [dbo].[Medicamento] ([MedicamentoId])
);
-------------------------------------------------------------------------------
 CREATE TABLE [dbo].[Medicamento] (
    [MedicamentoId] INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]        VARCHAR (100) NOT NULL,
    [Stock]         INT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([MedicamentoId] ASC)
);


ahora si desde aquí podemos avanzar teniendo en cuenta como trabaja mi profesor y también considerando lo que quiero hacer y lo que te pido "Registrar Alumno", "Registrar Atencion"







