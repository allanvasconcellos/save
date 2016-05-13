--Tabela historica de produto para registrar as mudanças.

CREATE TABLE TProdutoHistorico (
  ProdutoId         integer NOT NULL,
  UsuarioId         integer NOT NULL,
  QuantidadeAntiga  integer NOT NULL,
  QuantidadeNova    integer NOT NULL,
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