-- D0003

CREATE TABLE TProdutoSaldo_temp (
  ProdutoId      integer NOT NULL,
  UsuarioId      integer NOT NULL,
  Saldo          number NOT NULL,
  DataCriacao    datetime NOT NULL,
  DataAlteracao  datetime,
  PRIMARY KEY (ProdutoId, UsuarioId),
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

INSERT INTO TProdutoSaldo_temp SELECT * FROM TProdutoSaldo;

DROP TABLE TProdutoSaldo;

ALTER TABLE TProdutoSaldo_temp RENAME TO TProdutoSaldo;