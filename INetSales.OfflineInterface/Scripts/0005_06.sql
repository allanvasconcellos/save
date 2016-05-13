-- Add foreingkeys na tabela TCliente

CREATE TABLE TCliente01 (
  ClienteId        ,
  UsuarioId        ,
  Codigo           ,
  RazaoSocial      ,
  NomeFantasia     ,
  Documento        ,
  TipoPessoa       ,
  HasPendencia     ,
  IsAtivoAdm       ,
  IsDesabilitado   ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   
);

INSERT INTO TCliente01
SELECT
  ClienteId        ,
  UsuarioId        ,
  Codigo           ,
  RazaoSocial      ,
  NomeFantasia     ,
  Documento        ,
  TipoPessoa       ,
  HasPendencia     ,
  IsAtivoAdm       ,
  IsDesabilitado   ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   
FROM TCliente;

DROP TABLE TCliente;

CREATE TABLE TCliente (
  ClienteId        integer PRIMARY KEY NOT NULL,
  UsuarioId        integer NOT NULL,
  Codigo           text,
  RazaoSocial      text NOT NULL,
  NomeFantasia     text,
  Documento        text NOT NULL,
  TipoPessoa       char(1) NOT NULL,
  HasPendencia     boolean NOT NULL,
  IsAtivoAdm       boolean NOT NULL DEFAULT false,
  IsDesabilitado   boolean NOT NULL DEFAULT false,
  DataCriacao      datetime NOT NULL DEFAULT CURRENT_DATE,
  DataAlteracao    datetime,
  IsPendingUpload  boolean NOT NULL,
  DataLastUpload   datetime,
  /* Foreign keys */
  FOREIGN KEY (UsuarioId)
    REFERENCES TUsuario(UsuarioId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

CREATE UNIQUE INDEX TCliente_IXDocumento01
  ON TCliente
  (Documento);

INSERT INTO TCliente
  (ClienteId        ,
  UsuarioId        ,
  Codigo           ,
  RazaoSocial      ,
  NomeFantasia     ,
  Documento        ,
  TipoPessoa       ,
  HasPendencia     ,
  IsAtivoAdm       ,
  IsDesabilitado   ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   )
SELECT
  ClienteId        ,
  UsuarioId        ,
  Codigo           ,
  RazaoSocial      ,
  NomeFantasia     ,
  Documento        ,
  TipoPessoa       ,
  HasPendencia     ,
  IsAtivoAdm       ,
  IsDesabilitado   ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   
FROM TCliente01;

DROP TABLE TCliente01;