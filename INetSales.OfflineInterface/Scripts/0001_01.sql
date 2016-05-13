--SQLite Maestro 9.10.0.4
------------------------------------------
--Host     : localhost
--Database : C:\Projetos\Mobile\INetSales\Trunk\Data\inetsales.db


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
  DataLastUpload   datetime
);

CREATE UNIQUE INDEX IX0_CLIENTE
  ON TCliente
  (Documento);

CREATE TABLE TCondicaoPagamento (
  CondicaoId     integer PRIMARY KEY NOT NULL,
  Codigo         text,
  Descricao      text,
  IsDefault      boolean,
  IsBoleto       boolean,
  IsCheque       boolean,
  DataCriacao    datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  DataAlteracao  datetime
);

CREATE TABLE TConfiguracao (
  ConfiguracaoId     integer PRIMARY KEY NOT NULL,
  UrlWebService      text,
  ChaveIntegracao    text,
  CodigoTabelaPreco  text,
  UrlSiteERP         text,
  IsDesabilitado     boolean,
  DataCriacao        datetime
);

CREATE TABLE TEstoque (
  EstoqueId             integer PRIMARY KEY NOT NULL,
  Codigo                text,
  ProdutoId             integer NOT NULL,
  RemessaId             integer NOT NULL,
  UsuarioId             integer NOT NULL,
  QuantidadeInicial     integer NOT NULL,
  QuantidadeDisponivel  integer NOT NULL,
  IsAcerto              boolean,
  DataEntrega           datetime NOT NULL,
  DataCriacao           datetime NOT NULL,
  DataAlteracao         datetime
);

CREATE UNIQUE INDEX TEstoque_UX01
  ON TEstoque
  (ProdutoId, RemessaId);

CREATE TABLE TGrupo (
  GrupoId     integer PRIMARY KEY NOT NULL,
  GrupoPaiId  integer,
  Codigo      text,
  Nome        text NOT NULL
);

CREATE TABLE TIntegra (
  IntegraId          int PRIMARY KEY NOT NULL,
  Codigo             text NOT NULL,
  DataInicio         datetime NOT NULL,
  Intervalo          text NOT NULL,
  DataUltimaIntegra  datetime
);

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
  DataLastUpload   datetime
);

CREATE TABLE TPedidoEstoque (
  PedidoId              integer NOT NULL,
  EstoqueId             integer NOT NULL,
  ProdutoId             integer NOT NULL,
  QuantidadePedido      integer NOT NULL,
  ValorUnitarioProduto  number NOT NULL
);

CREATE TABLE TPedidoPagamento (
  PagamentoId          integer PRIMARY KEY NOT NULL,
  PedidoId             integer NOT NULL,
  CondicaoPagamentoId  integer NOT NULL,
  Valor                number NOT NULL
);

CREATE TABLE TPedidoPagamentoChequeInfo (
  PagamentoId  integer PRIMARY KEY NOT NULL,
  Numero       text,
  Agencia      text,
  Banco        text
);

CREATE TABLE TPedidoProduto (
  PedidoId              integer NOT NULL,
  ProdutoId             integer NOT NULL,
  ValorUnitarioVendido  number NOT NULL,
  DescontoPercent       number
);

CREATE TABLE TProduto (
  ProdutoId      integer PRIMARY KEY NOT NULL,
  GrupoId        integer NOT NULL,
  Codigo         text NOT NULL,
  Nome           text NOT NULL,
  ValorUnitario  number,
  SaldoAdm       integer
);

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
  DataLastUpload   datetime
);

CREATE TABLE TRota (
  RotaId       integer PRIMARY KEY NOT NULL,
  Dia          datetime,
  Nome         text,
  Pasta        integer,
  UsuarioId    integer,
  DataCriacao  datetime
);

CREATE INDEX IX_ROTEIRO
  ON TRota
  (Dia DESC, UsuarioId);

CREATE TABLE TRotaCliente (
  RotaId              integer NOT NULL,
  ClienteId           integer NOT NULL,
  Ordem               integer NOT NULL,
  IsAtivoRoteiro      bool NOT NULL,
  HasPedido           bool,
  IsPermitidoForaDia  boolean,
  DataCriacao         datetime,
  DataAlteracao       datetime,
  PRIMARY KEY (RotaId, ClienteId, Ordem, IsAtivoRoteiro)
);

CREATE TABLE TUsuario (
  UsuarioId       integer PRIMARY KEY NOT NULL,
  Nome            text NOT NULL,
  Username        text NOT NULL,
  SenhaHash       text NOT NULL,
  Codigo          text NOT NULL,
  PlacaVeiculo    text,
  IsAdm           boolean,
  DataCriacao     datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  DataAlteracao   datetime,
  IsDesabilitado  boolean NOT NULL DEFAULT true
);

/* Data for table TCliente */




/* Data for table TCondicaoPagamento */



/* Data for table TConfiguracao */
INSERT INTO TConfiguracao(ConfiguracaoId, UrlWebService, ChaveIntegracao, CodigoTabelaPreco, UrlSiteERP, IsDesabilitado, DataCriacao) 
VALUES (1, 'http://www.startsoftware.com.br:8080/integratornet/IntegraWS?wsdl', 'cd829da5-bcc4-47f4-97b4-3dad157eb419', '939', 'www.startsoftware.com.br:8080/integratornet/', 0, '2012-11-21 01:49:46');



/* Data for table TEstoque */




/* Data for table TGrupo */




/* Data for table TIntegra */




/* Data for table TPedido */




/* Data for table TPedidoEstoque */




/* Data for table TPedidoPagamento */




/* Data for table TPedidoPagamentoChequeInfo */




/* Data for table TPedidoProduto */




/* Data for table TProduto */




/* Data for table TRemessa */




/* Data for table TRota */




/* Data for table TRotaCliente */




/* Data for table TUsuario */


