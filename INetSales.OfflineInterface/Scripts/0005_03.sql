-- Add foreingkeys na tabela TProduto

CREATE TABLE TProduto01 (
  ProdutoId,
  GrupoId,
  Codigo,
  Nome,
  ValorUnitario,
  SaldoAdm
);

INSERT INTO TProduto01
SELECT
  ProdutoId,
  GrupoId,
  Codigo,
  Nome,
  ValorUnitario,
  SaldoAdm
FROM TProduto;

DROP TABLE TProduto;

CREATE TABLE TProduto (
  ProdutoId      integer PRIMARY KEY NOT NULL,
  GrupoId        integer NOT NULL,
  Codigo         text NOT NULL,
  Nome           text NOT NULL,
  ValorUnitario  number,
  SaldoAdm       integer,
  /* Foreign keys */
  FOREIGN KEY (GrupoId)
    REFERENCES TGrupo(GrupoId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

INSERT INTO TProduto
  (ProdutoId,
  GrupoId,
  Codigo,
  Nome,
  ValorUnitario,
  SaldoAdm)
SELECT
  ProdutoId,
  GrupoId,
  Codigo,
  Nome,
  ValorUnitario,
  SaldoAdm
FROM TProduto01;

DROP TABLE TProduto01;