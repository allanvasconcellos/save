-- Add foreingkeys na tabela TEstoque

CREATE TABLE TEstoque01(
  EstoqueId,
  Codigo,
  ProdutoId,
  RemessaId,
  UsuarioId,
  QuantidadeInicial,
  QuantidadeDisponivel,
  IsAcerto,
  DataEntrega,
  DataCriacao,
  DataAlteracao
);

INSERT INTO TEstoque01
SELECT
  EstoqueId,
  Codigo,
  ProdutoId,
  RemessaId,
  UsuarioId,
  QuantidadeInicial,
  QuantidadeDisponivel,
  IsAcerto,
  DataEntrega,
  DataCriacao,
  DataAlteracao
FROM TEstoque;

DROP TABLE TEstoque;

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
  DataAlteracao         datetime,
  /* Foreign keys */
  FOREIGN KEY (ProdutoId)
    REFERENCES TProduto(ProdutoId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  FOREIGN KEY (UsuarioId)
    REFERENCES TUsuario(UsuarioId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  FOREIGN KEY (RemessaId)
    REFERENCES TRemessa(RemessaId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

CREATE INDEX TEstoque_IXProduto01
  ON TEstoque
  (ProdutoId);

CREATE INDEX TEstoque_IXUsuario01
  ON TEstoque
  (UsuarioId);

CREATE UNIQUE INDEX TEstoque_UX01
  ON TEstoque
  (ProdutoId, RemessaId);

INSERT INTO TEstoque
  (EstoqueId,
  Codigo,
  ProdutoId,
  RemessaId,
  UsuarioId,
  QuantidadeInicial,
  QuantidadeDisponivel,
  IsAcerto,
  DataEntrega,
  DataCriacao,
  DataAlteracao)
SELECT
  EstoqueId,
  Codigo,
  ProdutoId,
  RemessaId,
  UsuarioId,
  QuantidadeInicial,
  QuantidadeDisponivel,
  IsAcerto,
  DataEntrega,
  DataCriacao,
  DataAlteracao
FROM TEstoque01;

DROP TABLE TEstoque01;