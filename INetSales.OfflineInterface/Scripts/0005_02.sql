-- Add foreingkeys na tabela TGrupo

CREATE TABLE TGrupo01 (
  GrupoId     ,
  GrupoPaiId  ,
  Codigo      ,
  Nome        
);

INSERT INTO TGrupo01
SELECT
  GrupoId     ,
  GrupoPaiId  ,
  Codigo      ,
  Nome        
FROM TGrupo;

DROP TABLE TGrupo;

CREATE TABLE TGrupo (
  GrupoId     integer PRIMARY KEY NOT NULL,
  GrupoPaiId  integer,
  Codigo      text,
  Nome        text NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (GrupoPaiId)
    REFERENCES TGrupo(GrupoId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

INSERT INTO TGrupo
  (GrupoId     ,
  GrupoPaiId  ,
  Codigo      ,
  Nome        )
SELECT
  GrupoId     ,
  GrupoPaiId  ,
  Codigo      ,
  Nome        
FROM TGrupo01;

DROP TABLE TGrupo01;