-- Add foreingkeys na tabela TRemessa

CREATE TABLE TRemessa01 (
  RemessaId        ,
  UsuarioId        ,
  Codigo           ,
  Descricao        ,
  QuantidadeTotal  ,
  DataPedido       ,
  DataEntrega      ,
  IsSos            ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   
);

INSERT INTO TRemessa01
SELECT
  RemessaId        ,
  UsuarioId        ,
  Codigo           ,
  Descricao        ,
  QuantidadeTotal  ,
  DataPedido       ,
  DataEntrega      ,
  IsSos            ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   
FROM TRemessa;

DROP TABLE TRemessa;

CREATE TABLE TRemessa (
  RemessaId        integer PRIMARY KEY NOT NULL,
  UsuarioId        integer NOT NULL,
  Codigo           text NOT NULL,
  Descricao        text,
  QuantidadeTotal  integer NOT NULL,
  DataPedido       datetime NOT NULL,
  DataEntrega      datetime NOT NULL,
  IsSos            boolean NOT NULL,
  DataCriacao      datetime NOT NULL,
  DataAlteracao    datetime,
  IsPendingUpload  bool NOT NULL,
  DataLastUpload   datetime,
  /* Foreign keys */
  FOREIGN KEY (UsuarioId)
    REFERENCES TUsuario(UsuarioId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

INSERT INTO TRemessa
  (RemessaId        ,
  UsuarioId        ,
  Codigo           ,
  Descricao        ,
  QuantidadeTotal  ,
  DataPedido       ,
  DataEntrega      ,
  IsSos            ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   )
SELECT
  RemessaId        ,
  UsuarioId        ,
  Codigo           ,
  Descricao        ,
  QuantidadeTotal  ,
  DataPedido       ,
  DataEntrega      ,
  IsSos            ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   
FROM TRemessa01;

DROP TABLE TRemessa01;