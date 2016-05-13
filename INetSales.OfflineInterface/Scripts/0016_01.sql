-- D0003

ALTER TABLE TConfiguracao ADD COLUMN IsPreVenda boolean;
ALTER TABLE TCondicaoPagamento ADD COLUMN IsPublica boolean;

-- Tabela de pendencias do cliente
CREATE TABLE TPendencia (
  PendenciaId      integer PRIMARY KEY NOT NULL,
  ClienteId      integer NOT NULL,
  Codigo           text,
  Documento          text NOT NULL,
  ValorTotal          number NOT NULL,
  ValorEmAberto          number NOT NULL,
  DataEmissao    datetime,
  DataVencimento    datetime,
  LinkPagamento          text NOT NULL,
  DataCriacao    datetime NOT NULL,
  DataAlteracao  datetime,
  /* Foreign keys */
  FOREIGN KEY (ClienteId)
    REFERENCES TCliente(ClienteId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);