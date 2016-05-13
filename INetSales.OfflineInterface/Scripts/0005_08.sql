-- Add foreingkeys na tabela TPedido

CREATE TABLE TPedido01 (
  PedidoId         ,
  UsuarioId        ,
  ClienteId        ,
  RoteiroId        ,
  Codigo           ,
  NFe              ,
  UrlHttpNFe       ,
  UrlLocalNFe      ,
  TipoPedido       ,
  UrlHttpBoleto    ,
  UrlLocalBoleto   ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   ,
  IsCancelado      ,
  IsRejeitado      
);

INSERT INTO TPedido01
SELECT
  PedidoId         ,
  UsuarioId        ,
  ClienteId        ,
  RoteiroId        ,
  Codigo           ,
  NFe              ,
  UrlHttpNFe       ,
  UrlLocalNFe      ,
  TipoPedido       ,
  UrlHttpBoleto    ,
  UrlLocalBoleto   ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   ,
  IsCancelado      ,
  IsRejeitado      
FROM TPedido;

DROP TABLE TPedido;

CREATE TABLE TPedido (
  PedidoId         integer PRIMARY KEY NOT NULL,
  UsuarioId        integer NOT NULL,
  ClienteId        integer,
  RoteiroId        integer,
  Codigo           text,
  NFe              text,
  UrlHttpNFe       text,
  UrlLocalNFe      text,
  TipoPedido       char(1) NOT NULL,
  UrlHttpBoleto    text,
  UrlLocalBoleto   text,
  DataCriacao      datetime NOT NULL,
  DataAlteracao    datetime,
  IsPendingUpload  bool NOT NULL,
  DataLastUpload   datetime,
  IsCancelado      boolean,
  IsRejeitado      boolean,
  /* Foreign keys */
  FOREIGN KEY (RoteiroId)
    REFERENCES TRota(RotaId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION, 
  FOREIGN KEY (UsuarioId)
    REFERENCES TUsuario(UsuarioId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION, 
  FOREIGN KEY (ClienteId)
    REFERENCES TCliente(ClienteId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

INSERT INTO TPedido
  (PedidoId         ,
  UsuarioId        ,
  ClienteId        ,
  RoteiroId        ,
  Codigo           ,
  NFe              ,
  UrlHttpNFe       ,
  UrlLocalNFe      ,
  TipoPedido       ,
  UrlHttpBoleto    ,
  UrlLocalBoleto   ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   ,
  IsCancelado      ,
  IsRejeitado      )
SELECT
  PedidoId         ,
  UsuarioId        ,
  ClienteId        ,
  RoteiroId        ,
  Codigo           ,
  NFe              ,
  UrlHttpNFe       ,
  UrlLocalNFe      ,
  TipoPedido       ,
  UrlHttpBoleto    ,
  UrlLocalBoleto   ,
  DataCriacao      ,
  DataAlteracao    ,
  IsPendingUpload  ,
  DataLastUpload   ,
  IsCancelado      ,
  IsRejeitado      
FROM TPedido01;

DROP TABLE TPedido01;