IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Departamentos] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Departamentos] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Sexo] nvarchar(max) NULL,
    [RG] nvarchar(max) NULL,
    [CPF] nvarchar(max) NOT NULL,
    [NomeMae] nvarchar(max) NULL,
    [SituacaoCadastro] nvarchar(max) NULL,
    [DataCadastro] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Contatos] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [Telefone] nvarchar(max) NULL,
    [Celular] nvarchar(max) NULL,
    CONSTRAINT [PK_Contatos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Contatos_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [DepartamentoUsuario] (
    [DepartamentosId] int NOT NULL,
    [UsuariosId] int NOT NULL,
    CONSTRAINT [PK_DepartamentoUsuario] PRIMARY KEY ([DepartamentosId], [UsuariosId]),
    CONSTRAINT [FK_DepartamentoUsuario_Departamentos_DepartamentosId] FOREIGN KEY ([DepartamentosId]) REFERENCES [Departamentos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DepartamentoUsuario_Usuarios_UsuariosId] FOREIGN KEY ([UsuariosId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EnderecoEntregas] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [NomeEndereco] nvarchar(max) NOT NULL,
    [CEP] nvarchar(max) NOT NULL,
    [Estado] nvarchar(max) NOT NULL,
    [Cidade] nvarchar(max) NOT NULL,
    [Bairro] nvarchar(max) NOT NULL,
    [Endereco] nvarchar(max) NOT NULL,
    [Numero] nvarchar(max) NULL,
    [Complemento] nvarchar(max) NULL,
    CONSTRAINT [PK_EnderecoEntregas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EnderecoEntregas_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Contatos_UsuarioId] ON [Contatos] ([UsuarioId]);
GO

CREATE INDEX [IX_DepartamentoUsuario_UsuariosId] ON [DepartamentoUsuario] ([UsuariosId]);
GO

CREATE INDEX [IX_EnderecoEntregas_UsuarioId] ON [EnderecoEntregas] ([UsuarioId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230403224830_BancoInicial', N'6.0.15');
GO

COMMIT;
GO

