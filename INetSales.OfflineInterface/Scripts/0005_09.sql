-- Add foreingkeys na tabela TPedidoProduto

CREATE TABLE TPedidoProduto01 (
  PedidoId              ,
  ProdutoId             ,
  ValorUnitarioVendido  ,
  DescontoPercent       ,
  QuantidadePedido      
);

INSERT INTO TPedidoProduto01
SELECT
  PedidoId              ,
  ProdutoId             ,
  ValorUnitarioVendido  ,
  DescontoPercent       ,
  QuantidadePedido      
FROM TPedidoProduto;

DROP TABLE TPedidoProduto;

CREATE TABLE TPedidoProduto (
  PedidoId              integer NOT NULL,
  ProdutoId             integer NOT NULL,
  ValorUnitarioVendido  number NOT NULL,
  DescontoPercent       number,
  QuantidadePedido      integer NOT NULL DEFAULT 0,
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

INSERT INTO TPedidoProduto
  (PedidoId              ,
  ProdutoId             ,
  ValorUnitarioVendido  ,
  DescontoPercent       ,
  QuantidadePedido      )
SELECT
  PedidoId              ,
  ProdutoId             ,
  ValorUnitarioVendido  ,
  DescontoPercent       ,
  QuantidadePedido      
FROM TPedidoProduto01;

DROP TABLE TPedidoProduto01;