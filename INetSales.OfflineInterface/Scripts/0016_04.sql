-- D0003

CREATE TABLE TPedidoProduto_temp (
  PedidoId              integer NOT NULL,
  ProdutoId             integer NOT NULL,
  ValorUnitarioVendido  number NOT NULL,
  DescontoPercent       number,
  QuantidadePedido      number NOT NULL DEFAULT 0,
  /* Foreign keys */
  FOREIGN KEY (ProdutoId)
    REFERENCES TProduto(ProdutoId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION, 
  FOREIGN KEY (PedidoId)
    REFERENCES TPedido(PedidoId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

INSERT INTO TPedidoProduto_temp SELECT * FROM TPedidoProduto;

DROP TABLE TPedidoProduto;

ALTER TABLE TPedidoProduto_temp RENAME TO TPedidoProduto;