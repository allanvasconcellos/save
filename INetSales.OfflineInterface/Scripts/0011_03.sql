-- Tabela de saldo do produto para o usuário

CREATE TABLE TProdutoSaldo (
  ProdutoId      integer NOT NULL,
  UsuarioId      integer NOT NULL,
  Saldo          integer NOT NULL,
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