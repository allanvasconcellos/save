-- D0003

CREATE TABLE TProdutoHistorico_temp (
  ProdutoId         integer NOT NULL,
  UsuarioId         integer NOT NULL,
  QuantidadeAntiga  number NOT NULL,
  QuantidadeNova    number NOT NULL,
  Valor             number NOT NULL,
  Motivo            text,
  DataCriacao       datetime NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (ProdutoId)
    REFERENCES TProduto(ProdutoId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION, 
  FOREIGN KEY (UsuarioId)
    REFERENCES TUsuario(UsuarioId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

INSERT INTO TProdutoHistorico_temp SELECT * FROM TProdutoHistorico;

DROP TABLE TProdutoHistorico;

ALTER TABLE TProdutoHistorico_temp RENAME TO TProdutoHistorico;