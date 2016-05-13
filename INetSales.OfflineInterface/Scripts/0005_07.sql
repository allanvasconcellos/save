-- Add foreingkeys na tabela TRotaCliente

CREATE TABLE TRotaCliente01 (
  RotaId              ,
  ClienteId           ,
  Ordem               ,
  IsAtivoRoteiro      ,
  HasPedido           ,
  IsPermitidoForaDia  ,
  DataCriacao         ,
  DataAlteracao		
);

INSERT INTO TRotaCliente01
SELECT
  RotaId              ,
  ClienteId           ,
  Ordem               ,
  IsAtivoRoteiro      ,
  HasPedido           ,
  IsPermitidoForaDia  ,
  DataCriacao         ,
  DataAlteracao		
FROM TRotaCliente;

DROP TABLE TRotaCliente;

CREATE TABLE TRotaCliente (
  RotaId              integer NOT NULL,
  ClienteId           integer NOT NULL,
  Ordem               integer NOT NULL,
  IsAtivoRoteiro      bool NOT NULL,
  HasPedido           bool,
  IsPermitidoForaDia  boolean,
  DataCriacao         datetime,
  DataAlteracao       datetime,
  PRIMARY KEY (RotaId, ClienteId, Ordem, IsAtivoRoteiro),
  /* Foreign keys */
  FOREIGN KEY (ClienteId)
    REFERENCES TCliente(ClienteId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION, 
  FOREIGN KEY (RotaId)
    REFERENCES TRota(RotaId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

INSERT INTO TRotaCliente
  (RotaId              ,
  ClienteId           ,
  Ordem               ,
  IsAtivoRoteiro      ,
  HasPedido           ,
  IsPermitidoForaDia  ,
  DataCriacao         ,
  DataAlteracao		 )
SELECT
  RotaId              ,
  ClienteId           ,
  Ordem               ,
  IsAtivoRoteiro      ,
  HasPedido           ,
  IsPermitidoForaDia  ,
  DataCriacao         ,
  DataAlteracao		
FROM TRotaCliente01;

DROP TABLE TRotaCliente01;