DELETE FROM TPedidoEstoque;
DELETE FROM TEstoque;
DELETE FROM TRemessa;

UPDATE TConfiguracao SET CampoEspecie = 'displays';
UPDATE TConfiguracao SET CampoMarca = 'Kraft';