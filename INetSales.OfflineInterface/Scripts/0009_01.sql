ALTER TABLE TConfiguracao ADD COLUMN IsPrimeiroAcesso boolean DEFAULT true;
UPDATE TConfiguracao SET IsPrimeiroAcesso = 1;